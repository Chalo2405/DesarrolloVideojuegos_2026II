using UnityEngine;

// Experimento: compara buscar un objeto o un componente en cada repetición con guardar la referencia.
public class MedidorComponentesS08 : MonoBehaviour
{
    [SerializeField] private int repeticiones = 100000;

    void Start()
    {
        System.Diagnostics.Stopwatch reloj = new System.Diagnostics.Stopwatch();
        int visibles = 0;

        // 1. GameObject.Find en cada repetición: recorre los objetos de la escena buscando el nombre
        reloj.Start();
        for (int i = 0; i < repeticiones; i++)
        {
            GameObject encontrado = GameObject.Find("JugadorS06");
            if (encontrado != null) visibles++;
        }
        reloj.Stop();
        double conFind = reloj.Elapsed.TotalMilliseconds;

        // 2. GetComponent en cada repetición: busca el componente dentro del mismo GameObject
        reloj.Restart();
        for (int i = 0; i < repeticiones; i++)
        {
            SpriteRenderer buscado = GetComponent<SpriteRenderer>();
            if (buscado.enabled) visibles++;
        }
        reloj.Stop();
        double conGetComponent = reloj.Elapsed.TotalMilliseconds;

        // 3. Referencia guardada: se busca una sola vez y se reutiliza
        SpriteRenderer guardado = GetComponent<SpriteRenderer>();
        reloj.Restart();
        for (int i = 0; i < repeticiones; i++)
        {
            if (guardado.enabled) visibles++;
        }
        reloj.Stop();
        double conReferencia = reloj.Elapsed.TotalMilliseconds;

        Debug.Log("S08 | Medición con " + repeticiones + " repeticiones (" + visibles + " lecturas) | Find = "
            + conFind.ToString("F2") + " ms | GetComponent = " + conGetComponent.ToString("F2")
            + " ms | referencia guardada = " + conReferencia.ToString("F2") + " ms");
    }
}
