using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControladorDialogo2 : MonoBehaviour
{
    [Header("Referencias de UI")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;
    public Button botonSiguiente;
    public Button botonEntendido;

    [Header("Mensajes de Instrucción - Nivel 2")]
    [TextArea(2, 5)]
    public string[] mensajes = new string[]
    {
        "¡Bienvenidos al Nivel 2! Es momento de la rutina de higiene de Ody y el ejercicio de Ramón.",
        "Mueve a Ody en la tina con las flechas del teclado, lava su pelaje con la regadera, el jabón y usa el secador en círculos.",
        "En el jardín, avienta la pelota para que Ramón corra tras ella y presiona la barra espaciadora para que salte los obstáculos."
    };

    private int indiceMensaje = 0;

    void Start()
    {
        indiceMensaje = 0;
        MostrarMensajeActual();

        if (botonSiguiente != null)
            botonSiguiente.onClick.AddListener(SiguienteMensaje);

        if (botonEntendido != null)
            botonEntendido.onClick.AddListener(CerrarDialogo);
    }

    void MostrarMensajeActual()
    {
        if (textoDialogo != null && mensajes.Length > 0)
        {
            textoDialogo.text = mensajes[indiceMensaje];
        }

        bool esUltimo = (indiceMensaje == mensajes.Length - 1);

        if (botonSiguiente != null)
            botonSiguiente.gameObject.SetActive(!esUltimo);

        if (botonEntendido != null)
            botonEntendido.gameObject.SetActive(esUltimo);
    }

    public void SiguienteMensaje()
    {
        if (indiceMensaje < mensajes.Length - 1)
        {
            indiceMensaje++;
            MostrarMensajeActual();
        }
    }

    public void CerrarDialogo()
    {
        if (panelDialogo != null)
        {
            panelDialogo.SetActive(false);
        }
    }
}
