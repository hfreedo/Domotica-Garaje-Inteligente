import http.client
import json
from pathlib import Path
import sys
import threading
import time
import unittest
sys.path.insert(0,str(Path(__file__).resolve().parents[1]/'interfaz_garaje'))
from server import Bridge,make_server
from protocol import DEFAULTS,FIELDS,validate_config,config_command,command_text

class ConfigTests(unittest.TestCase):
    def test_default_and_frame_size(self):
        self.assertEqual(validate_config(DEFAULTS),DEFAULTS)
        self.assertLess(len('65535 '+config_command(DEFAULTS)),189)
    def test_reject_duplicate_reserved_pins_and_ranges(self):
        for changes in ({'servo2':9},{'echo':0},{'trig':13},{'min_cm':30},{'travel_ms':0},{'s1_open':1100},{'servo_count':3},{'boot_auto':2}):
            with self.subTest(changes=changes),self.assertRaises(ValueError):validate_config(DEFAULTS|changes)
    def test_reject_noninteger_and_extra_fields(self):
        for changes in ({'max_cm':True},{'max_cm':'25'},{'unknown':2}):
            with self.assertRaises(ValueError):validate_config(DEFAULTS|changes)
    def test_no_serial_injection(self):
        for body in ({'command':'OPEN\n0 CLOSE'},{'command':'JOG','servo':2,'pulse':'1500\nCLOSE'}):
            with self.assertRaises(ValueError):command_text(body)

class FakeSerial:
    def __init__(self,bridge,reply=True,ok=True):self.b=bridge;self.reply=reply;self.ok=ok;self.writes=[]
    def write(self,data):
        self.writes.append(data)
        if self.reply:
            sid=int(data.split()[0]);item=self.b.pending[sid];item['reply']={'ok':self.ok,'message':'OK' if self.ok else 'REJECTED'};item['event'].set()

class TransportTests(unittest.TestCase):
    def setUp(self):self.b=Bridge();self.b.connected=True;self.b.last_rx=time.monotonic()
    def test_no_hardware_is_not_success(self):
        with self.assertRaises(ValueError):Bridge().send('OPEN')
    def test_ack_required(self):
        self.b.ser=FakeSerial(self.b);self.assertTrue(self.b.send('OPEN')['ok']);self.assertEqual(self.b.pending,{})
    def test_rejected_ack(self):
        self.b.ser=FakeSerial(self.b,ok=False)
        with self.assertRaisesRegex(ValueError,'REJECTED'):self.b.send('OPEN')
    def test_timeout_not_success_or_retry(self):
        self.b.ser=FakeSerial(self.b,reply=False)
        with self.assertRaises(TimeoutError):self.b.send('OPEN')
        self.assertEqual(len(self.b.ser.writes),1);self.assertEqual(self.b.pending,{})
    def test_stale_blocks_open_but_not_stop(self):
        self.b.ser=FakeSerial(self.b);self.b.last_rx=0
        with self.assertRaises(ValueError):self.b.send('OPEN')
        self.assertTrue(self.b.send('STOP')['ok'])
    def test_demo_cannot_take_over_real_port(self):
        self.b.ser=FakeSerial(self.b)
        with self.assertRaises(ValueError):self.b.set_demo(True)

class HTTPTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.s=make_server(port=0,pin='12345678');cls.thread=threading.Thread(target=cls.s.serve_forever,daemon=True);cls.thread.start()
    @classmethod
    def tearDownClass(cls):cls.s.shutdown();cls.s.server_close();cls.thread.join()
    def request(self,path,body=None,remote=False,cookie=None,extra=None):
        headers={}
        if remote:headers.update({'Host':'garage.example','X-Forwarded-For':'198.51.100.2'})
        if cookie:headers['Cookie']=cookie
        if body is not None:headers.update({'Content-Type':'application/json','X-Garage-Request':'1'})
        headers.update(extra or {})
        con=http.client.HTTPConnection('127.0.0.1',self.s.server_port,timeout=4)
        con.request('POST' if body is not None else 'GET',path,None if body is None else json.dumps(body),headers)
        r=con.getresponse();data=r.read();result=(r.status,data,r.getheader('Set-Cookie'));con.close();return result
    def test_forwarded_loopback_requires_pin(self):
        self.assertEqual(self.request('/api/state',remote=True)[0],401)
        self.assertEqual(self.request('/api/command',{'command':'STOP'},remote=True)[0],401)
    def test_host_spoof_does_not_bypass_forwarded(self):
        self.assertEqual(self.request('/api/state',remote=True,extra={'Host':f'127.0.0.1:{self.s.server_port}'})[0],401)
    def test_local_access(self):self.assertEqual(self.request('/api/state')[0],200)
    def test_pin_and_session(self):
        status,_,cookie=self.request('/api/login',{'pin':'12345678'},remote=True)
        self.assertEqual(status,200);self.assertIn('HttpOnly',cookie)
        self.assertEqual(self.request('/api/state',remote=True,cookie=cookie.split(';')[0])[0],200)
    def test_cross_origin_and_missing_header_denied(self):
        self.assertEqual(self.request('/api/demo',{'enabled':True},extra={'Origin':'https://evil.example'})[0],403)
        self.assertEqual(self.request('/api/demo',{'enabled':True},extra={'X-Garage-Request':''})[0],403)
    def test_disconnected_command_rejected(self):
        self.s.bridge.set_demo(False)
        self.assertEqual(self.request('/api/command',{'command':'OPEN'})[0],400)
    def test_demo_pause_and_config(self):
        self.assertEqual(self.request('/api/demo',{'enabled':True})[0],200)
        self.assertEqual(self.request('/api/command',{'command':'STOP'})[0],200)
        self.assertEqual(self.request('/api/command',{'command':'OPEN'})[0],400)
        self.assertEqual(self.request('/api/config',DEFAULTS)[0],400)
        self.request('/api/command',{'command':'MAINT'})
        self.assertEqual(self.request('/api/config',DEFAULTS|{'servo_count':2})[0],200)
        self.request('/api/command',{'command':'SAVE'})
        self.assertEqual(self.s.bridge.demo_saved['servo_count'],2)
        self.s.bridge.set_demo(False)
    def test_traversal_denied(self):self.assertEqual(self.request('/../server.py')[0],404)
    def test_static_available(self):self.assertEqual(self.request('/app.js')[0],200)

if __name__=='__main__':unittest.main(verbosity=2)
