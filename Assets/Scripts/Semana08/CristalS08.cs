using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CircleCollider2D))]
public class CristalS08 : MonoBehaviour
{
    // UnityEvent: es visible en el Inspector y sus receptores se conectan sin código
    public UnityEvent OnCristalRecogido;

    private bool recogido;

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (recogido || !otro.CompareTag("Player")) return;

        recogido = true;
        Debug.Log("S08 | " + name + " recogido: su UnityEvent tiene "
            + OnCristalRecogido.GetPersistentEventCount() + " receptores conectados en el Inspector");
        OnCristalRecogido?.Invoke();
    }
}
