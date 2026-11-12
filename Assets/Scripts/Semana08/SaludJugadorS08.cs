using System;
using System.Collections;
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
    private bool invulnerable;

    [Header("Referencias")]
    [SerializeField] private ParticleSystem particulasDerrota;

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

        if (invulnerable)
        {
            Debug.Log("S08 | Ataque n.º " + numeroAtaque + " ignorado: el jugador es invulnerable | t = "
                + Time.time.ToString("F2"));
            return;
        }

        vidas--;
        Debug.Log("S08 | Daño por el ataque n.º " + numeroAtaque + " | vidas = " + vidas + " de " + vidasIniciales
            + " | t = " + Time.time.ToString("F2"));
        OnSaludCambiada?.Invoke(vidas, vidasIniciales);

        if (vidas <= 0) StartCoroutine(SecuenciaDerrota());
        else StartCoroutine(Invulnerabilidad());
    }

    IEnumerator Invulnerabilidad()
    {
        invulnerable = true;
        float inicio = Time.time;

        // Corrutina anidada: esta corrutina espera a que la otra termine por completo
        yield return StartCoroutine(Parpadeo(parpadeos, 0.125f));

        invulnerable = false;
        Debug.Log("S08 | Invulnerabilidad terminada: " + parpadeos + " parpadeos en "
            + (Time.time - inicio).ToString("F2") + " s");
    }

    IEnumerator Parpadeo(int veces, float intervalo)
    {
        for (int i = 0; i < veces; i++)
        {
            sprite.enabled = false;
            yield return new WaitForSeconds(intervalo);
            sprite.enabled = true;
            yield return new WaitForSeconds(intervalo);
        }
    }

    IEnumerator SecuenciaDerrota()
    {
        if (derrotado) yield break;

        derrotado = true;
        Debug.Log("S08 | Derrota (1): inicia la secuencia | t = " + Time.time.ToString("F2"));
        particulasDerrota.Play();
        yield return StartCoroutine(Parpadeo(6, 0.125f));
        sprite.enabled = false;
        Debug.Log("S08 | Derrota (2): se publica OnJugadorDerrotado | t = " + Time.time.ToString("F2"));
        OnJugadorDerrotado?.Invoke();
    }

    // Receptor del mensaje que el gestor enviará con SendMessage en el Paso 19
    void AlTerminarPartida(bool victoria)
    {
        entrada.DeactivateInput();
        Debug.Log("S08 | Jugador: recibí el mensaje AlTerminarPartida (victoria = " + victoria
            + ") y desactivé la entrada");
    }
}
