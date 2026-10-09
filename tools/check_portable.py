"""Smoke test extracted package without Arduino or an external tunnel."""
import json,os,subprocess,time,tempfile,urllib.request,zipfile,socket
from pathlib import Path
root=Path(__file__).resolve().parents[1]
version=(root/'VERSION').read_text().strip()
(root/'.build').mkdir(exist_ok=True)
with socket.socket() as s:
    s.bind(('127.0.0.1',0));port=s.getsockname()[1]
extraction=Path(tempfile.mkdtemp(prefix='portable-qa-',dir=root/'.build'))
with zipfile.ZipFile(root/f'entregas/release-{version}/GarajeInteligente_v{version}_Windows.zip') as z:z.extractall(extraction)
folder=extraction/f'GarajeInteligente_v{version}_Windows'
env=os.environ.copy();env['GARAGE_PIN']='24681357'
proc=subprocess.Popen([str(folder/'GarajeServidor.exe'),'--port',str(port),'--no-browser'],cwd=folder,env=env,creationflags=subprocess.CREATE_NO_WINDOW,stdout=subprocess.PIPE,stderr=subprocess.STDOUT)
def call(path,body=None):
    req=urllib.request.Request(f'http://127.0.0.1:{port}'+path,data=None if body is None else json.dumps(body).encode(),headers={'Content-Type':'application/json','X-Garage-Request':'1'})
    with urllib.request.urlopen(req,timeout=3) as r:return r.read()
try:
    for attempt in range(40):
        try:
            assert json.loads(call('/health'))['service']=='garage';break
        except OSError:time.sleep(.2)
    else:raise RuntimeError('Portable server did not start')
    assert b'Garaje' in call('/')
    assert b'function render' in call('/app.js')
    call('/api/demo',{'enabled':True})
    assert json.loads(call('/api/state'))['demo']
    call('/api/command',{'command':'STOP'})
    assert json.loads(call('/api/state'))['telemetry']['phase']==5
    print('PASS: clean ZIP extraction, health, HTML, JS, demo and STOP. No hardware connected.')
finally:
    proc.terminate();proc.wait(timeout=8)
