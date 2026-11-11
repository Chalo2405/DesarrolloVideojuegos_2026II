using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(200)]
public class PruebaRutasS07 : MonoBehaviour
{
    [Header("Personalización por DNI")]
    [Tooltip("X = penúltimo dígito de tu DNI, tal cual. Y = 3")]
    [SerializeField] private Vector3Int celdaInicio = new Vector3Int(0, 3, 0);
    [Header("Prueba")]
    [SerializeField] private int columnaMeta = 20;
    [SerializeField] private CuadriculaS07 cuadricula;
    [SerializeField] private BuscadorRutasS07 buscador;
    private List<Vector3> rutaDibujada;
    private Vector3 origenDibujado;

    void Start()
    {
        Vector3Int celdaMeta = cuadricula.CeldaLibreMasBaja(columnaMeta);
        Vector3 origen = cuadricula.CentroDeCelda(celdaInicio);
        Vector3 destino = cuadricula.CentroDeCelda(celdaMeta);
        Debug.Log("Prueba de rutas: inicio " + celdaInicio + " | meta " + celdaMeta);
        HeuristicaS07[] heuristicas =
        {
            HeuristicaS07.Ninguna, HeuristicaS07.Euclidiana, HeuristicaS07.Manhattan
        };
        foreach (HeuristicaS07 heuristica in heuristicas)
        {
            List<Vector3> ruta = buscador.BuscarRuta(origen, destino, heuristica);
            string pasos = ruta == null ? "sin ruta" : ruta.Count.ToString();
            Debug.Log("Heurística " + heuristica + ": nodos explorados = " + buscador.UltimosExplorados
                + " | pasos de la ruta = " + pasos);
            rutaDibujada = ruta;
        }
        origenDibujado = origen;
    }

    void OnDrawGizmos()
    {
        if (rutaDibujada == null || rutaDibujada.Count == 0) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origenDibujado, rutaDibujada[0]);
        for (int i = 0; i < rutaDibujada.Count - 1; i++)
            Gizmos.DrawLine(rutaDibujada[i], rutaDibujada[i + 1]);
    }
}
