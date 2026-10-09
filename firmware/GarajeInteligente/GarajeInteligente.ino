#include <Servo.h>
#include <EEPROM.h>
#include "control.h"

// UNO R3, 115200 baud. 5 V external servo supply; common ground.
const uint16_t MAGIC=0x4701;
const int STOP_ADDR=128;
struct Record {uint16_t magic; Config config; uint16_t crc;};
DoorControl door;
Servo motors[2];
char line[190]; uint8_t used=0; bool overflow=false;
uint32_t lastSense=0,lastStatus=0,bootAt=0,irChanged=0;
bool irRaw=false,irStable=false,bootDone=false,dirty=false,stopped=false;
int16_t lastPulse[2]={-1,-1};
uint16_t crc16(const uint8_t *p,uint16_t n) {
  uint16_t crc=0xffff;while(n--){crc^=*p++;for(uint8_t i=0;i<8;i++)crc=(crc>>1)^((crc&1)?0xa001:0);}return crc;
}
bool loadConfig(Config &c) {
  Record r;EEPROM.get(0,r);
  if(r.magic!=MAGIC || r.crc!=crc16((const uint8_t*)&r.config,sizeof(Config)) || !validConfig(r.config))return false;
  c=r.config;return true;
}
void saveConfig(){Record r={MAGIC,door.cfg,0};r.crc=crc16((const uint8_t*)&r.config,sizeof(Config));EEPROM.put(0,r);dirty=false;}
void detachAll(){for(uint8_t i=0;i<2;i++){motors[i].detach();lastPulse[i]=-1;}}
void setupPins(){pinMode(door.cfg.v[TRIG],OUTPUT);digitalWrite(door.cfg.v[TRIG],LOW);pinMode(door.cfg.v[ECHO],INPUT);pinMode(door.cfg.v[IR],INPUT_PULLUP);}
void releasePins(){detachAll();for(uint8_t i=TRIG;i<=SERVO2;i++){pinMode(door.cfg.v[i],INPUT);digitalWrite(door.cfg.v[i],LOW);}}
void setPulse(uint8_t i,uint16_t pulse) {
  motors[i].writeMicroseconds(pulse); // preload before attaching; no implicit 90-degree command
  if(!motors[i].attached())motors[i].attach(door.cfg.v[i?SERVO2:SERVO1],700,2300);
  motors[i].writeMicroseconds(pulse);lastPulse[i]=pulse;
}
void drive(){
  if(door.phase==MAINTENANCE || door.position<0)return;
  for(uint8_t i=0;i<door.cfg.v[COUNT];i++) {
    int a=door.cfg.v[i?S2_CLOSED:S1_CLOSED],b=door.cfg.v[i?S2_OPEN:S1_OPEN];
    int pulse=a+((long)(b-a)*door.position)/1000;
    if(pulse!=lastPulse[i] || !motors[i].attached())setPulse(i,pulse);
  }
}
void ack(uint16_t id,bool ok,const __FlashStringHelper *msg){
  Serial.print(F("{\"type\":\"ack\",\"id\":"));Serial.print(id);Serial.print(F(",\"ok\":"));Serial.print(ok?F("true"):F("false"));Serial.print(F(",\"message\":\""));Serial.print(msg);Serial.println(F("\"}"));
}
void configOut(){Serial.print(F("{\"type\":\"config\",\"values\":["));for(uint8_t i=0;i<NFIELDS;i++){if(i)Serial.print(',');Serial.print(door.cfg.v[i]);}Serial.println(F("]}"));}
void status(){
  Serial.print(F("{\"type\":\"status\",\"fw\":\"GARAGE-1.0.0\",\"phase\":"));Serial.print((int)door.phase);
  Serial.print(F(",\"position\":"));Serial.print(door.position);
  Serial.print(F(",\"valid\":"));Serial.print(door.echoValid?F("true"):F("false"));
  Serial.print(F(",\"cm\":"));Serial.print(door.distance);
  Serial.print(F(",\"ir\":"));Serial.print(irStable?F("true"):F("false"));
  Serial.print(F(",\"ir_raw\":"));Serial.print(digitalRead(door.cfg.v[IR]));
  Serial.print(F(",\"vehicle\":"));Serial.print(door.vehicle?F("true"):F("false"));
  Serial.print(F(",\"clear\":"));Serial.print(door.clear?F("true"):F("false"));
  Serial.print(F(",\"entry\":"));Serial.print(door.entry?F("true"):F("false"));
  Serial.print(F(",\"remaining\":"));Serial.print(door.remaining);
  Serial.print(F(",\"dirty\":"));Serial.print(dirty?F("true"):F("false"));
  Serial.print(F(",\"stopped\":"));Serial.print(stopped?F("true"):F("false"));
  Serial.print(F(",\"attached\": ["));Serial.print(motors[0].attached()?1:0);Serial.print(',');Serial.print(motors[1].attached()?1:0);
  Serial.print(F("],\"pulse\":["));Serial.print(lastPulse[0]);Serial.print(',');Serial.print(lastPulse[1]);Serial.println(F("]}"));
}
bool number(const char *s,uint16_t &v){if(!s || !*s)return false;uint32_t n=0;while(*s){if(*s<'0'||*s>'9')return false;n=n*10+(*s++-'0');if(n>65535)return false;}v=n;return true;}
void command(char *s){
  char *ctx;uint16_t id;
  if(!number(strtok_r(s," ",&ctx),id))return;
  char *cmd=strtok_r(NULL," ",&ctx);if(!cmd){ack(id,false,F("COMANDO_INVALIDO"));return;}
  uint32_t now=millis();
  if(!strcmp(cmd,"CONFIG")){
    if(door.phase!=MAINTENANCE){ack(id,false,F("REQUIERE_MANTENIMIENTO"));return;}
    Config c;
    for(uint8_t i=0;i<NFIELDS;i++)if(!number(strtok_r(NULL," ",&ctx),c.v[i])){ack(id,false,F("CONFIG_INCOMPLETA"));return;}
    if(strtok_r(NULL," ",&ctx)||!validConfig(c)){ack(id,false,F("CONFIG_INVALIDA"));return;}
    releasePins();door.cfg=c;door.maintenance();setupPins();dirty=true;irRaw=irStable=false;irChanged=now;
    ack(id,true,F("APLICADA_EN_RAM"));configOut();return;
  }
  if(!strcmp(cmd,"JOG")){
    uint16_t servo,pulse;
    if(door.phase!=MAINTENANCE || !number(strtok_r(NULL," ",&ctx),servo) || !number(strtok_r(NULL," ",&ctx),pulse) || strtok_r(NULL," ",&ctx) || servo<1 || servo>door.cfg.v[COUNT] || pulse<700 || pulse>2300){ack(id,false,F("PRUEBA_INVALIDA"));return;}
    setPulse(servo-1,pulse);ack(id,true,F("PULSO_ORDENADO_NO_MEDIDO"));return;
  }
  if(strtok_r(NULL," ",&ctx)){ack(id,false,F("ARGUMENTOS_EXTRA"));return;}
  if(!strcmp(cmd,"GET")){configOut();status();ack(id,true,F("CONFIG_Y_ESTADO"));}
  else if(!strcmp(cmd,"PING"))ack(id,true,F("GARAGE-1.0.0"));
  else if(!strcmp(cmd,"STOP")){door.pause();stopped=true;EEPROM.update(STOP_ADDR,1);bootDone=true;ack(id,true,F("PAUSA_ENCLAVADA_MANTIENE_PULSOS"));}
  else if(!strcmp(cmd,"RESUME")){
    if(door.resume(now)){stopped=false;EEPROM.update(STOP_ADDR,0);bootDone=true;ack(id,true,F("REANUDA_ABRIENDO"));}else ack(id,false,F("CALIBRAR_O_SALIR_MANTENIMIENTO"));
  }
  else if(!strcmp(cmd,"OPEN")){bootDone=true;ack(id,door.open(now),F("ABRIR_REQUIERE_CALIBRACION_Y_SIN_PAUSA"));}
  else if(!strcmp(cmd,"CLOSE")){ack(id,door.close(now),F("CERRAR_REQUIERE_ABIERTA_Y_EXTERIOR_LIBRE"));}
  else if(!strcmp(cmd,"MAINT")){stopped=true;EEPROM.update(STOP_ADDR,1);bootDone=true;detachAll();door.maintenance();ack(id,true,F("SIN_PULSOS_SOSTENER_PUERTA"));}
  else if(!strcmp(cmd,"EXIT")){
    if(door.phase!=MAINTENANCE){ack(id,false,F("REQUIERE_MANTENIMIENTO"));return;}
    detachAll();door.leaveMaintenance();door.pause();ack(id,true,F("LISTA_PARA_REANUDAR"));
  }
  else if(!strcmp(cmd,"SAVE")){
    if(door.phase!=MAINTENANCE){ack(id,false,F("REQUIERE_MANTENIMIENTO"));return;}
    saveConfig();ack(id,true,F("GUARDADA_EN_EEPROM"));
  }
  else if(!strcmp(cmd,"LOAD") || !strcmp(cmd,"DEFAULTS")){
    if(door.phase!=MAINTENANCE){ack(id,false,F("REQUIERE_MANTENIMIENTO"));return;}
    Config c=defaults();if(!strcmp(cmd,"LOAD")&&!loadConfig(c)){ack(id,false,F("EEPROM_NO_VALIDA"));return;}
    releasePins();door.cfg=c;door.maintenance();setupPins();dirty=!strcmp(cmd,"DEFAULTS");configOut();ack(id,true,F("CONFIG_RECUPERADA"));
  }
  else ack(id,false,F("COMANDO_DESCONOCIDO"));
}
void setup(){
  Serial.begin(115200);Config c=defaults();bool loaded=loadConfig(c);stopped=EEPROM.read(STOP_ADDR)==1;
  door.reset(c,stopped);dirty=!loaded;setupPins();bootAt=millis();
  Serial.println(F("{\"type\":\"hello\",\"fw\":\"GARAGE-1.0.0\"}"));configOut();
}
void loop(){
  // Bound serial work per loop; no unbounded String allocation on UNO.
  for(uint8_t n=0;n<32 && Serial.available();n++){
    char c=Serial.read();if(c=='\r')continue;
    if(c=='\n'){if(!overflow){line[used]=0;command(line);}used=0;overflow=false;}
    else if(used<sizeof(line)-1 && !overflow)line[used++]=c;else overflow=true;
  }
  uint32_t now=millis();
  bool raw=digitalRead(door.cfg.v[IR])==(int)door.cfg.v[IR_ACTIVE];
  if(raw!=irRaw){irRaw=raw;irChanged=now;}
  if((uint32_t)(now-irChanged)>=100)irStable=irRaw;
  if((uint32_t)(now-lastSense)>=80){
    lastSense=now;digitalWrite(door.cfg.v[TRIG],LOW);delayMicroseconds(2);digitalWrite(door.cfg.v[TRIG],HIGH);delayMicroseconds(10);digitalWrite(door.cfg.v[TRIG],LOW);
    // Bounded blocking measurement: <=25 ms; Servo Timer1 keeps running.
    unsigned long echo=pulseIn(door.cfg.v[ECHO],HIGH,25000UL);
    uint16_t cm=echo/58UL;door.sense(echo>0 && cm>=2 && cm<=400,cm,irStable,millis());
  }
  now=millis();
  if(!bootDone && (uint32_t)(now-bootAt)>=3000){bootDone=true;if(door.cfg.v[BOOT_AUTO] && door.cfg.v[CALIBRATED] && !stopped)door.open(now);}
  door.tick(now);drive();
  if((uint32_t)(now-lastStatus)>=250){lastStatus=now;status();}
}
