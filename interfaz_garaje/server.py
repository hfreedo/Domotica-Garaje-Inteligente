"""Garage bridge. No hardware command is successful until the UNO acknowledges it."""
import argparse
import collections
import copy
import hmac
import http.cookies
import ipaddress
import json
import mimetypes
import os
from pathlib import Path
import secrets
import threading
import time
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer
from urllib.parse import urlsplit

import serial
from serial.tools import list_ports
from protocol import FIELDS, DEFAULTS, validate_config, config_command, command_text

class Bridge:
    def __init__(self):
        self.lock=threading.RLock()
        self.operation=threading.RLock()
        self.ser=None; self.reader=None; self.port=''; self.connected=False
        self.generation=0; self.sequence=0; self.pending={}
        self.telemetry={}; self.config=None; self.last_rx=0
        self.events=collections.deque(maxlen=80)
        self.demo=False; self.demo_saved=None; self.demo_time=time.monotonic()
        self.demo_distance=100; self.demo_ir=False; self.demo_wait=0

    def event(self,text):
        with self.lock:self.events.append({'time':time.strftime('%H:%M:%S'),'text':text})

    def disconnect(self):
        with self.operation:
            with self.lock:
                self.generation+=1; s=self.ser; self.ser=None; self.connected=False; self.port=''
                self.config=None; self.telemetry={}; self.last_rx=0
                for item in self.pending.values():
                    item['reply']={'ok':False,'message':'Conexión interrumpida; orden no confirmada.'}; item['event'].set()
            if s:
                s.close()
                self.event('Puerto cerrado. El Arduino conserva su automatización si mantiene alimentación.')

    def connect(self,port):
        if port not in {p.device for p in list_ports.comports()}:
            raise ValueError('El puerto seleccionado ya no está disponible.')
        with self.operation:
            self.disconnect()
            self.demo=False
            s=serial.Serial(port=None,baudrate=115200,timeout=.15,write_timeout=1)
            s.dtr=False; s.rts=False; s.port=port
            try:s.open()
            except Exception:
                s.close(); raise
            with self.lock:
                self.ser=s; self.port=port; gen=self.generation
            self.reader=threading.Thread(target=self.read,args=(s,gen),daemon=True); self.reader.start()
            # Some UNO USB bridges still reset on open despite DTR=False.
            time.sleep(2)
            try:
                result=self.send('PING',require_ready=False)
                if result.get('message')!='GARAGE-1.0.0':raise ValueError('Firmware incompatible. Cargue GarajeInteligente.ino.')
                self.send('GET',require_ready=False)
                with self.lock:
                    if self.config is None or not self.telemetry:raise ValueError('Sin configuración/telemetría del garaje.')
                    self.connected=True
                self.event('Arduino identificado. Conectar no envía órdenes de movimiento.')
            except Exception:
                self.disconnect();raise

    def read(self,s,gen):
        buf=bytearray()
        try:
            while self.ser is s and self.generation==gen:
                part=s.read(1)
                if not part:continue
                if part!=b'\n':
                    buf.extend(part)
                    if len(buf)>2048:buf.clear()
                    continue
                try:msg=json.loads(buf)
                except (ValueError,UnicodeError):buf.clear();continue
                buf.clear()
                with self.lock:
                    if gen!=self.generation:return
                    if msg.get('type')=='ack':
                        item=self.pending.get(msg.get('id'))
                        if item:item['reply']=msg;item['event'].set()
                    elif msg.get('type')=='config':
                        values=msg.get('values',[])
                        if len(values)==len(FIELDS):self.config=validate_config(dict(zip(FIELDS,values)))
                    elif msg.get('type')=='status' and msg.get('fw')=='GARAGE-1.0.0':
                        old=self.telemetry.get('phase')
                        self.telemetry=msg;self.last_rx=time.monotonic()
                        if old!=msg.get('phase'):self.event('Estado del Arduino: '+str(msg.get('phase')))
                    elif msg.get('type')=='hello':
                        self.telemetry={}; self.config=None; self.last_rx=0
                        self.event('Reinicio del Arduino detectado; posición física no medida.')
        except (serial.SerialException,OSError,ValueError) as e:
            self.event('Lectura serial interrumpida: '+str(e))
        finally:
            with self.lock:
                if self.ser is s:self.connected=False

    def send(self,cmd,require_ready=True):
        with self.operation:
            if self.demo:return self.demo_command(cmd)
            with self.lock:
                if self.ser is None:raise ValueError('Arduino no conectado.')
                if require_ready and (not self.connected or time.monotonic()-self.last_rx>2) and cmd!='STOP':
                    raise ValueError('Telemetría ausente o antigua. No se enviará movimiento.')
                self.sequence=self.sequence%65535+1; sid=self.sequence
                item={'event':threading.Event(),'reply':None};self.pending[sid]=item
                s=self.ser
            try:
                s.write(f'{sid} {cmd}\n'.encode('ascii'))
                if not item['event'].wait(2):raise TimeoutError('Orden enviada, pero SIN confirmación del Arduino. Verifique físicamente; no se reintenta automáticamente.')
                result=item['reply']
                if not result.get('ok'):raise ValueError(result.get('message','Orden rechazada.'))
                if cmd not in ('PING','GET'):self.event('Arduino confirmó '+cmd.split()[0]+': '+result.get('message',''))
                return result
            finally:
                with self.lock:self.pending.pop(sid,None)

    def set_demo(self,enabled):
        with self.operation,self.lock:
            if enabled:
                if self.ser is not None:raise ValueError('Desconecte el Arduino antes de activar demo.')
                self.demo=True;self.config=DEFAULTS.copy();self.config['calibrated']=1
                self.demo_saved=self.config.copy();self.demo_time=time.monotonic();self.demo_wait=0
                self.telemetry={'phase':0,'position':-1,'valid':True,'cm':100,'ir':False,'ir_raw':1,
                    'vehicle':False,'clear':True,'entry':False,'remaining':0,'dirty':False,'stopped':False,'attached':[0,0],'pulse':[-1,-1]}
                self.event('DEMO: sin conexión física. Los valores y confirmaciones son simulados.')
            else:
                self.demo=False;self.telemetry={};self.config=None;self.last_rx=0

    def demo_command(self,cmd):
        with self.lock:
            c=self.config;t=self.telemetry;parts=cmd.split();action=parts[0]
            if action=='CONFIG':
                if t['phase']!=6:raise ValueError('Requiere mantenimiento.')
                c=validate_config(dict(zip(FIELDS,map(int,parts[1:]))));self.config=c;t['dirty']=True;t['attached']=[0,0];t['pulse']=[-1,-1]
            elif action=='MAINT':t.update(phase=6,position=-1,stopped=True,attached=[0,0],pulse=[-1,-1])
            elif action=='EXIT':
                if t['phase']!=6:raise ValueError('Requiere mantenimiento.')
                t.update(phase=5,position=-1,attached=[0,0],pulse=[-1,-1])
            elif action=='STOP':t.update(phase=5,stopped=True,remaining=0)
            elif action in ('OPEN','RESUME'):
                if not c['calibrated'] or t['phase']==6 or (t['stopped'] and action=='OPEN'):raise ValueError('Calibre y reanude primero.')
                t.update(phase=2,stopped=False,entry=False,position=max(0,t['position']));self.demo_wait=0
            elif action=='CLOSE':
                if t['phase']!=3 or not t['clear']:raise ValueError('Requiere abierta y exterior libre.')
                t['phase']=4
            elif action in ('SAVE','LOAD','DEFAULTS'):
                if t['phase']!=6:raise ValueError('Requiere mantenimiento.')
                if action=='SAVE':self.demo_saved=c.copy();t['dirty']=False
                elif action=='LOAD':self.config=self.demo_saved.copy();t['dirty']=False
                else:self.config=DEFAULTS.copy();t['dirty']=True
            elif action=='JOG':
                if t['phase']!=6 or int(parts[1])>c['servo_count']:raise ValueError('Prueba no habilitada.')
                t['pulse'][int(parts[1])-1]=int(parts[2]);t['attached'][int(parts[1])-1]=1
            self.event('DEMO: '+action)
            return {'ok':True,'message':'SIMULADO: '+action}

    def simulate(self):
        now=time.monotonic();dt=min(.6,now-self.demo_time);self.demo_time=now
        t=self.telemetry;c=self.config;cm=self.demo_distance
        t.update(cm=cm or 0,valid=cm is not None,ir=self.demo_ir,ir_raw=c['ir_active'] if self.demo_ir else 1-c['ir_active'])
        t['vehicle']=cm is not None and c['min_cm']<=cm<=c['max_cm']
        t['clear']=cm is not None and cm>c['max_cm']+c['hysteresis_cm']
        if t['phase']==1 and t['vehicle']:t.update(phase=2,entry=False)
        if t['phase']==4 and not t['clear']:t['phase']=2
        if t['phase'] in (2,3) and t['ir']:t['entry']=True
        if t['phase'] in (2,4):
            t['position']=max(0,min(1000,t['position']+(1 if t['phase']==2 else -1)*dt*1000000/c['travel_ms']))
            if t['phase']==2 and t['position']==1000:t['phase']=3;self.demo_wait=0
            elif t['phase']==4 and t['position']==0:t['phase']=1
        if t['phase']==3:
            self.demo_wait=self.demo_wait+dt if t['clear'] else 0
            delay=c['close_ms'] if t['entry'] else c['abandon_ms']
            t['remaining']=max(0,delay-int(self.demo_wait*1000))
            if t['remaining']==0:t['phase']=4
        if t['phase'] not in (0,6) and t['position']>=0:
            for i in (1,2):
                if i<=c['servo_count']:
                    t['attached'][i-1]=1;t['pulse'][i-1]=round(c[f's{i}_closed']+(c[f's{i}_open']-c[f's{i}_closed'])*t['position']/1000)

    def snapshot(self):
        with self.lock:
            if self.demo:self.simulate()
            age=time.monotonic()-self.last_rx if self.last_rx else None
            return copy.deepcopy({'demo':self.demo,'connected':self.connected,'port':self.port,
                'fresh':self.demo or bool(self.connected and age is not None and age<2),'age':age,
                'telemetry':self.telemetry,'config':self.config,'events':list(self.events)})

