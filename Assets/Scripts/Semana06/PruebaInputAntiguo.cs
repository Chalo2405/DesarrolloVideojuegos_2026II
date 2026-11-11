using UnityEngine;

public class PruebaInputAntiguo : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            Debug.Log("Espacio leido con el sistema antiguo");
        }
    }
}