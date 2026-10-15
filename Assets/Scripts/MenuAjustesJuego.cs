using UnityEngine;
using UnityEngine.UI;

public class MenuAjustesJuego : MonoBehaviour
{
    [Header("Referencias de UI")]
    public GameObject panelMenuAjustes; // El panel flotante que se despliega
    public Slider sliderMusica;         // Slider para la música de fondo
    public Slider sliderSonido;         // Slider para los efectos de sonido

    [Header("Fuentes de Audio de la Escena")]
    public AudioSource fuenteMusica;    // Arrastra aquí tu objeto de MusicaFondo
    public AudioSource fuenteSonidos;   // Opcional: si tienes un AudioSource general para efectos

    void Start()
    {
        // Configuramos los valores iniciales de los sliders (por defecto al máximo = 1)
        if (sliderMusica != null && fuenteMusica != null)
        {
            sliderMusica.value = fuenteMusica.volume;
            sliderMusica.onValueChanged.AddListener(AjustarVolumenMusica);
        }

        if (sliderSonido != null && fuenteSonidos != null)
        {
            sliderSonido.value = fuenteSonidos.volume;
            sliderSonido.onValueChanged.AddListener(AjustarVolumenSonido);
        }

        // Aseguramos que el panel inicie cerrado
        if (panelMenuAjustes != null)
        {
            panelMenuAjustes.SetActive(false);
        }
    }

    // Método para abrir o desplegar el menú desde el botón de la esquina
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

    // Controla únicamente la música de fondo
    public void AjustarVolumenMusica(float volumen)
    {
        if (fuenteMusica != null)
        {
            fuenteMusica.volume = volumen;
        }
    }

    // Controla únicamente los efectos de sonido
    public void AjustarVolumenSonido(float volumen)
    {
        if (fuenteSonidos != null)
        {
            fuenteSonidos.volume = volumen;
        }
    }
}