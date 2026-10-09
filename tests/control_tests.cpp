#include "../firmware/GarajeInteligente/control.h"
// Compile to WebAssembly and run under Node; tests execute the actual control.h.
#define CHECK(x,n) if(!(x))return n
extern "C" int run_tests(){
  Config c=defaults();CHECK(validConfig(c),1);
  c.v[SERVO2]=c.v[SERVO1];CHECK(!validConfig(c),2);c=defaults();
  c.v[TRIG]=0;CHECK(!validConfig(c),3);c=defaults();
  c.v[MAX_CM]=c.v[MIN_CM];CHECK(!validConfig(c),4);c=defaults();
  c.v[CALIBRATED]=1;DoorControl d;d.reset(c,false);
  CHECK(d.position==-1&&d.phase==READY,5);
  CHECK(d.open(0),6);d.tick(2499);CHECK(d.phase==OPENING,7);
  d.tick(2500);CHECK(d.phase==WAITING&&d.position==1000,8);
  // No echo can never be taken as proof of a free exterior.
  for(uint32_t t=2500;t<30000;t+=100){d.sense(false,0,false,t);d.tick(t);}
  CHECK(d.phase==WAITING&&!d.clear,9);
  // Abandoned approach closes only after stable valid-clear readings + timeout.
  d.sense(true,100,false,30000);d.tick(30000);
  d.sense(true,100,false,31500);d.tick(31500);
  d.tick(39999);CHECK(d.phase==WAITING,10);
  d.tick(41000);CHECK(d.phase==CLOSING,11);
  d.tick(42000);CHECK(d.position>0&&d.position<1000,12);
  // A very near object below the opening range still aborts closing.
  d.sense(true,2,false,42001);CHECK(d.phase==OPENING,13);
  d.tick(45000);CHECK(d.phase==WAITING,14);
  // Interior held active (parked car) confirms entry but doesn't block closing.
  d.sense(true,100,true,45000);d.tick(45000);
  d.sense(true,100,true,46500);d.tick(46500);
  d.tick(52000);CHECK(d.phase==CLOSING&&d.entry,15);
  // Loss of echo while closing also reopens.
  d.sense(false,0,true,52100);CHECK(d.phase==OPENING,16);
  d.tick(55000);CHECK(d.phase==WAITING,17);
  // STOP survives future ticks and refuses direct OPEN/CLOSE.
  d.pause();int p=d.position;
  for(uint32_t t=55000;t<85000;t+=100){d.sense(true,10,true,t);d.tick(t);}
  CHECK(d.phase==PAUSED&&d.position==p,18);
  CHECK(!d.open(85001)&&!d.close(85002),19);
  CHECK(d.resume(85003)&&d.phase==OPENING,20);
  // Maintenance inhibits every automatic transition.
  d.maintenance();d.sense(true,10,true,86000);d.tick(96000);
  CHECK(d.phase==MAINTENANCE&&d.position==-1&&!d.open(96001),21);
  // No motion before calibration.
  d.reset(defaults(),false);CHECK(!d.open(0),22);
  // Debounce and millis wraparound.
  d.reset(c,false);d.phase=CLOSED;d.position=0;
  d.sense(true,10,false,0xffffff00U);d.tick(0xffffff00U);CHECK(d.phase==CLOSED,23);
  d.sense(true,10,false,0x50U);d.tick(0x50U);CHECK(d.phase==OPENING,24);
  // A short detection pulse doesn't open.
  d.reset(c,false);d.phase=CLOSED;d.position=0;
  d.sense(true,10,false,0);d.sense(true,100,false,100);d.tick(1000);CHECK(d.phase==CLOSED,25);
  // Reset restores persistent pause supplied by the EEPROM adapter.
  d.reset(c,true);d.sense(true,10,false,0);d.sense(true,10,false,1000);d.tick(1000);CHECK(d.phase==PAUSED,26);
  return 0;
}
