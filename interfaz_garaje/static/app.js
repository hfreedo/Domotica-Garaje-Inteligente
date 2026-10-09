'use strict';
// This nominal scale matches the 700..2300 us range used by Servo.attach.
const ServoUnits={
 toDegrees:p=>Math.round((p-700)*180/1600*100)/100,
 toPulse:(v,unit)=>{const n=Number(v);if(String(v).trim()===''||!Number.isFinite(n)||n<(unit==='deg'?0:700)||n>(unit==='deg'?180:2300)|| (unit==='us'&&!Number.isInteger(n)))throw Error('Valor de servo fuera de rango. Usa 0–180° o 700–2300 µs.');return unit==='deg'?Math.round(700+n*1600/180):n;}
};
const $=id=>document.getElementById(id);
const phases=['Lista para inicializar','Cerrada (estimada)','Abriendo','Abierta (estimada)','Cerrando','Pausa enclavada','Mantenimiento'];
let snapshot=null,initialized=false,lastDemo=null,busy=false,formDirty=false;
let servoUnit='us';
const specs=[
 ['min_cm','Distancia mínima (cm)',2,199,'detection'],['max_cm','Distancia máxima (cm)',3,200,'detection'],['hysteresis_cm','Margen de liberación (cm)',1,30,'detection'],
 ['detect_ms','Confirmar vehículo (ms)',100,5000,'detection'],['clear_ms','Confirmar exterior libre (ms)',500,10000,'detection'],['close_ms','Espera tras entrada (ms)',1000,60000,'detection'],
 ['abandon_ms','Espera sin entrada (ms)',1000,60000,'detection'],['travel_ms','Recorrido completo (ms)',1000,15000,'detection'],
 ['servo_count','Número de servos',1,2,'hardware'],['trig','Ultrasónico · TRIG',2,19,'hardware'],['echo','Ultrasónico · ECHO',2,19,'hardware'],
 ['ir_pin','Infrarrojo · OUT',2,19,'hardware'],['servo1','Señal del servo 1',2,19,'hardware'],['servo2','Señal del servo 2',2,19,'hardware'],['ir_active','Nivel de detección IR',0,1,'hardware'],
 ['s1_closed','Servo 1 · cerrado (µs)',700,2300,'servo'],['s1_open','Servo 1 · abierto (µs)',700,2300,'servo'],
 ['s2_closed','Servo 2 · cerrado (µs)',700,2300,'servo'],['s2_open','Servo 2 · abierto (µs)',700,2300,'servo']];
