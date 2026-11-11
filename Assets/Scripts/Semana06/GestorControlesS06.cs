using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class GestorControlesS06 : MonoBehaviour
{
    private const string ClaveRemapeo = "RemapeoSemana06";
    private PlayerInput playerInput;
    private InputActionRebindingExtensions.RebindingOperation operacionRemapeo;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        // Cargar el remapeo guardado (si existe) antes de usar los controles
        if (PlayerPrefs.HasKey(ClaveRemapeo))
        {
            playerInput.actions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(ClaveRemapeo));
            Debug.Log("Remapeo cargado desde PlayerPrefs. Salto usa: " + TeclaDeSalto());
        }
        else
        {
            Debug.Log("Sin remapeo guardado. Salto usa: " + TeclaDeSalto());
        }
    }

    void Start()
    {
        // Esquema con el que Player Input empareja los dispositivos al iniciar
        Debug.Log("Esquema de control inicial: " + playerInput.currentControlScheme
            + " | dispositivos: " + string.Join(", ", playerInput.devices));
    }

    // Mensaje enviado por Player Input cuando cambia el esquema de control activo
    void OnControlsChanged(PlayerInput entrada)
    {
        Debug.Log("Esquema de control activo: " + entrada.currentControlScheme);
    }

    // Mensaje enviado por Player Input al realizar la acción Remapear (tecla R)
    void OnRemapear(InputValue valor)
    {
        if (operacionRemapeo != null) return;

        InputAction salto = playerInput.actions["Salto"];
        int indice = salto.GetBindingIndex(group: "Teclado");
        if (indice < 0)
        {
            Debug.LogWarning("La acción Salto no tiene un binding en el esquema Teclado");
            return;
        }

        Debug.Log("Remapeo iniciado: presiona la nueva tecla para Salto (Escape cancela)");
        salto.Disable();
        operacionRemapeo = salto.PerformInteractiveRebinding(indice)
            .WithControlsHavingToMatchPath("<Keyboard>")
            .WithControlsExcluding("<Mouse>")
            .WithControlsExcluding("<Keyboard>/r")
            .WithControlsExcluding("<Keyboard>/t")
            .WithCancelingThrough("<Keyboard>/escape")
            .WithMatchingEventsBeingSuppressed()
            .OnComplete(operacion => FinalizarRemapeo(salto, true))
            .OnCancel(operacion => FinalizarRemapeo(salto, false))
            .Start();
    }

    void FinalizarRemapeo(InputAction salto, bool completado)
    {
        operacionRemapeo.Dispose();
        operacionRemapeo = null;
        salto.Enable();

        if (!completado)
        {
            Debug.Log("Remapeo cancelado. Salto sigue usando: " + TeclaDeSalto());
            return;
        }

        string json = playerInput.actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(ClaveRemapeo, json);
        PlayerPrefs.Save();
        Debug.Log("Remapeo guardado. Salto ahora usa: " + TeclaDeSalto());
        Debug.Log("JSON guardado: " + json);
    }

    // Mensaje enviado por Player Input al realizar la acción Restablecer (tecla T)
    void OnRestablecer(InputValue valor)
    {
        playerInput.actions.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey(ClaveRemapeo);
        Debug.Log("Controles restablecidos. Salto usa: " + TeclaDeSalto());
    }

    string TeclaDeSalto()
    {
        InputAction salto = playerInput.actions["Salto"];
        int indice = salto.GetBindingIndex(group: "Teclado");
        return indice < 0 ? "(sin binding de teclado)" : salto.GetBindingDisplayString(indice);
    }
}