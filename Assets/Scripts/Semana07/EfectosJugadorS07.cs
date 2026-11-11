using UnityEngine;

// Reutiliza la misma ráfaga de partículas en cada aterrizaje.
[RequireComponent(typeof(ControladorJugadorS06), typeof(BoxCollider2D))]
public class EfectosJugadorS07 : MonoBehaviour
{
    [SerializeField] private ParticleSystem polvoAterrizaje;
    [SerializeField] private LayerMask capaSuelo;
    private BoxCollider2D caja;
    private bool estabaEnSuelo = true;
    private int aterrizajes;

    void Awake() { caja = GetComponent<BoxCollider2D>(); }

    void Update()
    {
        bool enSuelo = EstaEnSuelo();
        if (Time.timeSinceLevelLoad < 0.5f)
        {
            estabaEnSuelo = enSuelo;
            return;
        }
        if (enSuelo && !estabaEnSuelo)
        {
            aterrizajes++;
            polvoAterrizaje.Play();
            Debug.Log("Aterrizaje n.º " + aterrizajes + " | t = " + Time.time.ToString("F2")
                + " | x = " + transform.position.x.ToString("F2"));
        }
        estabaEnSuelo = enSuelo;
    }

    bool EstaEnSuelo()
    {
        Bounds limites = caja.bounds;
        Vector2 centro = new Vector2(limites.center.x, limites.min.y - 0.05f);
        Vector2 dimensiones = new Vector2(limites.size.x * 0.9f, 0.1f);
        return Physics2D.OverlapBox(centro, dimensiones, 0f, capaSuelo) != null;
    }
}
