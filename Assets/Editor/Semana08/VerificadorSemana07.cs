using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

// Verificador del Paso 1 de la práctica de la Semana 8.
public static class VerificadorSemana07
{
    const string EscenaS07 = "Assets/Scenes/Semana07_ParticulasEIA.unity";
    const string EscenaS08 = "Assets/Scenes/Semana08_EventosYCorrutinas.unity";
    static int aprobadas, total;

    [MenuItem("Semana08/Verificar Semana 7 y crear escena")]
    public static void Verificar()
    {
        if (EditorApplication.isPlaying) { Debug.LogWarning("Sal del modo Play antes de ejecutar la verificación."); return; }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        aprobadas = total = 0;
        Comprobar(File.Exists(EscenaS07), "Existe la escena Semana07_ParticulasEIA");
        string[] scripts = { "EfectosJugadorS07", "NodoS07", "CuadriculaS07", "BuscadorRutasS07", "PruebaRutasS07", "EnemigoS07" };
        foreach (string nombre in scripts)
            Comprobar(File.Exists("Assets/Scripts/Semana07/" + nombre + ".cs"), "Existe el script " + nombre);
        Comprobar(LayerMask.NameToLayer("Jugador") != -1, "Existe la capa de física Jugador");
        int semilla = -1, digito = -1, nidoX = -1;
        if (File.Exists(EscenaS07))
        {
            Scene escena = EditorSceneManager.OpenScene(EscenaS07, OpenSceneMode.Single);
            Transform portal = Buscar<Transform>(escena, "PortalS07");
            Transform jugador = Buscar<Transform>(escena, "JugadorS06");
            Transform rutas = Buscar<Transform>(escena, "SistemaRutas");
            Transform enemigo = Buscar<Transform>(escena, "EnemigoS07");
            Transform generador = Buscar<Transform>(escena, "GeneradorTerritorioB");
            Tilemap suelo = Buscar<Tilemap>(escena, "TilemapSuelo");
            ParticleSystem psPortal = Buscar<ParticleSystem>(escena, "PS_PortalAmbiental");
            ParticleSystem psPolvo = Buscar<ParticleSystem>(escena, "PS_PolvoAterrizaje");
            ParticleSystem psEstela = Buscar<ParticleSystem>(escena, "PS_EstelaEnemigo");
            ParticleSystem psImpacto = Buscar<ParticleSystem>(escena, "PS_ImpactoAtaque");
            Comprobar(portal != null && portal.GetComponent("PortalTerritorio") == null && psPortal != null && psPortal.transform.parent == portal,
                "PortalS07 no tiene Portal Territorio y contiene PS_PortalAmbiental");
            Comprobar(psPortal != null && psPortal.main.maxParticles == 50, "PS_PortalAmbiental tiene Max Particles = 50");
            Comprobar(jugador != null && jugador.gameObject.layer == LayerMask.NameToLayer("Jugador") && jugador.GetComponent("EfectosJugadorS07") != null && psPolvo != null && psPolvo.transform.parent == jugador,
                "JugadorS06 tiene la capa Jugador, Efectos Jugador S07 y PS_PolvoAterrizaje");
            Comprobar(suelo != null && suelo.HasTile(new Vector3Int(-4,1,0)) && suelo.HasTile(new Vector3Int(12,5,0)) && suelo.HasTile(new Vector3Int(30,0,0)),
                "TilemapSuelo tiene el techo refugio, el muro flotante y el muro de cierre");
            Component pruebaRutas = rutas != null ? rutas.GetComponent("PruebaRutasS07") : null;
            Comprobar(rutas != null && rutas.GetComponent("CuadriculaS07") != null && rutas.GetComponent("BuscadorRutasS07") != null && pruebaRutas != null,
                "SistemaRutas tiene Cuadricula S07, Buscador Rutas S07 y Prueba Rutas S07");
            Component scriptEnemigo = enemigo != null ? enemigo.GetComponent("EnemigoS07") : null;
            Comprobar(scriptEnemigo != null && psEstela != null && psEstela.transform.parent == enemigo && psImpacto != null && psImpacto.transform.parent == enemigo,
                "EnemigoS07 tiene su script, PS_EstelaEnemigo y PS_ImpactoAtaque");
            Comprobar(TieneReferencia(scriptEnemigo,"jugador") && TieneReferencia(scriptEnemigo,"cuadricula") && TieneReferencia(scriptEnemigo,"buscador") && TieneReferencia(scriptEnemigo,"estela") && TieneReferencia(scriptEnemigo,"impacto"),
                "El enemigo tiene asignadas sus cinco referencias");
            if (generador != null) semilla = LeerEntero(generador.GetComponent("GeneradorTerritorioProcedural"),"semilla");
            digito = LeerEntero(scriptEnemigo,"ultimoDigitoDNI");
            nidoX = LeerCeldaX(scriptEnemigo,"celdaNido");
            int inicioX = LeerCeldaX(pruebaRutas,"celdaInicio");
            Comprobar(semilla >= 0 && digito >= 0 && semilla % 10 == digito,
                "El ultimo Digito DNI del enemigo (" + digito + ") corresponde a la Semilla (" + semilla + ")");
            Comprobar(semilla >= 0 && nidoX >= 0 && (semilla / 10) % 10 == nidoX && inicioX == nidoX,
                "Celda Nido X (" + nidoX + ") y Celda Inicio X (" + inicioX + ") corresponden al penúltimo dígito de la Semilla (" + semilla + ")");
        }
        if (aprobadas < total)
        {
            Debug.LogError("VERIFICACIÓN SEMANA 7: NO APROBADA (" + aprobadas + " de " + total
                + "). Corrige los puntos marcados con [FALTA] siguiendo la práctica de la Semana 7 y vuelve a ejecutar este menú. La escena de la Semana 8 no se ha creado.");
            return;
        }
        if (!File.Exists(EscenaS08)) AssetDatabase.CopyAsset(EscenaS07, EscenaS08);
        EditorSceneManager.OpenScene(EscenaS08, OpenSceneMode.Single);
        Debug.Log("VERIFICACIÓN SEMANA 7: APROBADA (" + aprobadas + " de " + total + "). Últimos 3 dígitos del DNI"
            + " según tu proyecto = " + semilla.ToString("D3") + " | Configuración de la Semana 8 = "
            + (digito % 2 == 0 ? "A (dígito par)" : "B (dígito impar)") + " | Escena abierta: " + EscenaS08);
    }
    static void Comprobar(bool condicion,string descripcion)
    {
        total++;
        if(condicion) { aprobadas++; Debug.Log("[OK] " + descripcion); }
        else Debug.LogWarning("[FALTA] " + descripcion);
    }
    static T Buscar<T>(Scene escena,string nombre) where T : Component
    {
        foreach(GameObject raiz in escena.GetRootGameObjects())
            foreach(T componente in raiz.GetComponentsInChildren<T>(true))
                if(componente.name == nombre) return componente;
        return null;
    }
    static int LeerEntero(Component componente,string campo)
    {
        if(componente==null) return -1;
        SerializedProperty propiedad=new SerializedObject(componente).FindProperty(campo);
        return propiedad==null ? -1 : propiedad.intValue;
    }
    static int LeerCeldaX(Component componente,string campo)
    {
        if(componente==null) return -1;
        SerializedProperty propiedad=new SerializedObject(componente).FindProperty(campo);
        return propiedad==null ? -1 : propiedad.vector3IntValue.x;
    }
    static bool TieneReferencia(Component componente,string campo)
    {
        if(componente==null) return false;
        SerializedProperty propiedad=new SerializedObject(componente).FindProperty(campo);
        return propiedad!=null && propiedad.objectReferenceValue!=null;
    }
}
