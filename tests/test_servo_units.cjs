const fs=require('node:fs'),vm=require('node:vm'),assert=require('node:assert/strict');
const source=fs.readFileSync('interfaz_garaje/static/app.js','utf8').split('const $=')[0];
const units=vm.runInNewContext(source+';ServoUnits');
for(const [angle,pulse] of [[0,700],[45,1100],[90,1500],[135,1900],[180,2300]]){
 assert.equal(units.toPulse(angle,'deg'),pulse);assert.equal(units.toDegrees(pulse),angle);
}
for(let pulse=700;pulse<=2300;pulse++)assert.equal(units.toPulse(units.toDegrees(pulse),'deg'),pulse);
for(const value of ['',NaN,-1,181,Infinity])assert.throws(()=>units.toPulse(value,'deg'));
for(const value of ['',699,2301,1500.5,NaN])assert.throws(()=>units.toPulse(value,'us'));
console.log('PASS: endpoints, all 1601 pulse round trips, invalid values.');
