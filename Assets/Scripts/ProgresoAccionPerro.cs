using UnityEngine;
using UnityEngine.UI;

// Barra de una acción (cepillado, juego, etc.) de un perro.
// Empieza vacía, solo sube cuando el jugador hace la acción, nunca baja
// y el valor se conserva al cambiar de escena o reiniciar el juego.
public class ProgresoAccionPerro : MonoBehaviour
{
    [Header("Identificación")]
    public string nombreBase = "Ody";        // Ody o Ramon (igual en todas las escenas)
    public string nombreAccion = "cepillado"; // cambia según la barra: "cepillado", "juego"...

    [Header("Barra")]
    public Slider barra;                      // arrastra BarraCepilladoOdy aquí
    public bool ocultarHandle = true;

    [Header("Valores (0 = vacío, 1 = lleno)")]
    [Range(0f, 1f)] public float valor = 0f;
    public float cantidadPorAccion = 0.1f;    // cuánto sube cada vez que cepillas

    [Header("Persistencia")]
    public bool persistir = true;
    public bool empezarVacioSiNoHayDatos = true;

    public bool EstaLleno { get { return valor >= 1f; } }

    string Clave { get { return "Progreso_" + nombreBase + "_" + nombreAccion; } }

    void Start()
    {
        if (barra != null)
        {
            barra.minValue = 0f;
            barra.maxValue = 1f;
            barra.wholeNumbers = false;
            barra.interactable = false;
            if (ocultarHandle && barra.handleRect != null) barra.handleRect.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("[ProgresoAccionPerro] Falta arrastrar el slider de " + nombreAccion + " de " + nombreBase, this);
        }

        // Sin datos guardados = empieza vacía; con datos = continúa donde se quedó
        if (persistir && PlayerPrefs.HasKey(Clave))
            valor = PlayerPrefs.GetFloat(Clave, 0f);
        else if (empezarVacioSiNoHayDatos)
            valor = 0f;

        valor = Mathf.Clamp01(valor);
        Pintar();
    }

    // Llámalo cada vez que el jugador hace la acción (cada pasada del cepillo, etc.)
    public void Sumar() { Sumar(cantidadPorAccion); }

    public void Sumar(float cantidad)
    {
        if (cantidad <= 0f) return;                 // nunca baja
        valor = Mathf.Clamp01(valor + cantidad);
        Pintar();
        Guardar();
    }

    // Solo para "Nueva partida"
    public void ReiniciarProgreso()
    {
        valor = 0f;
        PlayerPrefs.DeleteKey(Clave);
        Pintar();
    }

    void Guardar()
    {
        if (!persistir) return;
        PlayerPrefs.SetFloat(Clave, valor);
        PlayerPrefs.Save();
    }

    void Pintar()
    {
        if (barra == null) return;
        barra.value = valor;
    }

    void OnDisable() { Guardar(); }
    void OnApplicationPause(bool pausa) { if (pausa) Guardar(); }
    void OnApplicationQuit() { Guardar(); }
}