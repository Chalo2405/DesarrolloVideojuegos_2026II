using TMPro;
using UnityEngine;

// Marcador: solo escucha eventos. No tiene referencias al jugador ni al gestor.
public class MarcadorS08 : MonoBehaviour
{
    private static int creados; // marcadores creados desde que empezó el modo Play

    private TMP_Text texto;
    private int numero;
    private int vidas;
    private int vidasIniciales;
    private int cristales;
    private int total;
    private int puntos;

    void Awake()
    {
        texto = GetComponent<TMP_Text>();
        creados++;
        numero = creados;
    }

    void OnEnable()
    {
        SaludJugadorS08.OnSaludCambiada += AlCambiarSalud;
        SaludJugadorS08.OnJugadorDerrotado += AlDerrotarJugador;
        GestorPartidaS08.OnMarcadorCambiado += AlCambiarMarcador;
    }

    void OnDisable()
    {
        SaludJugadorS08.OnSaludCambiada -= AlCambiarSalud;
        SaludJugadorS08.OnJugadorDerrotado -= AlDerrotarJugador;
        GestorPartidaS08.OnMarcadorCambiado -= AlCambiarMarcador;
    }

    // Su firma coincide con el delegado SaludCambiada
    void AlCambiarSalud(int vidasActuales, int iniciales)
    {
        vidas = vidasActuales;
        vidasIniciales = iniciales;
        Debug.Log("S08 | Marcador n.º " + numero + " recibió OnSaludCambiada: " + vidas + " de " + vidasIniciales);
        Redibujar();
    }

    // Su firma coincide con Action<int, int, int>
    void AlCambiarMarcador(int recogidos, int enElNivel, int puntosActuales)
    {
        cristales = recogidos;
        total = enElNivel;
        puntos = puntosActuales;
        Redibujar();
    }

    // Su firma coincide con Action
    void AlDerrotarJugador()
    {
        Debug.Log("S08 | Marcador n.º " + numero + " recibió OnJugadorDerrotado");
        texto.color = new Color(1f, 0.4f, 0.4f);
    }

    void Redibujar()
    {
        if (texto == null) return; // el texto de este marcador ya fue destruido
        texto.text = "Vidas: " + vidas + "/" + vidasIniciales + "    Cristales: " + cristales + "/" + total
            + "    Puntos: " + puntos;
    }
}
