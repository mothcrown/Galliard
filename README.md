# Galliard

### Descripción
Galliard es una aplicación bastante tonta que permite al usuario subir el audio de una partida de rol y generar una novelización de lo que ha pasado en la partida. Para ello usamos dos servicios que instalaremos en local antes de ejecutar la aplicación: [Whisper](https://github.com/openai/whisper) y [Ollama](https://ollama.com/).

Por último Galliard subirá la novelización generada por Ollama a Google Drive, que es lo que el autor usa para editar textos. Realmente todo esto está un poco manga por hombro y centrado en las necesidades del autor, ¡siéntete libre de cambiar y romper cosas a tu gusto! Si usas OneDrive, cárgate la implementación de GoogleDrive y mete tu propio servicio. 

Este proyecto ha sido generado usando [Clean.Architecture.Solution.Template](https://github.com/jasontaylordev/CleanArchitecture) version 9.0.8.

### Requisitos
* Tener [Whisper](https://github.com/openai/whisper) instalado.
* Tener [Ollama](https://ollama.com/) instalado en local o poder consumirlo desde Docker.
  * ¡Lo correcto aquí hubiese sido meterle NET.Aspire, que es más limpito! ¿Quizá en un futuro?
* Requisitos normales de la implementación de Clean Architecture de Jason Taylor (*Node.js*, etc)

### Configuración
En `appsettings.json` añade el **FolderId** de la carpeta de Google Drive donde quieras subir las novelizaciones, y pon tu cuenta de Google (ie. *test@gmail.com*) en **OwnerMail**. El servicio creará el documento y le dará permiso a ese usuario para editarlo.

En la propiedad **Whisper** pasa ruta de el ejecutable de tu instalación de whisper. Si no sabes encontrarlo, ejecuta esto desde Powershell

```bash
(get-command whisper.exe).Source
```
### Ollama

En Galliard usamos el modelo [mannix/llama3.1-8b-lexi](https://ollama.com/mannix/llama3.1-8b-lexi) modificado con el ModelFile para añadir más contexto. Una vez tengas Ollama instalado necesitas ejecutar este script para crear el modelo que vamos a utilizar aquí

```bash
ollama create llama3.1-8b-lexi-high_context -f C:\...\Galliard\OllamaModelfile
```

Siéntete libre de jugar con otros modelos y de cambiar los parámetros de la Modelfile. ¡Quizá nosotros hagamos lo mismo!

### Google Cloud
Si quieres la funcionalidad de subir la novelización a Google Drive hay que pasar por aquí sí o sí. Entra en [Google Cloud](https://console.cloud.google.com/). Crea un nuevo proyecto y marca como enabled los servicios de Google Docs API y Google Drive API. Luego crea una service account en Credenciales con permisos para editar Google Docs y genera una key para conectarnos con ese proyecto. Si todo ha salido bien, te habrás bajado un ficherito .json. Renómbralo como `googleauth.json` y muévelo al directorio raíz del repositorio.  

### Ejecutar
Para ejecutar la aplicación web

```bash
cd .\src\Web\
dotnet watch run
```