# Publicación en GitHub

La preparación local no crea un repositorio remoto, commits, etiquetas ni releases en GitHub.

1. Revisar `git status --short` y `git diff --cached` antes de publicar. `.gitignore` excluye entregas, compilaciones, credenciales, historial del traslado, referencias originales y la herramienta local de adaptación del panel.
2. Definir la licencia de publicación del código propio con sus titulares. Los avisos de componentes externos no conceden derechos sobre el proyecto completo.
3. Crear un repositorio vacío en GitHub. Desde esta carpeta, ejecutar `git add .`, revisar `git diff --cached --stat` y confirmar con `git commit -m "Preparar Garaje Inteligente 1.0.2"`.
4. Ejecutar `git branch -M main`, agregar el remoto con `git remote add origin URL_DEL_REPOSITORIO` y publicar con `git push -u origin main`. Sustituir la URL por la real.
5. Crear un release con etiqueta `v1.0.2` sobre ese commit. Copiar el texto de `docs/RELEASE_1.0.2.md` y adjuntar los ZIP y `SHA256SUMS.txt` de `entregas/release-1.0.2/`.

El ZIP de fuentes se genera con la lista de archivos admitidos por Git, excluyendo binarios de drivers. El portable contiene el driver y los ejecutables como recursos del release, evitando agregarlos al historial Git.

El workflow de GitHub Actions valida backend, unidades de servo y compilación del panel. Sus resultados solo existirán después de subir el repositorio; no equivalen a pruebas físicas.
