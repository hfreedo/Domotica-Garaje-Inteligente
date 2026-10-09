"""Assemble release archives with fresh staging and verified driver."""
from pathlib import Path
import hashlib
import json
import shutil
import subprocess
import tempfile
import zipfile

ROOT = Path(__file__).resolve().parents[1]
VERSION = (ROOT / 'VERSION').read_text().strip()


def digest(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest()


def source_files():
    result = subprocess.run(['git', 'ls-files', '--cached', '--others', '--exclude-standard', '-z'],
                            cwd=ROOT, check=True, capture_output=True)
    paths = sorted(set(result.stdout.decode('utf-8').split('\0')) - {''})
    for name in paths:
        path = ROOT / name
        if path.is_file():
            if path.stat().st_size >= 100 * 1024 * 1024:
                raise RuntimeError(f'Archivo demasiado grande para GitHub: {name}')
            yield path, name


def main():
    driver = ROOT / 'support/ch340/CH341SER.EXE'
    manifest = json.loads((driver.parent / 'procedencia.json').read_text())
    if not driver.exists() or digest(driver) != manifest['sha256']:
        raise RuntimeError('Descargue y verifique CH340 con scripts/descargar_ch340.ps1 primero.')
    with driver.open('rb') as stream:
        if stream.read(2) != b'MZ':
            raise RuntimeError('El driver no es un ejecutable Windows.')
    server = ROOT / '.build/server-dist/GarajeServidor'
    panel = ROOT / '.build/panel/PanelGaraje.exe'
    if not (server / 'GarajeServidor.exe').is_file() or not panel.is_file():
        raise RuntimeError('Compile primero con scripts/build_windows.ps1.')
    sources = list(source_files())
    release = ROOT / f'entregas/release-{VERSION}'
    release.mkdir(parents=True, exist_ok=True)
    name = f'GarajeInteligente_v{VERSION}_Windows'
    with tempfile.TemporaryDirectory(prefix='release-', dir=ROOT / '.build') as tmp:
        dest = Path(tmp) / name
        shutil.copytree(server, dest)
        shutil.copy2(panel, dest / panel.name)
        for path, relative in sources:
            target = dest / relative
            target.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(path, target)
        shutil.copytree(ROOT / 'interfaz_garaje/static', dest / '_internal/static', dirs_exist_ok=True)
        shutil.copy2(driver, dest / 'support/ch340' / driver.name)
        windows_zip = release / f'{name}.zip'
        with zipfile.ZipFile(windows_zip, 'w', zipfile.ZIP_DEFLATED) as archive:
            for path in sorted(dest.rglob('*')):
                if path.is_file():
                    archive.write(path, path.relative_to(dest.parent))
    source_zip = release / f'GarajeInteligente_v{VERSION}_Fuentes.zip'
    with zipfile.ZipFile(source_zip, 'w', zipfile.ZIP_DEFLATED) as archive:
        for path, relative in sources:
            archive.write(path, f'GarajeInteligente_v{VERSION}_Fuentes/{relative}')
    shutil.copy2(ROOT / f'docs/RELEASE_{VERSION}.md', release / 'NOTAS_RELEASE.md')
    hashes = ''.join(f'{digest(path)}  {path.name}\n' for path in [windows_zip, source_zip])
    (release / 'SHA256SUMS.txt').write_text(hashes, encoding='utf-8')
    print(release)
    print(hashes)


if __name__ == '__main__':
    main()
