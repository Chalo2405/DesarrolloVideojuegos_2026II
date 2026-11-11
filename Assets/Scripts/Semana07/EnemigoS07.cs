using System.Collections.Generic;
using UnityEngine;

public enum EstadoEnemigoS07 { Patrullar, Perseguir, Atacar, Regresar }

public class EnemigoS07 : MonoBehaviour
{
    [Header("Personalización por DNI")]
    [Tooltip("Último dígito de tu DNI, tal cual")]
    [SerializeField] private int ultimoDigitoDNI = 0;
    [Tooltip("X = penúltimo dígito de tu DNI, tal cual. Y = 3")]
    [SerializeField] private Vector3Int celdaNido = new Vector3Int(0, 3, 0);
    [Header("Referencias")]
    [SerializeField] private Transform jugador;
    [SerializeField] private CuadriculaS07 cuadricula;
    [SerializeField] private BuscadorRutasS07 buscador;
    [SerializeField] private ParticleSystem estela;
    [SerializeField] private ParticleSystem impacto;
    [SerializeField] private LayerMask capaJugador;
    [SerializeField] private LayerMask capaSuelo;
    [Header("Movimiento")]
    [SerializeField] private Vector3Int celdaPatrulla = new Vector3Int(-8, 3, 0);
    [SerializeField] private float velocidadMaxima = 3.5f;
    [SerializeField] private float fuerzaMaxima = 30f;
    [SerializeField] private float radioLlegada = 1f;
    [SerializeField] private float intervaloRuta = 0.4f;
    [Header("Combate")]
    [SerializeField] private float radioAtaque = 1.2f;
    [SerializeField] private float cadenciaAtaque = 1f;
    [SerializeField] private float tiempoParaRendirse = 2f;

    private EstadoEnemigoS07 estado = EstadoEnemigoS07.Patrullar;
    private float radioDeteccion;
    private Color colorPersecucion;
    private Vector2 velocidad;
    private Vector3 puntoNido;
    private Vector3 puntoPatrulla;
    private Vector3 objetivoPatrulla;
    private List<Vector3> ruta = new List<Vector3>();
    private int indiceRuta;
    private bool hayRuta;
    private float proximaRuta;
    private float proximoAtaque;
    private float ultimaVezVisto = -100f;
    private int ataques;
    private bool avisoSinVision;

    // Semana 8: eventos que publica el enemigo
    public static event System.Action<int> OnEnemigoAtaco;
    public static event System.Action<EstadoEnemigoS07> OnEstadoCambiado;

    void Start()
    {
        bool par = ultimoDigitoDNI % 2 == 0;
        radioDeteccion = par ? 4.0f : 4.3f;
        colorPersecucion = par ? new Color(0f, 0.85f, 1f) : new Color(1f, 0.85f, 0f);
        puntoNido = cuadricula.CentroDeCelda(celdaNido);
        puntoPatrulla = cuadricula.CentroDeCelda(celdaPatrulla);
        objetivoPatrulla = puntoPatrulla;
        transform.position = puntoNido;
        Debug.Log("Enemigo: configuración " + (par ? "A (dígito par)" : "B (dígito impar)")
            + " | radio de detección = " + radioDeteccion + " | nido en la celda " + celdaNido);
    }

    void Update()
    {
        // PERCIBIR
        bool visto = JugadorVisible();
        if (visto) ultimaVezVisto = Time.time;
        float distancia = Vector2.Distance(transform.position, jugador.position);

        // DECIDIR Y ACTUAR
        switch (estado)
        {
            case EstadoEnemigoS07.Patrullar:
                if (visto)
                {
                    CambiarEstado(EstadoEnemigoS07.Perseguir, distancia);
                    break;
                }
                Mover(VelocidadLlegar(objetivoPatrulla));
                if (Vector2.Distance(transform.position, objetivoPatrulla) < 0.1f)
                    objetivoPatrulla = objetivoPatrulla == puntoPatrulla ? puntoNido : puntoPatrulla;
                break;

            case EstadoEnemigoS07.Perseguir:
                if (visto && distancia <= radioAtaque)
                {
                    CambiarEstado(EstadoEnemigoS07.Atacar, distancia);
                    break;
                }
                if (Time.time - ultimaVezVisto > tiempoParaRendirse)
                {
                    CambiarEstado(EstadoEnemigoS07.Regresar, distancia);
                    break;
                }
                SeguirRuta(jugador.position);
                break;

            case EstadoEnemigoS07.Atacar:
                velocidad = Vector2.zero;
                if (distancia > radioAtaque * 1.3f)
                {
                    CambiarEstado(EstadoEnemigoS07.Perseguir, distancia);
                    break;
                }
                if (Time.time >= proximoAtaque)
                {
                    proximoAtaque = Time.time + cadenciaAtaque;
                    ataques++;
                    impacto.Play();
                    Debug.Log("Ataque n.º " + ataques + " | t = " + Time.time.ToString("F2"));
                    OnEnemigoAtaco?.Invoke(ataques);
                }
                break;

            case EstadoEnemigoS07.Regresar:
                if (visto)
                {
                    CambiarEstado(EstadoEnemigoS07.Perseguir, distancia);
                    break;
                }
                SeguirRuta(puntoNido);
                if (Vector2.Distance(transform.position, puntoNido) < 0.15f)
                {
                    objetivoPatrulla = puntoPatrulla;
                    CambiarEstado(EstadoEnemigoS07.Patrullar, distancia);
                }
                break;
        }
    }

