using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

// Comprueba la configuración guardada sin modificar la escena ni sus objetos.
internal static class ComprobadorLocalSemana07
{
    static int total;
    static int correctas;

    [MenuItem("Semana07/Comprobar escena terminada")]
    public static void Verificar()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogWarning("Sal del modo Play antes de comprobar la configuración guardada.");
            return;
        }
        if (SceneManager.GetActiveScene().path != "Assets/Scenes/Semana07_ParticulasEIA.unity")
        {
            Debug.LogWarning("Abre Semana07_ParticulasEIA antes de ejecutar esta comprobación.");
            return;
        }
        total = correctas = 0;
        try
        {
            var jugador = GameObject.Find("JugadorS06");
            var portal = GameObject.Find("PortalS07");
            var sistema = GameObject.Find("SistemaRutas");
            var enemigo = GameObject.Find("EnemigoS07");
            var suelo = GameObject.Find("TilemapSuelo").GetComponent<Tilemap>();
            var regla = AssetDatabase.LoadAssetAtPath<TileBase>("Assets/Tiles/Semana06/ReglaSuelo.asset");
            Check(portal != null && portal.GetComponent<PortalTerritorio>() == null, "Portal visual sin carga de habitación");
            Check(jugador.layer == LayerMask.NameToLayer("Jugador") && jugador.CompareTag("Player"), "Jugador: capa y etiqueta");
            Check(jugador.GetComponent<ControladorJugadorS06>() != null && jugador.GetComponent<EfectosJugadorS07>() != null, "Movimiento y efectos del jugador");
            Check(Enumerable.Range(-5,3).All(x=>suelo.GetTile(new Vector3Int(x,1,0))==regla), "Techo refugio");
            Check(Enumerable.Range(2,6).All(y=>suelo.GetTile(new Vector3Int(12,y,0))==regla), "Muro flotante");
            Check(Enumerable.Range(-7,15).All(y=>suelo.GetTile(new Vector3Int(30,y,0))==regla), "Muro de cierre");
            Check(Enumerable.Range(2,4).All(y=>suelo.GetTile(new Vector3Int(10,y,0))==regla), "Obstáculo propio de cuatro celdas");
            Check(Enumerable.Range(-8,18).All(x=>!suelo.HasTile(new Vector3Int(x,3,0))), "Corredor de patrulla libre");
            Check(sistema.GetComponent<CuadriculaS07>() && sistema.GetComponent<BuscadorRutasS07>() && sistema.GetComponent<PruebaRutasS07>(), "Sistema de cuadrícula, A* y comparación de heurísticas");
            var ia = enemigo.GetComponent<EnemigoS07>();
            var datos = new SerializedObject(ia);
            Check(datos.FindProperty("ultimoDigitoDNI").intValue==8 && datos.FindProperty("celdaNido").vector3IntValue==new Vector3Int(4,3,0), "DNI: dígito 8 y nido (4,3)");
            Check(new SerializedObject(sistema.GetComponent<PruebaRutasS07>()).FindProperty("celdaInicio").vector3IntValue==new Vector3Int(4,3,0), "Inicio de la prueba según DNI");
            Check(new SerializedObject(GameObject.Find("GeneradorTerritorioB").GetComponent<GeneradorTerritorioProcedural>()).FindProperty("semilla").intValue==248, "Semilla 248");
            Check(datos.FindProperty("capaJugador").intValue==LayerMask.GetMask("Jugador") && datos.FindProperty("capaSuelo").intValue==LayerMask.GetMask("Suelo"), "Máscaras de percepción");
            Check(new[]{"jugador","cuadricula","buscador","estela","impacto"}.All(n=>datos.FindProperty(n).objectReferenceValue!=null), "Referencias del enemigo completas");
            Check(enemigo.GetComponent<Rigidbody2D>()==null, "Enemigo volador controlado por Transform");
            var ambiental = GameObject.Find("PS_PortalAmbiental").GetComponent<ParticleSystem>();
            var polvo = GameObject.Find("PS_PolvoAterrizaje").GetComponent<ParticleSystem>();
            var estela = GameObject.Find("PS_EstelaEnemigo").GetComponent<ParticleSystem>();
            var impacto = GameObject.Find("PS_ImpactoAtaque").GetComponent<ParticleSystem>();
            Check(ambiental.transform.parent==portal.transform && ambiental.main.maxParticles==50 && ambiental.main.playOnAwake && ambiental.main.loop && ambiental.emission.rateOverTime.constant==20, "Portal: emisión continua, límite final 50");
            Check(ambiental.colorOverLifetime.enabled && ambiental.sizeOverLifetime.enabled && ambiental.velocityOverLifetime.enabled && ambiental.shape.radiusThickness==0, "Portal: anillo, desvanecimiento, tamaño y ascenso");
            Check(polvo.transform.parent==jugador.transform && !polvo.main.loop && !polvo.main.playOnAwake && polvo.main.simulationSpace==ParticleSystemSimulationSpace.World && polvo.emission.GetBurst(0).count.constant==12, "Polvo: ráfaga de 12 en espacio World");
            Check(estela.transform.parent==enemigo.transform && estela.main.maxParticles==60 && estela.main.simulationSpace==ParticleSystemSimulationSpace.World && estela.emission.rateOverTime.constant==0 && estela.emission.rateOverDistance.constant==6, "Estela: emisión por distancia");
            Check(impacto.transform.parent==enemigo.transform && impacto.emission.GetBurst(0).count.constant==20 && impacto.main.gravityModifier.constant==0 && impacto.shape.arc==360, "Impacto: ráfaga roja de 20");
            Check(new[]{ambiental,polvo,estela,impacto}.All(p=>p.GetComponent<ParticleSystemRenderer>().sharedMaterial!=null && p.GetComponent<ParticleSystemRenderer>().sharedMaterial.shader.isSupported), "Materiales de partículas compatibles");
            int faltantes = SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Transform>(true)).Sum(t=>GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject));
            Check(faltantes==0, "Sin scripts perdidos en la escena");
        }
        catch (Exception e) { Check(false, "Configuración incompleta: " + e.Message); }
        string mensaje = "VERIFICACIÓN SEMANA 7: " + (correctas==total ? "APROBADA" : "REVISAR")
            + " (" + correctas + " de " + total + "). Semilla = 248 | Nido = (4, 3) | Último dígito = 8.";
        if (correctas==total) Debug.Log(mensaje); else Debug.LogError(mensaje);
    }

    static void Check(bool valor, string descripcion)
    {
        total++;
        if(valor) correctas++; else Debug.LogWarning("[FALTA S07] " + descripcion);
    }
}
