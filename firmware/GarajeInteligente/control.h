#pragma once
#include <stdint.h>

// Shared with the native tests; no Arduino dependencies.
enum Field { COUNT, TRIG, ECHO, IR, SERVO1, SERVO2, MIN_CM, MAX_CM,
  HYST_CM, DETECT_MS, CLEAR_MS, CLOSE_MS, ABANDON_MS, TRAVEL_MS,
  S1_CLOSED, S1_OPEN, S2_CLOSED, S2_OPEN, IR_ACTIVE, BOOT_AUTO, CALIBRATED, NFIELDS };
struct Config { uint16_t v[NFIELDS]; };
inline Config defaults() {
  Config c = {{1,4,2,7,9,10,3,25,5,300,1500,5000,10000,2500,
    1100,1900,1900,1100,0,0,0}};
  return c;
}
inline bool validConfig(const Config &c) {
  const uint16_t *v=c.v;
  if(v[COUNT]<1 || v[COUNT]>2) return false;
  // D2..D12, A0..A5 (14..19); avoid USB UART and onboard LED D13.
  for(uint8_t i=TRIG;i<=SERVO2;i++) {
    if(v[i]<2 || v[i]>19 || v[i]==13) return false;
    for(uint8_t j=TRIG;j<i;j++) if(v[i]==v[j]) return false;
  }
  if(v[MIN_CM]<2 || v[MAX_CM]<=v[MIN_CM] || v[MAX_CM]>200) return false;
  if(v[HYST_CM]<1 || v[HYST_CM]>30) return false;
  if(v[DETECT_MS]<100 || v[DETECT_MS]>5000) return false;
  if(v[CLEAR_MS]<500 || v[CLEAR_MS]>10000) return false;
  if(v[CLOSE_MS]<1000 || v[CLOSE_MS]>60000 || v[ABANDON_MS]<1000 || v[ABANDON_MS]>60000) return false;
  if(v[TRAVEL_MS]<1000 || v[TRAVEL_MS]>15000) return false;
  for(uint8_t i=S1_CLOSED;i<=S2_OPEN;i++) if(v[i]<700 || v[i]>2300) return false;
  int d1=(int)v[S1_OPEN]-v[S1_CLOSED], d2=(int)v[S2_OPEN]-v[S2_CLOSED];
  if((d1<0?-d1:d1)<100 || (d2<0?-d2:d2)<100) return false;
  return v[IR_ACTIVE]<=1 && v[BOOT_AUTO]<=1 && v[CALIBRATED]<=1;
}
enum Phase { READY, CLOSED, OPENING, WAITING, CLOSING, PAUSED, MAINTENANCE };
class DoorControl {
public:
  Config cfg;
  Phase phase=READY;
  int16_t position=-1; // 0..1000 commanded progress; -1 unknown
  bool entry=false, vehicle=false, clear=false, echoValid=false, ir=false;
  uint16_t distance=0;
  uint32_t remaining=0;
  DoorControl():cfg(defaults()){}
  void reset(const Config &c, bool stopped) {
    *this=DoorControl(); cfg=c; phase=stopped?PAUSED:READY;
  }
  void sense(bool valid,uint16_t cm,bool inside,uint32_t now) {
    echoValid=valid; distance=cm; ir=inside;
    bool candidate=valid && cm>=cfg.v[MIN_CM] && cm<=cfg.v[MAX_CM];
    if(candidate) {
      if(!detectRunning){detectRunning=true;detectSince=now;}
      if((uint32_t)(now-detectSince)>=cfg.v[DETECT_MS]) vehicle=true;
    } else {
      detectRunning=false;
      if(!valid || cm<cfg.v[MIN_CM] || cm>cfg.v[MAX_CM]+cfg.v[HYST_CM]) vehicle=false;
    }
    bool free=valid && cm>cfg.v[MAX_CM]+cfg.v[HYST_CM];
    if(free) {
      if(!clearRunning){clearRunning=true;clearSince=now;}
      clear=(uint32_t)(now-clearSince)>=cfg.v[CLEAR_MS];
    } else {clearRunning=false;clear=false;}
    if((phase==OPENING || phase==WAITING) && inside) entry=true;
    // Interior sensor observes parked vehicle: it is NOT a threshold safety beam.
    if(phase==CLOSING && !free) open(now,false);
  }
  bool open(uint32_t now,bool newCycle=true) {
    if(!cfg.v[CALIBRATED] || phase==MAINTENANCE || phase==PAUSED) return false;
    if(newCycle) entry=false;
    if(position<0) { position=1000; unknownStart=true; }
    else unknownStart=false;
    begin(OPENING,now); return true;
  }
  bool close(uint32_t now) {
    if(phase!=WAITING || !clear || !echoValid) return false;
    begin(CLOSING,now); return true;
  }
  void pause() {phase=PAUSED;remaining=0;}
  void maintenance() {phase=MAINTENANCE;position=-1;remaining=0;}
  void leaveMaintenance(){phase=READY;position=-1;}
  bool resume(uint32_t now) {
    if(phase!=PAUSED || !cfg.v[CALIBRATED])return false;
    phase=READY;return open(now);
  }
  void tick(uint32_t now) {
    if(phase==CLOSED && vehicle) open(now);
    if(phase==OPENING || phase==CLOSING) {
      uint32_t elapsed=now-motionStart;
      uint32_t step=elapsed*1000UL/cfg.v[TRAVEL_MS];
      if(phase==OPENING){
        if(!unknownStart) position=(step>=(uint32_t)(1000-startPosition))?1000:startPosition+step;
        if(position>=1000 && (!unknownStart || elapsed>=cfg.v[TRAVEL_MS])) {
          phase=WAITING;waitSince=now;remaining=0;
        }
      } else {
        position=(step>=(uint32_t)startPosition)?0:startPosition-step;
        if(position==0)phase=CLOSED;
      }
    }
    if(phase==WAITING) {
      uint32_t delay=entry?cfg.v[CLOSE_MS]:cfg.v[ABANDON_MS];
      // Full waiting interval starts again on blocked/unknown exterior.
      if(!clear)waitSince=now;
      uint32_t passed=now-waitSince;
      remaining=passed>=delay?0:delay-passed;
      if(clear && passed>=delay)close(now);
    }
  }
private:
  bool detectRunning=false,clearRunning=false,unknownStart=false;
  uint32_t detectSince=0,clearSince=0,motionStart=0,waitSince=0;
  int16_t startPosition=0;
  void begin(Phase p,uint32_t now){phase=p;motionStart=now;startPosition=position;remaining=0;}
};
