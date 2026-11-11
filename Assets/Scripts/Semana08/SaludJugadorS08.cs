using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Salud del jugador. Publica eventos: no conoce al marcador ni al gestor de la partida.
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PlayerInput))]
public class SaludJugadorS08 : MonoBehaviour
{
    // Evento con delegado propio: el delegado define la firma que deben tener los receptores
    public delegate void SaludCambiada(int vidas, int vidasIniciales);
    public static event SaludCambiada OnSaludCambiada;

    // Evento compacto con Action: una notificación sin datos
    public static event Action OnJugadorDerrotado;

    [Header("Personalización por DNI")]
    [Tooltip("Último dígito de tu DNI, tal cual")]
    [SerializeField] private int ultimoDigitoDNI = 0;

    private SpriteRenderer sprite;
    private PlayerInput entrada;
    private int vidasIniciales;
    private int vidas;
    private int parpadeos;
    private bool derrotado;

    void Awake()
    {
        // Componentes del mismo GameObject: GetComponent una sola vez en Awake
        sprite = GetComponent<SpriteRenderer>();
        entrada = GetComponent<PlayerInput>();

        bool par = ultimoDigitoDNI % 2 == 0;
        vidasIniciales = par ? 3 : 4;
        parpadeos = par ? 10 : 6;
        vidas = vidasIniciales;
    }

    void OnEnable()
    {
        EnemigoS07.OnEnemigoAtaco += RecibirAtaque;
    }

    void OnDisable()
    {
        EnemigoS07.OnEnemigoAtaco -= RecibirAtaque;
    }

    void Start()
    {
        Debug.Log("S08 | Salud: configuración " + (ultimoDigitoDNI % 2 == 0 ? "A (dígito par)" : "B (dígito impar)")
            + " | vidas iniciales = " + vidasIniciales + " | parpadeos de invulnerabilidad = " + parpadeos);
        OnSaludCambiada?.Invoke(vidas, vidasIniciales);
    }

    void RecibirAtaque(int numeroAtaque)
    {
        if (derrotado) return;

        vidas--;
        Debug.Log("S08 | Daño por el ataque n.º " + numeroAtaque + " | vidas = " + vidas + " de " + vidasIniciales
            + " | t = " + Time.time.ToString("F2"));
        OnSaludCambiada?.Invoke(vidas, vidasIniciales);

        if (vidas <= 0)
        {
            derrotado = true;
            sprite.enabled = false;
            Debug.Log("S08 | Derrota: se publica OnJugadorDerrotado | t = " + Time.time.ToString("F2"));
            OnJugadorDerrotado?.Invoke();
        }
    }

    // Receptor del mensaje que el gestor enviará con SendMessage en el Paso 19
    void AlTerminarPartida(bool victoria)
    {
        entrada.DeactivateInput();
        Debug.Log("S08 | Jugador: recibí el mensaje AlTerminarPartida (victoria = " + victoria
            + ") y desactivé la entrada");
    }
}
