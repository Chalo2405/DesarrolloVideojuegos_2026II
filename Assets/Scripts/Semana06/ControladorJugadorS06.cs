using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PlayerInput))]
public class ControladorJugadorS06 : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidadBase = 5f;
    [SerializeField] private float fuerzaSalto = 14f;
    [SerializeField] private float gravedadBase = 3f;
    [SerializeField] private LayerMask capaSuelo;

    [Header("Aparición en una celda del Tilemap")]
    [SerializeField] private Tilemap tilemapAparicion;
    [SerializeField] private Vector3Int celdaAparicion = new Vector3Int(-7, -3, 0);
    [SerializeField] private float alturaMinima = -15f;

    private Rigidbody2D rb;
    private BoxCollider2D caja;
    private InputAction accionMovimiento;
    private InputAction accionSalto;
    private float entradaHorizontal;
    private float multiplicadorVelocidad = 1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        caja = GetComponent<BoxCollider2D>();
        PlayerInput playerInput = GetComponent<PlayerInput>();
        accionMovimiento = playerInput.actions["Movimiento"];
        accionSalto = playerInput.actions["Salto"];
        rb.gravityScale = gravedadBase;
    }

    void OnEnable()
    {
        accionSalto.started += AlIniciarSalto;
        accionSalto.performed += AlRealizarSalto;
        accionSalto.canceled += AlCancelarSalto;
    }

    void OnDisable()
    {
        accionSalto.started -= AlIniciarSalto;
        accionSalto.performed -= AlRealizarSalto;
        accionSalto.canceled -= AlCancelarSalto;
    }

    void Start()
    {
        Reaparecer();
    }

    void Update()
    {
        // Polling: se consulta el valor de la acción en cada fotograma
        entradaHorizontal = accionMovimiento.ReadValue<Vector2>().x;

        if (transform.position.y < alturaMinima)
        {
            Debug.Log("El jugador cayó fuera del nivel: reaparece");
            Reaparecer();
        }
    }

    void FixedUpdate()
    {
        float velocidadX = entradaHorizontal * velocidadBase * multiplicadorVelocidad;
        rb.linearVelocity = new Vector2(velocidadX, rb.linearVelocity.y);
    }

    // Callbacks: se ejecutan solo cuando ocurre cada fase de la acción Salto
    void AlIniciarSalto(InputAction.CallbackContext contexto)
    {
        Debug.Log("Salto started  t = " + Time.time.ToString("F2"));
    }

    void AlRealizarSalto(InputAction.CallbackContext contexto)
    {
        Debug.Log("Salto performed t = " + Time.time.ToString("F2") + " | en suelo: " + EstaEnSuelo());
        if (EstaEnSuelo())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }
    }

    void AlCancelarSalto(InputAction.CallbackContext contexto)
    {
        Debug.Log("Salto canceled  t = " + Time.time.ToString("F2"));
        // Salto de altura variable: si se suelta el botón mientras sube, se corta el salto
        if (rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    bool EstaEnSuelo()
    {
        Bounds limites = caja.bounds;
        Vector2 centro = new Vector2(limites.center.x, limites.min.y - 0.05f);
        Vector2 dimensiones = new Vector2(limites.size.x * 0.9f, 0.1f);
        return Physics2D.OverlapBox(centro, dimensiones, 0f, capaSuelo) != null;
    }

    void Reaparecer()
    {
        if (tilemapAparicion == null)
        {
            Debug.LogWarning("Asigna el campo Tilemap Aparicion en el Inspector");
            return;
        }

        Vector3 esquina = tilemapAparicion.CellToWorld(celdaAparicion);
        Vector3 centro = tilemapAparicion.GetCellCenterWorld(celdaAparicion);
        transform.position = centro;
        rb.position = centro;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Celda de aparición = " + celdaAparicion + " | CellWorld = " + esquina
            + " | GetCellCenterWorld = " + centro);
    }

    public void EstablecerModificadores(float nuevoMultiplicadorVelocidad , float NuevaEscalaGravedad)
    {
        multiplicadorVelocidad = nuevoMultiplicadorVelocidad;
        rb.gravityScale = NuevaEscalaGravedad;
    }
}