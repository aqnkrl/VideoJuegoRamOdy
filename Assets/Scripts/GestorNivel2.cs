using UnityEngine;
using UnityEngine.UI;

public class GestorNivel2 : MonoBehaviour
{
    public static GestorNivel2 Instancia;

    [Header("Sliders de Ody (Baño)")]
    public Slider sliderJabon;
    public Slider sliderRegadera;
    public Slider sliderSecador;

    [Header("Sliders de Ramón (Jardín)")]
    public Slider sliderPelota;
    public Slider sliderSalto;

    [Header("Panel del Regalo")]
    public GameObject panelRegalo; // Arrastra aquí el objeto de la caja de regalo cerrada
    public bool nivelCompletado = false;

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

    void Start()
    {
        // Asegurarnos de que el regalo empiece oculto al iniciar el nivel
        if (panelRegalo != null)
        {
            panelRegalo.SetActive(false);
        }
    }

    void Update()
    {
        if (nivelCompletado) return;

        // Comprueba constantemente si las 3 barras de Ody y las 2 de Ramón llegaron al máximo
        if (EstanSlidersLlenos())
        {
            CompletarNivel();
        }
    }

    bool EstanSlidersLlenos()
    {
        bool odyListo = (sliderJabon != null && sliderJabon.value >= sliderJabon.maxValue) &&
                        (sliderRegadera != null && sliderRegadera.value >= sliderRegadera.maxValue) &&
                        (sliderSecador != null && sliderSecador.value >= sliderSecador.maxValue);

        bool ramonListo = (sliderPelota != null && sliderPelota.value >= sliderPelota.maxValue) &&
                          (sliderSalto != null && sliderSalto.value >= sliderSalto.maxValue);

        return odyListo && ramonListo;
    }

    void CompletarNivel()
    {
        nivelCompletado = true;

        // Activamos el panel del regalo en la pantalla
        if (panelRegalo != null)
        {
            panelRegalo.SetActive(true);
        }

        Debug.Log("¡Nivel 2 completado! El regalo ha aparecido.");
    }
}