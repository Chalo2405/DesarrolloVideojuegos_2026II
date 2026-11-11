using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CamaraConLimites : MonoBehaviour
{
    [SerializeField] private Transform objetivo;
    [SerializeField] private float xMin = -15f, xMax = 15f, yMin = -7f, yMax = 7f;
    private Camera camara;

    void Start()
    {
        camara = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (objetivo == null) return;

        float mitadAlto = camara.orthographicSize;
        float mitadAncho = mitadAlto * camara.aspect;

        float x = Mathf.Clamp(objetivo.position.x, xMin + mitadAncho, xMax - mitadAncho);
        float y = Mathf.Clamp(objetivo.position.y, yMin + mitadAlto, yMax - mitadAlto);

        transform.position = new Vector3(x, y, transform.position.z);
    }
}