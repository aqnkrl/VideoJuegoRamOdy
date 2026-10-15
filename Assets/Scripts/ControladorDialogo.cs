using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControladorDialogo : MonoBehaviour
{
    [Header("Referencias de UI")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;
    public Button botonSiguiente;
    public Button botonEntendido;

    [Header("Mensajes de Instrucción")]
    [TextArea(2, 5)]
    public string[] mensajes = new string[]
    {
        "¡Hola! En este Nivel 1 tienes que alimentar e hidratar a Ody y a Ramón para cuidarlos.",
        "Mueve a Ody usando las flechas del teclado y a Ramón usando las teclas W, A, S y D.",
        "Arrastra las croquetas y el agua a sus respectivos platos para llenar las barras de estado. ¡Cuando estén llenas avanzarás de nivel!"
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