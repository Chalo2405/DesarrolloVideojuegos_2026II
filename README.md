# Desarrollo de Videojuegos — 2026 II

Avances guardados hasta el **8 de octubre de 2026**. El repositorio contiene el proyecto de Unity y el material de apoyo para continuar el trabajo en otra computadora.

## Abrir el proyecto en tu PC principal

1. Instala Unity Hub y el editor **Unity 6000.3.23f1**, la versión registrada en `ProjectSettings/ProjectVersion.txt`.
2. En GitHub Desktop, inicia sesión con tu cuenta y clona `Chalo2405/DesarrolloVideojuegos_2026II` en una carpeta de tu PC. También puedes clonarlo con Git:

   ```sh
   git clone https://github.com/Chalo2405/DesarrolloVideojuegos_2026II.git
   ```

3. En Unity Hub, agrega el proyecto desde el disco seleccionando la carpeta clonada que contiene `Assets`, `Packages` y `ProjectSettings`.
4. Ábrelo con Unity **6000.3.23f1**. La primera apertura necesita conexión para descargar los paquetes y puede tardar mientras Unity importa los recursos.
5. En la ventana Project, abre `Assets/Scenes` y haz doble clic en la escena de la semana que quieras continuar. Para el avance más reciente, abre `Semana06_TileBasedyTerritorios.unity`.

## Trabajos incluidos

| Semana | Escena |
| --- | --- |
| 1 | `Assets/Scenes/Semana01_GenerosVideojuegos.unity` |
| 2 | `Assets/Scenes/Semana02_TeoriaDeJuegos.unity` |
| 3 | `Assets/Scenes/Semana03_AzaryBalance.unity` |
| 5 | `Assets/Scenes/Semana05_InterfazyRendering2D.unity` |
| 6 | `Assets/Scenes/Semana06_TileBasedyTerritorios.unity` |

También están incluidos los scripts, herramientas del editor, animaciones, audio, fotografías, sprites, prefabs, tiles, archivos `.meta`, configuración del proyecto y versiones de los paquetes.

En `MaterialDeApoyo` se conservan copias de los archivos que estaban junto al proyecto:

- `registro-evidencia.html`: herramienta de registro de evidencia; se abre en un navegador.
- `Rubrica/rubrica-09.png`, `rubrica-10.png` y `rubrica-11.png`: imágenes de la rúbrica.

## Continuar entre las dos computadoras

1. Antes de trabajar, usa **Fetch origin** y, si aparece, **Pull origin** en GitHub Desktop para descargar los cambios.
2. Guarda las escenas y los recursos en Unity cuando termines.
3. En GitHub Desktop, revisa los cambios, escribe un resumen, crea el commit y pulsa **Push origin**.
4. En la otra computadora, descarga esos cambios antes de abrir el proyecto. Evita modificar la misma escena a la vez en ambos equipos.

Conserva los archivos `.meta` junto a sus recursos: mantienen las referencias de las escenas. Las carpetas `Library`, `Temp`, `Logs` y `UserSettings` son datos locales que Unity vuelve a generar y no se incluyen en GitHub.

La lista de escenas para compilar todavía referencia `SampleScene.unity`, que ya no existe. Para generar un ejecutable, selecciona primero la escena que quieras incluir en la configuración de compilación de Unity. Esto no impide abrir y editar las escenas anteriores.