for(const [name,label,min,max,group]of specs){
 const l=document.createElement('label');l.textContent=label;let input;
 if(name==='servo_count'||name==='ir_active'){
   input=document.createElement('select');for(let i=min;i<=max;i++){const o=document.createElement('option');o.value=i;o.textContent=name==='ir_active'?(i?'HIGH (1)':'LOW (0)'):String(i);input.append(o);}
 }else{input=document.createElement('input');input.type='number';input.min=min;input.max=max;input.step=1;input.required=true;}
 input.name=name;l.append(input);$(group+'-fields').append(l);
 if(group==='servo'){const hint=document.createElement('small');hint.className='muted';l.append(hint);input.dataset.servoLabel=label.replace(' (µs)','');input._hint=hint;}
}
const servoInputs=[...document.querySelectorAll('#servo-fields input'),$('jog-pulse')];
$('jog-pulse').dataset.servoLabel='Posición de prueba';
function showServo(input,pulse){
 input.dataset.pulse=String(pulse);input.value=servoUnit==='deg'?ServoUnits.toDegrees(pulse):pulse;
 input.min=servoUnit==='deg'?0:700;input.max=servoUnit==='deg'?180:2300;input.step=servoUnit==='deg'?.01:1;
 input.parentElement.firstChild.textContent=`${input.dataset.servoLabel} (${servoUnit==='deg'?'°':'µs'})`;
 if(input._hint)input._hint.textContent=servoUnit==='deg'?`${pulse} µs equivalentes`:`${ServoUnits.toDegrees(pulse)}° nominales`;
}
function servoValue(input){return ServoUnits.toPulse(input.dataset.pulse??input.value,'us');}
for(const input of servoInputs)input.addEventListener('input',()=>{try{const pulse=ServoUnits.toPulse(input.value,servoUnit);input.dataset.pulse=pulse;input.setCustomValidity('');if(input._hint)input._hint.textContent=servoUnit==='deg'?`${pulse} µs equivalentes`:`${ServoUnits.toDegrees(pulse)}° nominales`;}catch(e){delete input.dataset.pulse;input.setCustomValidity(e.message);}});
showServo($('jog-pulse'),1500);
$('servo-unit').onchange=()=>{
 if(servoInputs.some(input=>!input.checkValidity())){$('servo-unit').value=servoUnit;notice('Corrige los valores de servo antes de cambiar de unidad.',true);return;}
 const pulses=servoInputs.map(servoValue);servoUnit=$('servo-unit').value;servoInputs.forEach((input,i)=>showServo(input,pulses[i]));
};
$('config-form').addEventListener('input',()=>formDirty=true);
function notice(text,error=false){$('notice').textContent=text;$('notice').classList.toggle('error',error);}
async function api(path,body){
 const options=body===undefined?{}:{method:'POST',headers:{'Content-Type':'application/json','X-Garage-Request':'1'},body:JSON.stringify(body)};
 const r=await fetch(path,options);if(r.status===401){location.reload();throw Error('Sesión caducada.');}
 const d=await r.json();if(!r.ok)throw Error(d.error||'Solicitud rechazada.');return d;
}
async function action(fn){if(busy)return;busy=true;renderButtons();try{const r=await fn();notice(r?.message||'Operación completada.');await poll();}catch(e){notice(e.message,true);}finally{busy=false;renderButtons();}}
function fillConfig(c){if(!c)return;for(const[k,v]of Object.entries(c)){const f=$('config-form').elements.namedItem(k);if(!f)continue;if(f.type==='checkbox')f.checked=!!v;else if(f.dataset.servoLabel){f.setCustomValidity('');showServo(f,v);}else f.value=v;}initialized=true;formDirty=false;}
function renderButtons(){
 const s=snapshot,t=s?.telemetry||{},fresh=!!s?.fresh,maint=t.phase===6;
 for(const b of document.querySelectorAll('[data-cmd]')){
  const cmd=b.dataset.cmd;let allowed=fresh;
  if(cmd==='OPEN')allowed=fresh&&!t.stopped&&!maint&&!!s.config?.calibrated;
  if(cmd==='CLOSE')allowed=fresh&&t.phase===3&&t.clear;
  if(cmd==='RESUME')allowed=fresh&&t.phase===5&&!!s.config?.calibrated;
  if(['SAVE','EXIT'].includes(cmd))allowed=fresh&&maint;
  if(cmd==='STOP')allowed=!!(s?.connected||s?.demo); // permit STOP even with stale telemetry
  b.disabled=busy||!allowed;
 }
 for(const el of $('config-form').elements)el.disabled=busy||!fresh||!maint;
 $('maint').disabled=busy||!fresh||maint;$('read-config').disabled=busy||!fresh;
 $('connect').disabled=busy||!!s?.connected||!!s?.demo||!$('port').value;
 $('disconnect').disabled=busy||!s?.connected;$('demo').disabled=busy||!!s?.connected;
 $('jog-servo').options[1].disabled=s?.config?.servo_count!==2;
}
function render(s){
 snapshot=s;const t=s.telemetry||{},c=s.config;
 if(lastDemo!==s.demo){initialized=false;lastDemo=s.demo;}
 if(!initialized&&c)fillConfig(c);
 $('connection').textContent=s.demo?'Demostración':s.connected?(s.fresh?'Arduino conectado':'Sin telemetría reciente'):'Sin Arduino';
 $('demo-banner').hidden=!s.demo;$('demo-controls').hidden=!s.demo;$('demo').textContent=s.demo?'Salir de demo':'Probar sin Arduino';
 $('phase').textContent=!s.fresh?'Sin datos actuales':phases[t.phase]||'Estado desconocido';
 $('mode').textContent=c?`${c.servo_count} servo${c.servo_count===1?'':'s'}`:'—';
 const p=s.fresh&&t.position>=0?Math.round(t.position/10):null;
 $('progress').textContent=p===null?'desconocida':`${p} %`;$('progress-bar').style.width=`${p??0}%`;
 $('door-leaf').setAttribute('height',String(p===null?161:Math.max(8,161*(1-p/100))));
 $('countdown').textContent=s.fresh&&t.phase===3?(t.clear?`Cierre en ${(t.remaining/1000).toFixed(1)} s`:'Cierre inhibido'):'—';
 $('cm').textContent=s.fresh&&t.valid?t.cm:'—';
 $('exterior').textContent=!s.fresh?'Esperando medición':!t.valid?'Sin eco válido · cierre inhibido':t.vehicle?'Vehículo confirmado':t.clear?'Exterior libre confirmado':'Confirmando / zona próxima';
 $('range').textContent=c?`${c.min_cm}–${c.max_cm} cm`:'—';
 $('ir').textContent=!s.fresh?'—':t.ir?'Presencia detectada':'Sin presencia';
 $('entry').textContent=t.entry?'Detección interior registrada en este ciclo':'Sin detección interior en este ciclo';
 $('saved').textContent=c?(t.dirty?'Cambios en RAM sin guardar':'Configuración guardada'):'Esperando Arduino';
 $('pulse').textContent=t.pulse?`Posiciones ordenadas: ${t.pulse.map((x,i)=>t.attached[i]?`${x} µs (${ServoUnits.toDegrees(x)}° nominales)`:'sin señal').join(' / ')}`:'Posiciones de los servos: —';
 $('maintenance-status').textContent=t.phase===6?'Mantenimiento activo':'Mantenimiento inactivo';
 const log=$('events');log.replaceChildren();for(const e of(s.events||[]).slice().reverse()){const li=document.createElement('li'),time=document.createElement('time');time.textContent=e.time;li.append(time,document.createTextNode(e.text));log.append(li);}
 renderButtons();
}
async function poll(){render(await api('/api/state'));}
async function refresh(){const ports=await api('/api/ports');const selected=$('port').value;$('port').replaceChildren(new Option('Seleccionar puerto',''));for(const p of ports)$('port').append(new Option(`${p.device} · ${p.description}`,p.device));if(ports.some(p=>p.device===selected))$('port').value=selected;else if(ports.length===1)$('port').value=ports[0].device;renderButtons();}
$('port').onchange=renderButtons;$('refresh').onclick=()=>action(refresh);
$('connect').onclick=()=>action(async()=>{notice('Identificando Arduino…');initialized=false;return api('/api/connect',{port:$('port').value});});
$('disconnect').onclick=()=>action(async()=>{initialized=false;return api('/api/disconnect',{});});
$('demo').onclick=()=>action(()=>api('/api/demo',{enabled:!snapshot?.demo}));
for(const b of document.querySelectorAll('[data-cmd]'))b.onclick=()=>action(()=>api('/api/command',{command:b.dataset.cmd}));
$('maint').onclick=()=>{if(confirm('Sostén la puerta o desacopla los servos. Se retirarán sus señales y podría caer. ¿Entrar en mantenimiento?'))action(()=>api('/api/command',{command:'MAINT'}));};
$('read-config').onclick=()=>{if(formDirty&&!confirm('¿Reemplazar los cambios del formulario por la configuración del Arduino?'))return;action(async()=>{await api('/api/command',{command:'GET'});const s=await api('/api/state');fillConfig(s.config);});};
$('config-form').onsubmit=e=>{e.preventDefault();const data={};for(const spec of specs){const input=$('config-form').elements.namedItem(spec[0]);data[spec[0]]=spec[4]==='servo'?servoValue(input):Number(input.value);}for(const k of['calibrated','boot_auto'])data[k]=$('config-form').elements.namedItem(k).checked?1:0;action(async()=>{const result=await api('/api/config',data);formDirty=false;return result;});};
for(const[id,cmd]of[['load','LOAD'],['defaults','DEFAULTS']])$(id).onclick=()=>{if(confirm('¿Reemplazar la configuración en RAM? No se guardará en EEPROM hasta pulsar Guardar.'))action(async()=>{const r=await api('/api/command',{command:cmd});await api('/api/command',{command:'GET'});fillConfig((await api('/api/state')).config);return r;});};
$('jog').onclick=()=>{if(!$('jog-pulse').reportValidity())return;if(confirm('Prueba individual: el servo debe estar desacoplado del eje común y sin carga. ¿Enviar la posición?'))action(()=>api('/api/command',{command:'JOG',servo:Number($('jog-servo').value),pulse:servoValue($('jog-pulse'))}));};
$('demo-apply').onclick=()=>action(()=>api('/api/demo/sensors',{cm:$('demo-invalid').checked?null:Number($('demo-cm').value),ir:$('demo-ir').checked}));
for(const b of document.querySelectorAll('[data-tab]'))b.onclick=()=>{for(const page of document.querySelectorAll('.tabpage'))page.hidden=page.id!==b.dataset.tab;for(const tab of document.querySelectorAll('[data-tab]'))tab.classList.toggle('active',tab===b);};
(async()=>{try{await refresh();await poll();}catch(e){notice(e.message,true);}for(;;){await new Promise(r=>setTimeout(r,400));try{await poll();}catch(e){if(snapshot){snapshot.fresh=false;snapshot.connected=false;render(snapshot);}notice('Servidor no disponible. La pantalla no confirma el estado físico.',true);}}})();