class Access:
    def __init__(self,pin):
        self.pin=pin;self.lock=threading.Lock();self.sessions={};self.attempts={}
    def login(self,pin,ip):
        now=time.monotonic()
        with self.lock:
            self.attempts={k:v for k,v in self.attempts.items() if now-v[0]<60}
            start,n=self.attempts.get(ip,(now,0))
            if n>=5:return None,429
            if len(self.attempts)>=256 and ip not in self.attempts:return None,429
            self.attempts[ip]=(start,n+1)
            if not isinstance(pin,str) or not hmac.compare_digest(pin,self.pin):return None,401
            self.sessions={k:v for k,v in self.sessions.items() if v>now}
            if len(self.sessions)>=128:return None,429
            token=secrets.token_urlsafe(32);self.sessions[token]=now+8*3600
            return token,200
    def valid(self,cookie):
        jar=http.cookies.SimpleCookie()
        try:jar.load(cookie or '');token=jar['garage_session'].value
        except (KeyError,http.cookies.CookieError):return False
        with self.lock:return self.sessions.get(token,0)>time.monotonic()

STATIC=Path(__file__).resolve().parent/'static'
LOGIN='''<!doctype html><html lang="es"><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1"><title>Acceso al garaje</title><link rel="stylesheet" href="/style.css"><main class="login card"><p class="eyebrow">GARAJE INTELIGENTE</p><h1>Acceso privado</h1><p>Escribe el PIN que aparece en el panel de la computadora.</p><form id="login"><label>PIN de sesión<input id="pin" type="password" inputmode="numeric" autocomplete="one-time-code" required maxlength="32"></label><button>Entrar</button></form><p id="error" role="alert"></p></main><script src="/login.js"></script></html>'''

