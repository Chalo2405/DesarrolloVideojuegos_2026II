using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// El generador de la semana 6 debe terminar antes de leer el terreno.
[DefaultExecutionOrder(100)]
public class CuadriculaS07 : MonoBehaviour
{
    [SerializeField] private Tilemap tilemapSuelo;
    [SerializeField] private int xMin = -10;
    [SerializeField] private int xMax = 29;
    [SerializeField] private int yMin = -7;
    [SerializeField] private int yMax = 7;
    [SerializeField] private bool dibujarBloqueados = true;
    private NodoS07[,] nodos;
    private static readonly Vector3Int[] direcciones =
    {
        Vector3Int.up, Vector3Int.right, Vector3Int.down, Vector3Int.left
    };

    public int Ancho { get { return xMax - xMin + 1; } }
    public int Alto { get { return yMax - yMin + 1; } }

    void Start() { Construir(); }

    public void Construir()
    {
        nodos = new NodoS07[Ancho, Alto];
        int bloqueados = 0;
        for (int x = 0; x < Ancho; x++)
        {
            for (int y = 0; y < Alto; y++)
            {
                Vector3Int celda = new Vector3Int(xMin + x, yMin + y, 0);
                bool libre = !tilemapSuelo.HasTile(celda);
                if (!libre) bloqueados++;
                nodos[x, y] = new NodoS07(celda, libre);
            }
        }
        int total = Ancho * Alto;
        Debug.Log("Cuadrícula construida: " + Ancho + " x " + Alto + " = " + total
            + " nodos | transitables = " + (total - bloqueados) + " | bloqueados = " + bloqueados);
    }

    public NodoS07 NodoEnCelda(Vector3Int celda)
    {
        int x = celda.x - xMin;
        int y = celda.y - yMin;
        if (nodos == null || x < 0 || x >= Ancho || y < 0 || y >= Alto) return null;
        return nodos[x, y];
    }

    public NodoS07 NodoEnMundo(Vector3 posicion)
    {
        return NodoEnCelda(tilemapSuelo.WorldToCell(posicion));
    }

    public Vector3 CentroDeCelda(Vector3Int celda)
    {
        return tilemapSuelo.GetCellCenterWorld(celda);
    }

    public List<NodoS07> Vecinos(NodoS07 nodo)
    {
        List<NodoS07> lista = new List<NodoS07>();
        foreach (Vector3Int direccion in direcciones)
        {
            NodoS07 vecino = NodoEnCelda(nodo.celda + direccion);
            if (vecino != null && vecino.transitable) lista.Add(vecino);
        }
        return lista;
    }

    public void ReiniciarCostos()
    {
        foreach (NodoS07 nodo in nodos)
        {
            nodo.g = int.MaxValue;
            nodo.h = 0;
            nodo.padre = null;
        }
    }

    public Vector3Int CeldaLibreMasBaja(int x)
    {
        for (int y = yMin; y <= yMax; y++)
        {
            NodoS07 nodo = NodoEnCelda(new Vector3Int(x, y, 0));
            if (nodo != null && nodo.transitable) return nodo.celda;
        }
        return new Vector3Int(x, yMax, 0);
    }

    void OnDrawGizmos()
    {
        if (nodos == null || !dibujarBloqueados) return;
        Gizmos.color = new Color(1f, 0f, 0f, 0.35f);
        foreach (NodoS07 nodo in nodos)
            if (!nodo.transitable)
                Gizmos.DrawCube(CentroDeCelda(nodo.celda), new Vector3(0.9f, 0.9f, 0.01f));
    }
}
