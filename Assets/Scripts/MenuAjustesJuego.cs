using UnityEngine;
using UnityEngine.UI;

public class MenuAjustesJuego : MonoBehaviour
{
    [Header("Referencias de UI")]
    public GameObject panelMenuAjustes; // El panel flotante que se despliega
    public Slider sliderMusica;         // Slider para la música de fondo
    public Slider sliderSonido;         // Slider para los efectos de sonido

    void Start()
    {
        // 1. Configurar el slider de música con el valor guardado
        if (sliderMusica != null)
        {
            float volumenActual = PlayerPrefs.GetFloat("VolumenMusica", 1f);
            sliderMusica.value = volumenActual;
            sliderMusica.onValueChanged.AddListener(AjustarVolumenMusica);
        }

        // 2. Configurar el slider de efectos de sonido
        if (sliderSonido != null)
        {
            float sonidoActual = PlayerPrefs.GetFloat("VolumenSonido", 1f);
            sliderSonido.value = sonidoActual;
            sliderSonido.onValueChanged.AddListener(AjustarVolumenSonido);
        }

        // Aseguramos que el panel inicie cerrado
        if (panelMenuAjustes != null)
        {
            panelMenuAjustes.SetActive(false);
        }
    }

    public void AlternarMenu()
    {
        if (panelMenuAjustes != null)
        {
            bool estadoActual = panelMenuAjustes.activeSelf;
            panelMenuAjustes.SetActive(!estadoActual);
        }
    }

    public void CerrarMenu()
    {
        if (panelMenuAjustes != null)
        {
            panelMenuAjustes.SetActive(false);
        }
    }

    public void AjustarVolumenMusica(float volumen)
    {
        // Buscar siempre el controlador global vigente en la escena actual o anterior
        ControladorMusica controlador = Object.FindFirstObjectByType<ControladorMusica>();
        if (controlador != null)
        {
            controlador.CambiarVolumen(volumen);
        }
        else
        {
            PlayerPrefs.SetFloat("VolumenMusica", volumen);
            PlayerPrefs.Save();
        }
    }

    public void AjustarVolumenSonido(float volumen)
    {
        PlayerPrefs.SetFloat("VolumenSonido", volumen);
        PlayerPrefs.Save();

        // Controla el volumen general de todos los efectos del juego (jabón, agua, secadora, saltos)
        AudioListener.volume = volumen;
    }
}