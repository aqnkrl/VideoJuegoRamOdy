using UnityEngine;

public class GestorNivel : MonoBehaviour
{
    public static GestorNivel Instancia;

    [Header("Progreso del Nivel")]
    public int totalTareas = 4; // 2 para Ody (Comida y Agua) y 2 para Ramón (Comida y Agua)
    private int tareasCompletadas = 0;
    // Linea agregada
    public bool nivelCompletado = false;

    [Header("Panel del Regalo")]
    public GameObject panelRegalo; // Arrastra aquí tu PanelRegalo de la Jerarquía

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AvanzarProgreso()
    {
        tareasCompletadas++;
        Debug.Log("Progreso actual: " + tareasCompletadas + " / " + totalTareas);

        if (tareasCompletadas >= totalTareas)
        {
            CompletarNivel();
        }
    }

    void CompletarNivel()
    {
        // Linea agregada
        nivelCompletado = true;
        // Activamos el panel del regalo que creaste en el Canvas
        if (panelRegalo != null)
        {
            panelRegalo.SetActive(true);
        }

        Debug.Log("¡Todas las tareas listas! El regalo ha aparecido.");
    }
}