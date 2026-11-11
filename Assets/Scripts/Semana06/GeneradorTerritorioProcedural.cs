using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GeneradorTerritorioProcedural : MonoBehaviour
{
    [Header("Personalización por DNI")]
    [Tooltip("Escribe aquí, tal cual, los últimos 3 dígitos de tu DNI")]
    [SerializeField] private int semilla = 0;

    [Header("Destino")]
    [SerializeField] private Tilemap tilemapSuelo;
    [SerializeField] private TileBase tileSuelo;

    [Header("Zona a generar (coordenadas de celda)")]
    [SerializeField] private int xInicio = 10;
    [SerializeField] private int ancho = 20;
    [SerializeField] private int yFondo = -7;
    [SerializeField] private int yBase = -4;
    [SerializeField] private int alturaMaxima = 3;
    [SerializeField] private float escala = 0.25f;

    void Start()
    {
        Generar();
    }

    void Generar()
    {
        StringBuilder alturas = new StringBuilder();
        int alturaAnterior = 0; // el suelo del Territorio A termina a la altura 0

        for (int x = xInicio; x < xInicio + ancho; x++)
        {
            // 1. Limpiar la columna: SetTile con null borra el tile
            for (int y = yFondo; y <= yBase + alturaMaxima + 2; y++)
            {
                tilemapSuelo.SetTile(new Vector3Int(x, y, 0), null);
            }

            // 2. Calcular la altura de la columna con Perlin Noise
            float ruido = Mathf.PerlinNoise(x * escala + semilla * 0.731f, semilla * 0.237f + 0.5f);
            int altura = Mathf.Clamp(Mathf.FloorToInt(ruido * (alturaMaxima + 1)), 0, alturaMaxima);

            // 3. Limitar cada escalón a 1 celda para que el jugador siempre pueda subirlo
            altura = Mathf.Clamp(altura, alturaAnterior - 1, alturaAnterior + 1);
            alturaAnterior = altura;

            // 4. Rellenar la columna desde abajo hasta su altura
            for (int y = yFondo; y <= yBase + altura; y++)
            {
                tilemapSuelo.SetTile(new Vector3Int(x, y, 0), tileSuelo);
            }

            alturas.Append(altura).Append(' ');
        }

        Debug.Log("Territorio B generado con semilla " + semilla
            + ". Alturas por columna (x = " + xInicio + " a " + (xInicio + ancho - 1) + "): " + alturas);
    }
}