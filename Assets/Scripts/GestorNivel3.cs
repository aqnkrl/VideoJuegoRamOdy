using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GestorNivel3 : MonoBehaviour
{
    [Header("Sliders de Ody (Vienen llenos de la Escena 2)")]
    public Slider sliderJabonOdy;
    public Slider sliderRegaderaOdy;
    public Slider sliderSecadorOdy;

    [Header("Sliders de Ramón (Se llenan en la Escena 3)")]
    public Slider sliderPelotaRamon;
    public Slider sliderSaltoRamon;

    [Header("Diálogo final")]
    public DialogoNivel3 dialogoFinal;
    public string nombrePersonaje = "Karla";
    [TextArea(2, 5)]
    public string[] lineasFinales =
    {
        "¡Felicidades! Completaste todos los niveles con mucho cariño y esfuerzo. Ody y Ramón están felices y bien cuidados.",
        "Por portarse tan bien, Ody y Ramón se ganaron unos disfraces muy especiales. ¡Mira qué bien se ven!",
        "Y tú, como excelente cuidador, te ganaste una medalla. ¡Gracias por cuidar tan bien de ellos!"
    };

    [Header("Panel del Regalo (Caja Cerrada)")]
    public GameObject panelRegaloCerrado;

    [Header("Elementos opcionales a ordenar al abrir")]
    public GameObject objetoRegaloCerrado;
    public GameObject panelAccesoriosAbierto;
    public GameObject botonEquipar;
    public GameObject botonSiguienteNivel;

    public float valorMaximoOdy = 3f;
    public float valorMaximoRamon = 4f;

    private bool nivelCompletado = false;

    void Start()
    {
        if (panelRegaloCerrado != null) panelRegaloCerrado.SetActive(false);

        if (DatosJuego.Instance != null)
        {
            if (sliderJabonOdy != null) { sliderJabonOdy.maxValue = valorMaximoOdy; sliderJabonOdy.value = DatosJuego.Instance.progresoJabon; }
            if (sliderRegaderaOdy != null) { sliderRegaderaOdy.maxValue = valorMaximoOdy; sliderRegaderaOdy.value = DatosJuego.Instance.progresoRegadera; }
            if (sliderSecadorOdy != null) { sliderSecadorOdy.maxValue = valorMaximoOdy; sliderSecadorOdy.value = DatosJuego.Instance.progresoSecador; }
        }
    }

    void Update()
    {
        if (nivelCompletado) return;

        if (DatosJuego.Instance != null)
        {
            if (sliderPelotaRamon != null) DatosJuego.Instance.progresoPelota = sliderPelotaRamon.value;
            if (sliderSaltoRamon != null) DatosJuego.Instance.progresoSalto = sliderSaltoRamon.value;

            bool odyListo = DatosJuego.Instance.progresoJabon >= valorMaximoOdy &&
                            DatosJuego.Instance.progresoRegadera >= valorMaximoOdy &&
                            DatosJuego.Instance.progresoSecador >= valorMaximoOdy;

            bool ramonListo = DatosJuego.Instance.progresoPelota >= valorMaximoRamon &&
                              DatosJuego.Instance.progresoSalto >= valorMaximoRamon;

            if (odyListo && ramonListo)
            {
                nivelCompletado = true;
                IniciarDialogoFinal();
            }
        }
    }

    void IniciarDialogoFinal()
    {
        if (dialogoFinal != null)
        {
            dialogoFinal.alTerminar.RemoveListener(MostrarRegalo);
            dialogoFinal.alTerminar.AddListener(MostrarRegalo);
            dialogoFinal.IniciarConLineas(nombrePersonaje, lineasFinales, false);
        }
        else
        {
            MostrarRegalo();
        }
    }

    void MostrarRegalo()
    {
        if (panelRegaloCerrado != null)
        {
            panelRegaloCerrado.SetActive(true);

            // Muestra la caja cerrada y oculta el botón de siguiente nivel para mantener la secuencia
            if (objetoRegaloCerrado != null) objetoRegaloCerrado.SetActive(true);
            if (panelAccesoriosAbierto != null) panelAccesoriosAbierto.SetActive(false);
            if (botonEquipar != null) botonEquipar.SetActive(false);

            // CORREGIDO: Inicia apagado para forzar la apertura del cofre primero
            if (botonSiguienteNivel != null) botonSiguienteNivel.SetActive(false);

            Debug.Log("Diálogo terminado: aparece el regalo final.");
        }
    }

    // Método que llamará tu botón directamente para ir a la Escena 4
    public void IrASiguienteNivel()
    {
        SceneManager.LoadScene("Escena4");
    }
}