using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PortalVictoriaS08 : MonoBehaviour
{
    [Header("Personalización por DNI")]
    [Tooltip("Últimos 3 dígitos de tu DNI, tal cual")]
    [SerializeField] private int bonificacionDNI = 0;

    [Header("Referencias")]
    [SerializeField] private ParticleSystem particulas;

    private SpriteRenderer sprite;
    private Coroutine espera;
    private bool encendido;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        GestorPartidaS08.ConsultarBonificacion += EntregarBonificacion;
        SaludJugadorS08.OnJugadorDerrotado += Apagar;
    }

    void OnDisable()
    {
        GestorPartidaS08.ConsultarBonificacion -= EntregarBonificacion;
        SaludJugadorS08.OnJugadorDerrotado -= Apagar;
    }

    void Start()
    {
        sprite.color = Color.gray;
        particulas.Stop();
        espera = StartCoroutine(EsperarCristales());
    }

    IEnumerator EsperarCristales()
    {
        float inicio = Time.time;
        Debug.Log("S08 | Portal apagado: espera a que se reúnan los cristales");

        // Espera hasta que la condición sea verdadera
        yield return new WaitUntil(() => GestorPartidaS08.Instancia != null
            && GestorPartidaS08.Instancia.TodosRecogidos);

        encendido = true;
        sprite.color = Color.cyan;
        particulas.Play();
        Debug.Log("S08 | Portal encendido tras esperar " + (Time.time - inicio).ToString("F2") + " s");
    }

    // Su firma coincide con Func<int>: no recibe datos y devuelve un entero
    int EntregarBonificacion()
    {
        Debug.Log("S08 | Portal: respondo la consulta con la bonificación " + bonificacionDNI);
        return bonificacionDNI;
    }

    void Apagar()
    {
        if (espera != null) StopCoroutine(espera);
        encendido = false;
        sprite.color = Color.gray;
        particulas.Stop();
        Debug.Log("S08 | Portal: recibí OnJugadorDerrotado y me apago");
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (!otro.CompareTag("Player")) return;

        if (!encendido)
        {
            Debug.Log("S08 | Portal: todavía apagado, faltan cristales");
            return;
        }

        Debug.Log("S08 | Portal cruzado | t = " + Time.time.ToString("F2"));
        GestorPartidaS08.Instancia.DeclararVictoria();
    }
}
