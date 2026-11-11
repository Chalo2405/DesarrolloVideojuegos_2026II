using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

// Script de editor: agrega el menú "Semana07 > Verificar Semana 6 y crear escena".
// Solo crea la escena de la Semana 7 si la Semana 6 pasa todas las comprobaciones.
public static class VerificadorSemana06
{
    const string EscenaS06 = "Assets/Scenes/Semana06_TileBasedyTerritorios.unity";
    const string HabitacionS06 = "Assets/Scenes/Semana06_Habitacion.unity";
    const string EscenaS07 = "Assets/Scenes/Semana07_ParticulasEIA.unity";

    static int aprobadas;
    static int total;

    [MenuItem("Semana07/Verificar Semana 6 y crear escena")]
    public static void Verificar()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        aprobadas = 0;
        total = 0;

        // 1. Archivos del proyecto
        Comprobar(File.Exists(EscenaS06), "Existe la escena Semana06_TileBasedyTerritorios");
        Comprobar(File.Exists(HabitacionS06), "Existe la escena Semana06_Habitacion");
        Comprobar(EnListaDeEscenas(EscenaS06) && EnListaDeEscenas(HabitacionS06),
                  "Las dos escenas de la Semana 6 están en Build Profiles > Scene List");

        string[] scripts = { "ControladorJugadorS06", "GeneradorTerritorioProcedural", "DetectorTerritorio",
                             "GestorControlesS06", "PortalTerritorio", "CamaraConLimites" };
        foreach (string nombre in scripts)
        {
            Comprobar(File.Exists("Assets/Scripts/Semana06/" + nombre + ".cs"), "Existe el script " + nombre);
        }

        Comprobar(File.Exists("Assets/Prefabs/Semana06/JugadorS06.prefab"), "Existe el Prefab JugadorS06");
        Comprobar(LayerMask.NameToLayer("Suelo") != -1, "Existe la capa de física Suelo");

        // 2. Tiles
        TileBase regla = AssetDatabase.LoadAssetAtPath<TileBase>("Assets/Tiles/Semana06/ReglaSuelo.asset");
        Comprobar(regla != null && CantidadDeReglas(regla) == 3, "ReglaSuelo es un Rule Tile con 3 reglas");

        Tile cielo = AssetDatabase.LoadAssetAtPath<Tile>("Assets/Tiles/Semana06/Generados/TilesetSemana06_4.asset");
        Tile cueva = AssetDatabase.LoadAssetAtPath<Tile>("Assets/Tiles/Semana06/Generados/TilesetSemana06_5.asset");
        Comprobar(cielo != null && cueva != null, "Existen los Tile Assets de cielo y de cueva");

        // 3. Input System
        InputActionAsset controles = AssetDatabase.LoadAssetAtPath<InputActionAsset>(
            "Assets/Input/Semana06/ControlesSemana06.inputactions");
        InputActionMap mapa = controles != null ? controles.FindActionMap("Jugador") : null;
        Comprobar(mapa != null && mapa.FindAction("Movimiento") != null && mapa.FindAction("Salto") != null
                  && mapa.FindAction("Remapear") != null && mapa.FindAction("Restablecer") != null,
                  "ControlesSemana06 tiene el mapa Jugador con sus cuatro acciones");
        Comprobar(controles != null && controles.FindControlScheme("Teclado") != null
                  && controles.FindControlScheme("Gamepad") != null,
                  "ControlesSemana06 tiene los esquemas Teclado y Gamepad");

