using UnityEngine;
using UnityEngine.UI;

public class GestorNivel3 : MonoBehaviour
{
    [Header("Sliders de Ody (Vienen llenos de la Escena 2)")]
    public Slider sliderJabonOdy;
    public Slider sliderRegaderaOdy;
    public Slider sliderSecadorOdy;

    [Header("Sliders de Ramón (Se llenan en la Escena 3)")]
    public Slider sliderPelotaRamon;
    public Slider sliderSaltoRamon;

    [Header("Panel del Regalo (Caja Cerrada)")]
    public GameObject panelRegaloCerrado; // El objeto raíz de tu UI de regalo (la caja cerrada)

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
        // Al iniciar la escena 3, aseguramos que el panel del regalo empiece oculto
        if (panelRegaloCerrado != null) panelRegaloCerrado.SetActive(false);

        // Asignar los valores de Ody guardados desde el nivel anterior
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

        // Actualizar progreso de Ramón en tiempo real
        if (DatosJuego.Instance != null)
        {
            if (sliderPelotaRamon != null) DatosJuego.Instance.progresoPelota = sliderPelotaRamon.value;
            if (sliderSaltoRamon != null) DatosJuego.Instance.progresoSalto = sliderSaltoRamon.value;

            // Verificar si ambos perritos completaron sus tareas
            bool odyListo = DatosJuego.Instance.progresoJabon >= valorMaximoOdy &&
                            DatosJuego.Instance.progresoRegadera >= valorMaximoOdy &&
                            DatosJuego.Instance.progresoSecador >= valorMaximoOdy;

            bool ramonListo = DatosJuego.Instance.progresoPelota >= valorMaximoRamon &&
                              DatosJuego.Instance.progresoSalto >= valorMaximoRamon;

            if (odyListo && ramonListo)
            {
                nivelCompletado = true;
                MostrarRegalo();
            }
        }
    }

    void MostrarRegalo()
    {
        if (panelRegaloCerrado != null)
        {
            panelRegaloCerrado.SetActive(true); // ¡Aparece el panel del regalo en la pantalla!

            // Forzamos el orden visual inicial por si acaso
            if (objetoRegaloCerrado != null) objetoRegaloCerrado.SetActive(true);
            if (panelAccesoriosAbierto != null) panelAccesoriosAbierto.SetActive(false);
            if (botonEquipar != null) botonEquipar.SetActive(false);
            if (botonSiguienteNivel != null) botonSiguienteNivel.SetActive(false);

            Debug.Log("¡Todas las barras de Ody y Ramón están listas! Aparece el regalo final.");
        }
    }
}
