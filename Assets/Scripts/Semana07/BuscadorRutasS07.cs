using System.Collections.Generic;
using UnityEngine;

public enum HeuristicaS07 { Ninguna, Manhattan, Euclidiana }

[RequireComponent(typeof(CuadriculaS07))]
public class BuscadorRutasS07 : MonoBehaviour
{
    private const int CostoPorPaso = 10;
    private CuadriculaS07 cuadricula;
    public int UltimosExplorados { get; private set; }

    void Awake() { cuadricula = GetComponent<CuadriculaS07>(); }

    public List<Vector3> BuscarRuta(Vector3 origen, Vector3 destino, HeuristicaS07 heuristica)
    {
        UltimosExplorados = 0;
        NodoS07 inicio = cuadricula.NodoEnMundo(origen);
        NodoS07 meta = cuadricula.NodoEnMundo(destino);
        if (inicio == null || meta == null || !inicio.transitable || !meta.transitable) return null;

        cuadricula.ReiniciarCostos();
        List<NodoS07> abierta = new List<NodoS07>();
        HashSet<NodoS07> cerrada = new HashSet<NodoS07>();
        inicio.g = 0;
        inicio.h = Estimar(inicio, meta, heuristica);
        abierta.Add(inicio);

        while (abierta.Count > 0)
        {
            NodoS07 actual = abierta[0];
            for (int i = 1; i < abierta.Count; i++)
                if (abierta[i].F < actual.F || (abierta[i].F == actual.F && abierta[i].h < actual.h))
                    actual = abierta[i];

            abierta.Remove(actual);
            cerrada.Add(actual);
            UltimosExplorados++;
            if (actual == meta) return Reconstruir(inicio, meta);

            foreach (NodoS07 vecino in cuadricula.Vecinos(actual))
            {
                if (cerrada.Contains(vecino)) continue;
                int costoNuevo = actual.g + CostoPorPaso;
                if (costoNuevo < vecino.g)
                {
                    vecino.g = costoNuevo;
                    vecino.h = Estimar(vecino, meta, heuristica);
                    vecino.padre = actual;
                    if (!abierta.Contains(vecino)) abierta.Add(vecino);
                }
            }
        }
        return null;
    }

    int Estimar(NodoS07 desde, NodoS07 hasta, HeuristicaS07 heuristica)
    {
        int dx = Mathf.Abs(desde.celda.x - hasta.celda.x);
        int dy = Mathf.Abs(desde.celda.y - hasta.celda.y);
        if (heuristica == HeuristicaS07.Manhattan) return CostoPorPaso * (dx + dy);
        if (heuristica == HeuristicaS07.Euclidiana)
            return Mathf.FloorToInt(CostoPorPaso * Mathf.Sqrt(dx * dx + dy * dy));
        return 0;
    }

    List<Vector3> Reconstruir(NodoS07 inicio, NodoS07 meta)
    {
        List<Vector3> ruta = new List<Vector3>();
        NodoS07 actual = meta;
        while (actual != inicio)
        {
            ruta.Add(cuadricula.CentroDeCelda(actual.celda));
            actual = actual.padre;
        }
        ruta.Reverse();
        return ruta;
    }
}