        // 4. Contenido de la escena de la Semana 6
        int semilla = -1;
        int digito = -1;
        if (File.Exists(EscenaS06))
        {
            Scene escena = EditorSceneManager.OpenScene(EscenaS06, OpenSceneMode.Single);
            Tilemap suelo = Buscar<Tilemap>(escena, "TilemapSuelo");
            Tilemap fondo = Buscar<Tilemap>(escena, "TilemapFondo");
            Transform jugador = Buscar<Transform>(escena, "JugadorS06");
            Transform generador = Buscar<Transform>(escena, "GeneradorTerritorioB");

            Rigidbody2D cuerpoSuelo = suelo != null ? suelo.GetComponent<Rigidbody2D>() : null;
            Comprobar(suelo != null && suelo.gameObject.layer == LayerMask.NameToLayer("Suelo")
                      && suelo.GetComponent<CompositeCollider2D>() != null
                      && cuerpoSuelo != null && cuerpoSuelo.bodyType == RigidbodyType2D.Static,
                      "TilemapSuelo tiene la capa Suelo, Composite Collider 2D y Rigidbody 2D Static");
            Comprobar(suelo != null && regla != null
                      && suelo.GetTile(new Vector3Int(-10, -4, 0)) == regla
                      && suelo.GetTile(new Vector3Int(-5, -2, 0)) == regla
                      && suelo.GetTile(new Vector3Int(2, 0, 0)) == regla,
                      "El suelo y las dos plataformas están pintados con ReglaSuelo");
            Comprobar(fondo != null && cielo != null && cueva != null
                      && fondo.GetTile(new Vector3Int(0, 0, 0)) == cielo
                      && fondo.GetTile(new Vector3Int(15, 0, 0)) == cueva,
                      "TilemapFondo tiene cielo en el Territorio A y cueva en el Territorio B");
            Comprobar(jugador != null && jugador.CompareTag("Player")
                      && jugador.GetComponent<PlayerInput>() != null
                      && jugador.GetComponent("ControladorJugadorS06") != null
                      && jugador.GetComponent("DetectorTerritorio") != null
                      && jugador.GetComponent("GestorControlesS06") != null,
                      "JugadorS06 tiene el Tag Player, Player Input y sus tres scripts");

            if (generador != null) semilla = LeerEntero(generador.GetComponent("GeneradorTerritorioProcedural"), "semilla");
            if (jugador != null) digito = LeerEntero(jugador.GetComponent("DetectorTerritorio"), "ultimoDigitoDNI");
            Comprobar(semilla >= 0 && digito >= 0 && semilla % 10 == digito,
                      "La Semilla (" + semilla + ") y el ultimo Digito DNI (" + digito + ") corresponden al mismo DNI");
            Comprobar(cueva != null && ((digito % 2 == 0 && cueva.color.g > cueva.color.b)
                      || (digito % 2 == 1 && cueva.color.b > cueva.color.g)),
                      "El color del tile de cueva corresponde a la configuración de tu DNI");
            Comprobar(Buscar<Transform>(escena, "CamaraSeguimiento") != null
                      && Buscar<Transform>(escena, "LimitesMundo") != null
                      && Buscar<Transform>(escena, "PortalHaciaHabitacion") != null,
                      "Existen CamaraSeguimiento, LimitesMundo y PortalHaciaHabitacion");
        }

        // 5. Resultado
        if (aprobadas < total)
        {
            Debug.LogError("VERIFICACIÓN SEMANA 6: NO APROBADA (" + aprobadas + " de " + total
                + "). Corrige los puntos marcados con [FALTA] siguiendo la práctica de la Semana 6"
                + " y vuelve a ejecutar este menú. La escena de la Semana 7 no se ha creado.");
            return;
        }

        if (!File.Exists(EscenaS07)) AssetDatabase.CopyAsset(EscenaS06, EscenaS07);
        EditorSceneManager.OpenScene(EscenaS07, OpenSceneMode.Single);
        Debug.Log("VERIFICACIÓN SEMANA 6: APROBADA (" + aprobadas + " de " + total + "). Semilla = " + semilla
            + " | Ultimo Digito DNI = " + digito + " | Escena abierta: " + EscenaS07);
    }

    static void Comprobar(bool condicion, string descripcion)
    {
        total++;
        if (condicion)
        {
            aprobadas++;
            Debug.Log("[OK] " + descripcion);
        }
        else
        {
            Debug.LogWarning("[FALTA] " + descripcion);
        }
    }

    static bool EnListaDeEscenas(string ruta)
    {
        foreach (EditorBuildSettingsScene escena in EditorBuildSettings.scenes)
        {
            if (escena.path == ruta && escena.enabled) return true;
        }
        return false;
    }

    static T Buscar<T>(Scene escena, string nombre) where T : Component
    {
        foreach (GameObject raiz in escena.GetRootGameObjects())
        {
            foreach (T componente in raiz.GetComponentsInChildren<T>(true))
            {
                if (componente.name == nombre) return componente;
            }
        }
        return null;
    }

    static int LeerEntero(Component componente, string campo)
    {
        if (componente == null) return -1;
        SerializedProperty propiedad = new SerializedObject(componente).FindProperty(campo);
        return propiedad == null ? -1 : propiedad.intValue;
    }

    static int CantidadDeReglas(TileBase tile)
    {
        SerializedProperty reglas = new SerializedObject(tile).FindProperty("m_TilingRules");
        return reglas == null ? -1 : reglas.arraySize;
    }
}