class Handler(BaseHTTPRequestHandler):
    def log_message(self,*args):pass
    @property
    def bridge(self):return self.server.bridge
    def local(self):
        # ngrok forwards through loopback. Never trust loopback alone, or Host supplied by LAN.
        host=(self.headers.get('Host') or '').lower()
        return (ipaddress.ip_address(self.client_address[0]).is_loopback and
            host in (f'127.0.0.1:{self.server.server_port}',f'localhost:{self.server.server_port}') and
            not any(self.headers.get(h) for h in ('Forwarded','X-Forwarded-For','X-Forwarded-Host','X-Forwarded-Proto')))
    def authorized(self):return self.local() or self.server.access.valid(self.headers.get('Cookie'))
    def respond(self,data,status=200,mime='application/json; charset=utf-8',headers=None):
        if mime.startswith('application/json'):data=json.dumps(data,ensure_ascii=False)
        if isinstance(data,str):data=data.encode('utf-8')
        self.send_response(status);self.send_header('Content-Type',mime);self.send_header('Content-Length',str(len(data)))
        self.send_header('Cache-Control','no-store');self.send_header('X-Content-Type-Options','nosniff')
        self.send_header('X-Frame-Options','DENY');self.send_header('Referrer-Policy','no-referrer')
        self.send_header('Content-Security-Policy',"default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; frame-ancestors 'none'; base-uri 'none'")
        for k,v in (headers or {}).items():self.send_header(k,v)
        self.end_headers()
        try:self.wfile.write(data)
        except (BrokenPipeError,ConnectionResetError):pass
    def do_GET(self):
        path=urlsplit(self.path).path
        if path=='/health':return self.respond({'service':'garage','version':'1.0.0'})
        if path in ('/style.css','/login.js'):return self.static(path)
        if not self.authorized():
            if path.startswith('/api/'):return self.respond({'error':'Ingrese el PIN.'},401)
            return self.respond(LOGIN,mime='text/html; charset=utf-8')
        if path=='/api/state':return self.respond(self.bridge.snapshot())
        if path=='/api/ports':return self.respond([{'device':p.device,'description':p.description} for p in list_ports.comports()])
        if path in ('/','/index.html','/app.js'):return self.static('/index.html' if path=='/' else path)
        self.respond({'error':'No encontrado.'},404)
    def static(self,path):
        p=STATIC/path.lstrip('/')
        self.respond(p.read_bytes(),mime=(mimetypes.guess_type(str(p))[0] or 'text/plain')+'; charset=utf-8')
    def do_POST(self):
        try:
            # A custom header prevents drive-by HTML forms. No CORS permission is emitted.
            if self.headers.get('X-Garage-Request')!='1':return self.respond({'error':'Solicitud no válida.'},403)
            origin=self.headers.get('Origin')
            if origin and urlsplit(origin).netloc.lower()!=(self.headers.get('Host') or '').lower():
                return self.respond({'error':'Origen no permitido.'},403)
            n=int(self.headers.get('Content-Length','0'))
            if n<2 or n>4096:raise ValueError('Tamaño de solicitud no válido.')
            if not self.headers.get('Content-Type','').startswith('application/json'):raise ValueError('Se requiere JSON.')
            body=json.loads(self.rfile.read(n))
            if not isinstance(body,dict):raise ValueError('Se requiere un objeto JSON.')
            path=urlsplit(self.path).path
            if path=='/api/login':
                token,status=self.server.access.login(body.get('pin'),self.client_address[0])
                if not token:return self.respond({'error':'PIN incorrecto o demasiados intentos. Espere un minuto.'},status)
                secure='; Secure' if self.headers.get('X-Forwarded-Proto')=='https' else ''
                return self.respond({'ok':True},headers={'Set-Cookie':f'garage_session={token}; Path=/; HttpOnly; SameSite=Strict; Max-Age=28800{secure}'})
            if not self.authorized():return self.respond({'error':'Ingrese el PIN.'},401)
            if path=='/api/connect':self.bridge.connect(str(body.get('port','')))
            elif path=='/api/disconnect':self.bridge.disconnect()
            elif path=='/api/demo':
                if type(body.get('enabled')) is not bool:raise ValueError('Valor demo no válido.')
                self.bridge.set_demo(body['enabled'])
            elif path=='/api/demo/sensors':
                with self.bridge.lock:
                    if not self.bridge.demo:raise ValueError('Solo disponible en demo.')
                    cm=body.get('cm');inside=body.get('ir')
                    if cm is not None and (type(cm) is not int or not 2<=cm<=400):raise ValueError('Distancia no válida.')
                    if type(inside) is not bool:raise ValueError('Presencia no válida.')
                    self.bridge.demo_distance=cm;self.bridge.demo_ir=inside
            elif path=='/api/command':return self.respond(self.bridge.send(command_text(body)))
            elif path=='/api/config':return self.respond(self.bridge.send(config_command(body)))
            else:return self.respond({'error':'No encontrado.'},404)
            self.respond({'ok':True})
        except TimeoutError as e:self.respond({'error':str(e)},504)
        except (ValueError,serial.SerialException,OSError) as e:self.respond({'error':str(e)},400)

def make_server(host='127.0.0.1',port=8773,pin=None):
    srv=ThreadingHTTPServer((host,port),Handler)
    srv.bridge=Bridge();srv.access=Access(pin or secrets.token_hex(4));return srv

def main():
    p=argparse.ArgumentParser();p.add_argument('--host',default='127.0.0.1');p.add_argument('--port',type=int,default=8773)
    p.add_argument('--no-browser',action='store_true');args=p.parse_args()
    pin=os.environ.get('GARAGE_PIN') or str(secrets.randbelow(90000000)+10000000)
    srv=make_server(args.host,args.port,pin)
    print(f'Garaje listo en http://127.0.0.1:{srv.server_port}',flush=True)
    if not os.environ.get('GARAGE_PIN'):print('PIN remoto: '+pin,flush=True)
    if not args.no_browser:
        import webbrowser
        webbrowser.open(f'http://127.0.0.1:{srv.server_port}')
    try:srv.serve_forever()
    except KeyboardInterrupt:pass
    finally:srv.bridge.disconnect();srv.server_close()

if __name__=='__main__':main()
