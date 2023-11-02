<div align="center">
  <img src="Resources/icon.ico" alt="App Icon" width="124" style="display:inline; vertical-align:middle;">

  # Terrors of Nowhere: Save Manager
  Herramienta para administrar tus **Códigos de Guardado** para que puedas jugar y recuperar tus códigos más tarde si olvidaste hacer una copia de seguridad. Además, guarda tu historial de códigos localmente para que puedas usarlos mas tarde.

  # [Descargar](https://github.com/ChrisFeline/ToNSaveManager/releases/latest/download/ToNSaveManager.zip "Use this link to download the latest version directly from GitHub.")

  [Versiones](https://github.com/ChrisFeline/ToNSaveManager/releases "Show a list of current and previous releases.") • 
  [Reciente](https://github.com/ChrisFeline/ToNSaveManager/releases/latest "Show information about the latest release.") • 
  [Como Utilizar](#-faq)
</div>

<p align="center">
  <img src="Resources/preview.png" alt="Preview" title="Bu! Te asusté!">
</p>

# 🛠️ Características y Aclaraciones
- Escanea automáticamente tus logs en busca de **Códigos de Guardado** anteriores.
- Mientras la herramienta esté abierta, detectará nuevos códigos a medida que juegas.
- Los códigos de guardado detectados previamente se guardarán en una base de datos local, de modo que si VRChat borra los registros con el tiempo, tendrás un historial de códigos local y seguro.

### Settings (Configuración)
- `Auto Clipboard Copy` Copiará automáticamente los nuevos códigos de guardado al portapapeles.
- `Notification Sounds` Reproducirá una notificación cuando se detecte un nuevo guardado.
  * `Select Custom Sound` Abrirá un cuadro de diálogo para seleccionar un archivo, y podrás elegir qué audio se reproduce al recibir una notificación. (Debe ser en formato **.wav**).
  * `Clear Custom Sound` Reestablece audio personalizado al predeterminado.
- `Collect Player Names` Al pasar el cursor sobre un código de guardado se mostrará qué jugadores estaban en la sala en el momento de el guardado.
- `Check For Updates` Comprobará este repositorio de GitHub en busca de nuevas versiones y te sugerirá intentar una actualización automática.

### Menús de Clic Derecho
- ### Fechas de Logs (Panel izquierdo)
  * `Import` Puedes introducir tu propio código y guardarlo en esa colección.
  * `Rename` Te permite cambiar el nombre de una colección.
  * `Delete` Elimina todo el log seleccionado de la base de datos.
- ### Códigos de guardado (Panel derecho)
  * `Add to` Te permite guardar o marcar este código en una colección personalizada separada con un nombre de tu elección.
  * `Edit Note` Puedes adjuntar una nota a este código de guardado para poder reconocerlo mejor.
  * `Delete` Elimina solo este código de guardado de la base de datos.
  
### Objectives Window (Ventana de Objetivos)
- Esta ventana te proporciona una lista de elementos desbloqueables que puedes marcar para hacer un seguimiento de tu progreso. Simplemente haz clic en las cosas que ya has desbloqueado.

# 📋 FAQ:
### Preguntas frecuentes
> ## ¿Cómo lo utilizo?
> 1. Descarga en la [<u>versión mas reciente</u>](https://github.com/ChrisFeline/ToNSaveManager/releases/latest), el archivo comprimido `ToNSaveManager.zip`.
> 2. Extrae el contenido del archivo **.zip** en una carpeta de tu elección.
> 3. Abrir `ToNSaveManager.exe`.
> 4. Selecciona la fecha del registro a la izquierda y luego haz clic en uno de los guardados a la derecha.
> 5. Tu código se encuentra ahora en el portapapeles. Ve a VRChat y pega el código en el panel de guardado.

> ## ¿Dónde puedo dejar mis sugerencias?
> Si deseas sugerir nuevas funciones o cambios, puedes abrir un (Issue) aquí o contactarme en el Discord oficial de [Toren Discord](https://discord.gg/bus-to-nowhere) como @**Kittenji**

> ## ¿Por qué el .exe es tan grande? >100MB
> El archivo .exe es combinado con el framework .NET que se requiere para ejecutar el programa. Se utiliza un argumento de línea de comandos para la publicación de dotnet: `--self-contained true -p:PublishSingleFile=true`
> Esto agrega tamaño al archivo, pero garantiza que el programa se ejecute de manera independiente sin depender de una instalación previa de .NET
>
> Esto se hace para que las personas que descargan el programa no tengan que descargar el framework de .NET por sí mismas y esté listo para ejecutarse sin ninguna acción adicional por parte del usuario.
>
> El programa es compilado directamente a partir del código fuente utilizando las [Acciones de GitHub](https://github.com/ChrisFeline/ToNSaveManager/blob/a0d503b02fe25fde1b36ca9807756f1830c8e7a8/.github/workflows/dotnet-desktop.yml#L46C45-L46C45).

> ## ¿Esto va en contra de los Términos de Servicio de VRChat?
> - **Respuesta breve:** No
>
> Este es una herramienta externa que utiliza archivos de texto plano locales que VRChat escribe en la carpeta Local APPDATA.
> Está permitido leer estos archivos ya que no modifican ni alteran el juego de ninguna manera.
> **Esto no es un mod ni un cheat.**
