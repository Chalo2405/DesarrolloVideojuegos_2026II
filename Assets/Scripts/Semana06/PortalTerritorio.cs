using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalTerritorio : MonoBehaviour
{
    [SerializeField] private string escenaDestino = "Semana06_Habitacion";

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (!otro.CompareTag("Player")) return;

        Debug.Log("Portal: cargando la escena " + escenaDestino);
        SceneManager.LoadScene(escenaDestino, LoadSceneMode.Single);
    }
}