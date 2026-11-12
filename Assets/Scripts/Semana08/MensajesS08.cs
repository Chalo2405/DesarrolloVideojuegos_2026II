using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// Muestra mensajes en pantalla. Solo escucha eventos y usa corrutinas para borrarlos a tiempo.
public class MensajesS08 : MonoBehaviour
{
    private TMP_Text texto;
    private Coroutine corrutinaActual;
    private EstadoEnemigoS07 estadoEnemigo = EstadoEnemigoS07.Patrullar;
    private int lanzadas;
    private int pruebas;

    void Awake()
    {
        texto = GetComponent<TMP_Text>();
        texto.text = "";
    }

    void OnEnable()
    {
        GestorPartidaS08.OnAviso += AlRecibirAviso;
        GestorPartidaS08.OnPartidaTerminada += AlAnunciarFinal;
        EnemigoS07.OnEstadoCambiado += AlCambiarEstadoEnemigo;
    }

    void OnDisable()
    {
        GestorPartidaS08.OnAviso -= AlRecibirAviso;
        GestorPartidaS08.OnPartidaTerminada -= AlAnunciarFinal;
        EnemigoS07.OnEstadoCambiado -= AlCambiarEstadoEnemigo;
    }

    void Update()
    {
        // Tecla de prueba M: lanza un mensaje temporal de 2 segundos
        if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
        {
            pruebas++;
            Mostrar("Mensaje de prueba " + pruebas, 2f);
        }
    }

    void AlRecibirAviso(string contenido, float segundos)
    {
        if (segundos <= 0f) MostrarFijo(contenido);
        else Mostrar(contenido, segundos);
    }

    void AlAnunciarFinal(bool victoria, int puntos)
    {
        MostrarFijo((victoria ? "VICTORIA" : "DERROTA") + ": " + puntos + " puntos. Enter para reiniciar");
    }

    void AlCambiarEstadoEnemigo(EstadoEnemigoS07 estado)
    {
        estadoEnemigo = estado;
        if (estado == EstadoEnemigoS07.Perseguir)
        {
            if (corrutinaActual != null) StopCoroutine(corrutinaActual);
            lanzadas++;
            corrutinaActual = StartCoroutine(AlertaPersecucion(lanzadas));
        }
    }

    void Mostrar(string contenido, float segundos)
    {
        if (corrutinaActual != null) StopCoroutine(corrutinaActual);
        lanzadas++;
        corrutinaActual = StartCoroutine(MostrarTemporal(contenido, segundos, lanzadas));
    }

    void MostrarFijo(string contenido)
    {
        if (corrutinaActual != null) StopCoroutine(corrutinaActual);
        texto.text = contenido;
    }

    IEnumerator MostrarTemporal(string contenido, float segundos, int numero)
    {
        texto.text = contenido;
        Debug.Log("S08 | Mensaje n.º " + numero + " mostrado: \"" + contenido + "\" | t = "
            + Time.time.ToString("F2"));
        yield return new WaitForSeconds(segundos);
        texto.text = "";
        Debug.Log("S08 | Texto borrado por la corrutina n.º " + numero + " | t = " + Time.time.ToString("F2"));
    }

    IEnumerator AlertaPersecucion(int numero)
    {
        texto.text = "¡El enemigo te persigue!";
        Debug.Log("S08 | Mensaje n.º " + numero + " mostrado: alerta de persecución | t = "
            + Time.time.ToString("F2"));
        // Espera mientras la condición sea verdadera
        yield return new WaitWhile(() => estadoEnemigo == EstadoEnemigoS07.Perseguir);
        texto.text = "";
        Debug.Log("S08 | Texto borrado por la corrutina n.º " + numero + " (terminó la persecución) | t = "
            + Time.time.ToString("F2"));
    }
}
