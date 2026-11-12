using System;
using System.Collections;
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
    public static event Func<int> ConsultarBonificacion;        // consulta: devuelve un valor

    [Header("Referencias")]
    [SerializeField] private Transform contenedorCristales;

    private static int reinicios; // static: conserva su valor al recargar la escena
    private int total;
    private int cristales;
    private int puntos;
    private bool terminada;
    private bool reanudando;

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

        if (teclado.enterKey.wasPressedThisFrame || teclado.numpadEnterKey.wasPressedThisFrame)
        {
            Reiniciar();
            return;
        }

        if (teclado.pKey.wasPressedThisFrame && !terminada && !reanudando)
        {
            AlternarPausa();
        }
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

    void AlternarPausa()
    {
        if (!EnPausa)
        {
            EnPausa = true;
            Time.timeScale = 0f;
            Debug.Log("S08 | Pausa activada | Time.time = " + Time.time.ToString("F2")
                + " | tiempo real = " + Time.realtimeSinceStartup.ToString("F2"));
            OnAviso?.Invoke("PAUSA: presiona P para continuar", 0f);
        }
        else
        {
            StartCoroutine(Reanudar());
        }
    }

    IEnumerator Reanudar()
    {
        reanudando = true;
        for (int cuenta = 3; cuenta >= 1; cuenta--)
        {
            OnAviso?.Invoke("Reanudando en " + cuenta, 0f);
            Debug.Log("S08 | Reanudando en " + cuenta + " | Time.time = " + Time.time.ToString("F2")
                + " | tiempo real = " + Time.realtimeSinceStartup.ToString("F2"));
            // Tiempo real: no depende de Time.timeScale, que ahora vale 0
            yield return new WaitForSecondsRealtime(1f);
        }

        Time.timeScale = 1f;
        EnPausa = false;
        reanudando = false;
        OnAviso?.Invoke("", 0f);
        Debug.Log("S08 | Juego reanudado | Time.time = " + Time.time.ToString("F2")
            + " | tiempo real = " + Time.realtimeSinceStartup.ToString("F2"));
    }

    // Método público: el portal lo llamará cuando el jugador lo cruce estando encendido
    public void DeclararVictoria()
    {
        if (terminada) return;
        terminada = true;
        // (Paso 19) Aquí llamarás a Finalizar(true)
        StartCoroutine(SecuenciaVictoria());
    }

    IEnumerator SecuenciaVictoria()
    {
        // Func: el gestor pregunta y espera una respuesta. Si nadie responde, usa 0
        int bonificacion = ConsultarBonificacion?.Invoke() ?? 0;
        Debug.Log("S08 | Victoria: bonificación consultada = " + bonificacion + " | puntos antes = " + puntos);
        yield return StartCoroutine(ContarBonificacion(bonificacion));
        AnunciarFinal(true);
    }

    IEnumerator ContarBonificacion(int bonificacion)
    {
        if (bonificacion <= 0)
        {
            Debug.Log("S08 | Sin bonificación que sumar");
            yield break;
        }

        int sumado = 0;
        int fotogramas = 0;
        while (sumado < bonificacion)
        {
            int paso = Mathf.Min(7, bonificacion - sumado);
            sumado += paso;
            puntos += paso;
            fotogramas++;
            OnMarcadorCambiado?.Invoke(cristales, total, puntos);
            yield return null; // espera un fotograma
        }
        Debug.Log("S08 | Bonificación de " + bonificacion + " puntos sumada en " + fotogramas + " fotogramas");
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
