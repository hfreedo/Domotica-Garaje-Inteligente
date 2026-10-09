"""Wire configuration contract. Keep field order aligned with control.h."""
FIELDS = ['servo_count','trig','echo','ir_pin','servo1','servo2','min_cm','max_cm',
          'hysteresis_cm','detect_ms','clear_ms','close_ms','abandon_ms','travel_ms',
          's1_closed','s1_open','s2_closed','s2_open','ir_active','boot_auto','calibrated']
DEFAULTS = dict(zip(FIELDS,[1,4,2,7,9,10,3,25,5,300,1500,5000,10000,2500,
                           1100,1900,1900,1100,0,0,0]))
RANGES = {'servo_count':(1,2),'min_cm':(2,199),'max_cm':(3,200),'hysteresis_cm':(1,30),
          'detect_ms':(100,5000),'clear_ms':(500,10000),'close_ms':(1000,60000),
          'abandon_ms':(1000,60000),'travel_ms':(1000,15000),'ir_active':(0,1),
          'boot_auto':(0,1),'calibrated':(0,1)}
for name in ('s1_closed','s1_open','s2_closed','s2_open'): RANGES[name]=(700,2300)
PINS=('trig','echo','ir_pin','servo1','servo2')
COMMANDS={'GET','STOP','RESUME','OPEN','CLOSE','MAINT','EXIT','SAVE','LOAD','DEFAULTS'}

def validate_config(data):
    if not isinstance(data,dict) or set(data)!=set(FIELDS):
        raise ValueError('Configuración incompleta o campos desconocidos.')
    c={}
    for k in FIELDS:
        if type(data[k]) is not int: raise ValueError(f'{k}: debe ser un número entero.')
        c[k]=data[k]
        if k in RANGES and not RANGES[k][0]<=c[k]<=RANGES[k][1]:
            raise ValueError(f'{k}: fuera de rango {RANGES[k]}.')
    if any(c[k] not in (*range(2,13),*range(14,20)) for k in PINS):
        raise ValueError('Pines admitidos: D2–D12 y A0–A5 (14–19).')
    if len({c[k] for k in PINS})!=5: raise ValueError('No se permiten pines repetidos, incluso para el servo inactivo.')
    if c['min_cm']>=c['max_cm']: raise ValueError('La distancia máxima debe superar la mínima.')
    if any(abs(c[f's{i}_open']-c[f's{i}_closed'])<100 for i in (1,2)):
        raise ValueError('Cada servo necesita al menos 100 µs entre extremos.')
    return c

def config_command(data):
    c=validate_config(data)
    return 'CONFIG '+' '.join(str(c[k]) for k in FIELDS)

def command_text(body):
    cmd=body.get('command')
    if cmd in COMMANDS:return cmd
    if cmd=='JOG':
        s,p=body.get('servo'),body.get('pulse')
        if type(s) is int and type(p) is int and s in (1,2) and 700<=p<=2300:
            return f'JOG {s} {p}'
    raise ValueError('Comando o parámetros no válidos.')
