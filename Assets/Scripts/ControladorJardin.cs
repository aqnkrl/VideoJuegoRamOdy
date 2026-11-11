using UnityEngine;
using UnityEngine.UI;

public class ControladorJardin : MonoBehaviour
{
    [Header("Sliders de Ramón en UI")]
    public Slider sliderPelota;
    public Slider sliderSalto;

    [Header("Configuración de Intentos")]
    public float valorMaximo = 4f; // Número exacto de veces que debe hacer la acción para llenarse

    void Start()
    {
        // Configurar valores iniciales de los sliders
        if (sliderPelota != null)
        {
            sliderPelota.minValue = 0;
            sliderPelota.maxValue = valorMaximo;
            if (DatosJuego.Instance != null)
                sliderPelota.value = DatosJuego.Instance.progresoPelota;
        }

        if (sliderSalto != null)
        {
            sliderSalto.minValue = 0;
            sliderSalto.maxValue = valorMaximo;
            if (DatosJuego.Instance != null)
                sliderSalto.value = DatosJuego.Instance.progresoSalto;
        }
    }

    public void SumarProgresoPelota()
    {
        if (sliderPelota != null)
        {
            // Sume de 1 en 1 sin pasarse del máximo
            sliderPelota.value = Mathf.Min(sliderPelota.value + 1f, valorMaximo);

            if (DatosJuego.Instance != null)
            {
                DatosJuego.Instance.progresoPelota = sliderPelota.value;
            }
            Debug.Log("Progreso pelota Ramón: " + sliderPelota.value + "/" + valorMaximo);
        }
    }

    public void SumarProgresoSalto()
    {
        if (sliderSalto != null)
        {
            // Sume de 1 en 1 sin pasarse del máximo
            sliderSalto.value = Mathf.Min(sliderSalto.value + 1f, valorMaximo);

            if (DatosJuego.Instance != null)
            {
                DatosJuego.Instance.progresoSalto = sliderSalto.value;
            }
            Debug.Log("Progreso salto Ramón: " + sliderSalto.value + "/" + valorMaximo);
        }
    }
}