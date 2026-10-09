using UnityEngine;

public class SimuladorAzarBalance : MonoBehaviour
{
    [Header("Personalizacion por DNI (ver Paso 9 del documento)")]
    [Tooltip("Coloca aqui los ultimos 3 digitos de tu DNI, tal cual, sin modificarlos.")]
    public int cantidadTiradas = 100;

    [Tooltip("Coloca aqui el ultimo digito de tu DNI. Par = Tabla A, impar = Tabla B.")]
    public int ultimoDigitoDNI = 0;

    [Header("Parametros de balance (no modificar)")]
    public float dificultadInicial = 5f;
    public float pasoAjusteDificultad = 0.5f;
    public float umbralExito = 0.55f;

    private int comunes = 0;
    private int raros = 0;
    private int epicos = 0;
    private int legendarios = 0;

    void Start()
    {
        bool esTablaA = (ultimoDigitoDNI % 2 == 0);
        string nombreTabla = esTablaA ? "Tabla A" : "Tabla B";

        float pComun, pRaro, pEpico, pLegendario;

        if (esTablaA)
        {
            pComun = 0.60f;
            pRaro = 0.25f;
            pEpico = 0.12f;
            pLegendario = 0.03f;
        }
        else
        {
            pComun = 0.50f;
            pRaro = 0.30f;
            pEpico = 0.15f;
            pLegendario = 0.05f;
        }

        int exitos = 0;

        for (int i = 0; i < cantidadTiradas; i++)
        {
            float valorRareza = Random.Range(0f, 1f);

            if (valorRareza < pLegendario)
            {
                legendarios++;
            }
            else if (valorRareza < pLegendario + pEpico)
            {
                epicos++;
            }
            else if (valorRareza < pLegendario + pEpico + pRaro)
            {
                raros++;
            }
            else
            {
                comunes++;
            }

            bool resultadoFavorable = Random.Range(0f, 1f) < umbralExito;

            if (resultadoFavorable)
            {
                exitos++;
            }
        }

        float tasaExito = (float)exitos / cantidadTiradas;
        float dificultadFinal = dificultadInicial;

        if (tasaExito > umbralExito)
        {
            dificultadFinal += pasoAjusteDificultad;
        }
        else
        {
            dificultadFinal -= pasoAjusteDificultad;
        }

        Debug.Log("=== SIMULADOR DE AZAR Y BALANCE - SEMANA 03 ===");
        Debug.Log("Tabla de probabilidades usada: " + nombreTabla + " (ultimo digito de DNI = " + ultimoDigitoDNI + ")");
        Debug.Log("Cantidad de tiradas (ultimos 3 digitos del DNI): " + cantidadTiradas);
        Debug.Log("Comunes: " + comunes + " (" + (100f * comunes / cantidadTiradas).ToString("F2") + "%)");
        Debug.Log("Raros: " + raros + " (" + (100f * raros / cantidadTiradas).ToString("F2") + "%)");
        Debug.Log("Epicos: " + epicos + " (" + (100f * epicos / cantidadTiradas).ToString("F2") + "%)");
        Debug.Log("Legendarios: " + legendarios + " (" + (100f * legendarios / cantidadTiradas).ToString("F2") + "%)");
        Debug.Log("Tasa de exito simulada: " + (100f * tasaExito).ToString("F2") + "%");
        Debug.Log("Dificultad inicial: " + dificultadInicial + " -> Dificultad ajustada (DDA): " + dificultadFinal);
    }
}