using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Gestor global de la partida (patrón Singleton). Cuenta cristales y puntos, y publica eventos.
public class GestorPartidaS08 : MonoBehaviour
{
    public static GestorPartidaS08 Instancia { get; private set; }

    public static event Action<int, int, int> OnMarcadorCambiado; // cristales, total, puntos
    public static event Action<string, float> OnAviso;           // texto, segundos (0 = fijo)
    public static event Action<bool, int> OnPartidaTerminada;    // victoria, puntos finales

    [Header("Referencias")]
    [SerializeField] private Transform contenedorCristales;

    private static int reinicios; // static: conserva su valor al recargar la escena
    private int total;
    private int cristales;
    private int puntos;
    private bool terminada;

    public bool EnPausa { get; private set; }

    public bool TodosRecogidos
    {
        get { return total > 0 && cristales >= total; }
    }

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Debug.LogWarning("S08 | Singleton: ya existe un gestor en el objeto '" + Instancia.name
                + "'. Se destruye el duplicado '" + name + "'.");
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        Time.timeScale = 1f;
    }

    void OnEnable()
    {
        if (Instancia != this) return;
        SaludJugadorS08.OnJugadorDerrotado += AlDerrotarJugador;
    }

    void OnDisable()
    {
        SaludJugadorS08.OnJugadorDerrotado -= AlDerrotarJugador;
    }

    void OnDestroy()
    {
        if (Instancia == this) Instancia = null;
    }

    void Start()
    {
        if (Instancia != this) return;
        total = contenedorCristales != null ? contenedorCristales.childCount : 0;
        // (Paso 19) Aquí buscarás al jugador por su etiqueta
        Debug.Log("S08 | Partida iniciada | cristales en el nivel = " + total + " | reinicios = " + reinicios);
        OnAviso?.Invoke("Reúne los " + total + " cristales", 3f);
        OnMarcadorCambiado?.Invoke(cristales, total, puntos);
    }

    void Update()
    {
        if (Instancia != this) return;
        Keyboard teclado = Keyboard.current;
        if (teclado == null) return;

        if (teclado.enterKey.wasPressedThisFrame)
        {
            Reiniciar();
            return;
        }

        // (Paso 14) Aquí leerás la tecla P para la pausa
    }

    // Método público: el UnityEvent de cada cristal lo llamará desde el Inspector
    public void RegistrarCristal(int valor)
    {
        if (terminada) return;
        cristales++;
        puntos += valor;
        Debug.Log("S08 | Cristal " + cristales + " de " + total + " | puntos = " + puntos
            + " | t = " + Time.time.ToString("F2"));
        OnMarcadorCambiado?.Invoke(cristales, total, puntos);
        if (TodosRecogidos) OnAviso?.Invoke("Cristales completos: ve al portal del Territorio B", 3f);
    }

    void AlDerrotarJugador()
    {
        if (terminada) return;
        terminada = true;
        Debug.Log("S08 | Gestor: recibí OnJugadorDerrotado");
        // (Paso 19) Aquí llamarás a Finalizar(false)
        AnunciarFinal(false);
    }

    void AnunciarFinal(bool victoria)
    {
        Debug.Log("S08 | Partida terminada: " + (victoria ? "VICTORIA" : "DERROTA") + " | puntos = " + puntos
            + " | t = " + Time.time.ToString("F2"));
        OnPartidaTerminada?.Invoke(victoria, puntos);
    }

    void Reiniciar()
    {
        reinicios++;
        Time.timeScale = 1f;
        Debug.Log("S08 | Reinicio n.º " + reinicios + ": se recarga la escena");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