    bool JugadorVisible()
    {
        Collider2D detectado = Physics2D.OverlapCircle(transform.position, radioDeteccion, capaJugador);
        if (detectado == null)
        {
            avisoSinVision = false;
            return false;
        }
        Vector2 direccion = jugador.position - transform.position;
        RaycastHit2D obstaculo = Physics2D.Raycast(transform.position, direccion.normalized,
            direccion.magnitude, capaSuelo);
        if (obstaculo.collider != null)
        {
            if (!avisoSinVision)
            {
                avisoSinVision = true;
                Debug.Log("Percepción: jugador dentro del radio, pero sin línea de visión. Obstáculo en "
                    + obstaculo.point + " | estado = " + estado);
            }
            return false;
        }
        avisoSinVision = false;
        return true;
    }

    void CambiarEstado(EstadoEnemigoS07 nuevo, float distancia)
    {
        Debug.Log("FSM: " + estado + " -> " + nuevo + " | t = " + Time.time.ToString("F2")
            + " | distancia al jugador = " + distancia.ToString("F2"));
        estado = nuevo;
        ruta.Clear();
        indiceRuta = 0;
        hayRuta = false;
        proximaRuta = 0f;
        ActualizarEfectos();
        OnEstadoCambiado?.Invoke(estado);
    }

    void ActualizarEfectos()
    {
        ParticleSystem.MainModule principal = estela.main;
        if (estado == EstadoEnemigoS07.Perseguir)
        {
            principal.startColor = colorPersecucion;
            estela.Play();
        }
        else if (estado == EstadoEnemigoS07.Regresar)
        {
            principal.startColor = Color.gray;
            estela.Play();
        }
        else estela.Stop();
    }

    void SeguirRuta(Vector3 destino)
    {
        if (Time.time >= proximaRuta)
        {
            proximaRuta = Time.time + intervaloRuta;
            List<Vector3> nueva = buscador.BuscarRuta(transform.position, destino, HeuristicaS07.Manhattan);
            if (nueva != null)
            {
                ruta = nueva;
                indiceRuta = 0;
                hayRuta = true;
            }
        }
        if (!hayRuta || indiceRuta >= ruta.Count)
        {
            Mover(VelocidadLlegar(destino));
            return;
        }
        Vector3 punto = ruta[indiceRuta];
        bool esUltimo = indiceRuta == ruta.Count - 1;
        Mover(esUltimo ? VelocidadLlegar(punto) : VelocidadBuscar(punto));
        if (Vector2.Distance(transform.position, punto) < 0.2f) indiceRuta++;
    }

    // Seek y Arrive: rapidez máxima y frenado cerca de la meta, respectivamente.
    Vector2 VelocidadBuscar(Vector3 objetivo)
    {
        Vector2 hacia = objetivo - transform.position;
        return hacia.normalized * velocidadMaxima;
    }

    Vector2 VelocidadLlegar(Vector3 objetivo)
    {
        Vector2 hacia = objetivo - transform.position;
        float distancia = hacia.magnitude;
        float rapidez = distancia < radioLlegada ? velocidadMaxima * distancia / radioLlegada : velocidadMaxima;
        return hacia.normalized * rapidez;
    }

    void Mover(Vector2 velocidadDeseada)
    {
        velocidad = Vector2.MoveTowards(velocidad, velocidadDeseada, fuerzaMaxima * Time.deltaTime);
        transform.position += (Vector3)(velocidad * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion > 0 ? radioDeteccion : 4f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioAtaque);
        Gizmos.color = Color.green;
        for (int i = indiceRuta; i < ruta.Count - 1; i++) Gizmos.DrawLine(ruta[i], ruta[i + 1]);
    }
}
