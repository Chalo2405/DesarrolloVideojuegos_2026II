using UnityEngine;

// Datos de una celda; no es un componente de escena.
public class NodoS07
{
    public Vector3Int celda;
    public bool transitable;
    public int g;
    public int h;
    public NodoS07 padre;
    public int F { get { return g + h; } }

    public NodoS07(Vector3Int celda, bool transitable)
    {
        this.celda = celda;
        this.transitable = transitable;
    }
}
