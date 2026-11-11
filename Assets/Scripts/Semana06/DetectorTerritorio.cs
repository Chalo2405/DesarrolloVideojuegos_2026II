using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(ControladorJugadorS06))]
public class DetectorTerritorio : MonoBehaviour
{
    [Header("Personalización por DNI")]
    [Tooltip("Escribe aquí el último dígito de tu DNI, tal cual")]
    [SerializeField] private int ultimoDigitoDNI = 0;

    [Header("Territorios (dejar el Tilemap vacío en la habitación)")]
    [SerializeField] private Tilemap tilemapTerritorios;
    [SerializeField] private TileBase tileTerritorioA;
    [SerializeField] private TileBase tileTerritorioB;

    private ControladorJugadorS06 controlador;
    private string territorioActual = "";

    void Awake()
    {
        controlador = GetComponent<ControladorJugadorS06>();
    }

    void Update()
    {
        string detectado;
        Vector3Int celda = Vector3Int.zero;

        if (tilemapTerritorios == null)
        {
            detectado = "Habitación";
        }
        else
        {
            celda = tilemapTerritorios.WorldToCell(transform.position);
            TileBase tile = tilemapTerritorios.GetTile(celda);
            if (tile == tileTerritorioA) detectado = "Territorio A";
            else if (tile == tileTerritorioB) detectado = "Territorio B";
            else detectado = "Fuera de los territorios";
        }

        if (detectado == territorioActual) return;

        territorioActual = detectado;
        string regla = AplicarRegla();
        Debug.Log("Entró a: " + territorioActual + " | celda " + celda + " | " + regla);
    }

    string AplicarRegla()
    {
        if (territorioActual == "Territorio B")
        {
            if (ultimoDigitoDNI % 2 == 0)
            {
                controlador.EstablecerModificadores(1f, 1.5f);
                return "Regla B (dígito par): gravedad reducida a 1.5";
            }
            controlador.EstablecerModificadores(1.8f, 3f);
            return "Regla B (dígito impar): velocidad x 1.8";
        }

        controlador.EstablecerModificadores(1f, 3f);
        return "Reglas normales: velocidad x 1, gravedad 3";
    }
}