using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ControladorBano : MonoBehaviour
{
    [Header("Referencia al Jugador (Ody)")]
    public ControladorJugador jugador;

    [Header("Efecto de Agua")]
    public GameObject aguaEfecto;

    [Header("Sliders Individuales")]
    public Slider sliderJabon;
    public Slider sliderRegadera;
    public Slider sliderSecador;

    private float progresoJabon = 0f;
    private float progresoRegadera = 0f;
    private float progresoSecador = 0f;
    public float valorMaximo = 3f;

    void Start()
    {
        if (aguaEfecto != null) aguaEfecto.SetActive(false);

        // Recuperar datos si ya existían
        if (DatosJuego.Instance != null)
        {
            progresoJabon = DatosJuego.Instance.progresoJabon;
            progresoRegadera = DatosJuego.Instance.progresoRegadera;
            progresoSecador = DatosJuego.Instance.progresoSecador;
        }

        ConfigurarSlider(sliderJabon, progresoJabon);
        ConfigurarSlider(sliderRegadera, progresoRegadera);
        ConfigurarSlider(sliderSecador, progresoSecador);
    }

    void ConfigurarSlider(Slider slider, float valorInicial)
    {
        if (slider != null)
        {
            slider.minValue = 0;
            slider.maxValue = valorMaximo;
            slider.value = valorInicial;
        }
    }

    public void InteractuarConHerramienta(string nombreHerramienta)
    {
        if (jugador == null) return;

        // 1. JABÓN
        if (nombreHerramienta == "Jabon" && sliderJabon.value < valorMaximo)
        {
            progresoJabon += Time.deltaTime * 3f;
            progresoJabon = Mathf.Min(progresoJabon, valorMaximo);
            sliderJabon.value = progresoJabon;

            if (DatosJuego.Instance != null) DatosJuego.Instance.progresoJabon = progresoJabon;

            if (progresoJabon >= valorMaximo)
            {
                jugador.CambiarEstado(ControladorJugador.EstadoMascota.Enjabonado);
            }
        }
        // 2. REGADERA (Funciona incluso si Ody se mueve un poco, con solo tocarlo)
        else if (nombreHerramienta == "Regadera" && sliderJabon.value >= valorMaximo && sliderRegadera.value < valorMaximo)
        {
            if (aguaEfecto != null && !aguaEfecto.activeSelf) StartCoroutine(MostrarAguaTemporal());

            progresoRegadera += Time.deltaTime * 3f;
            progresoRegadera = Mathf.Min(progresoRegadera, valorMaximo);
            sliderRegadera.value = progresoRegadera;

            if (DatosJuego.Instance != null) DatosJuego.Instance.progresoRegadera = progresoRegadera;

            if (progresoRegadera >= valorMaximo)
            {
                jugador.CambiarEstado(ControladorJugador.EstadoMascota.Mojado);
            }
        }
        // 3. SECADOR
        else if (nombreHerramienta == "Secador" && sliderRegadera.value >= valorMaximo && sliderSecador.value < valorMaximo)
        {
            progresoSecador += Time.deltaTime * 3f;
            progresoSecador = Mathf.Min(progresoSecador, valorMaximo);
            sliderSecador.value = progresoSecador;

            if (DatosJuego.Instance != null)
            {
                DatosJuego.Instance.progresoSecador = progresoSecador;
                DatosJuego.Instance.banoCompletado = true; // Corregido sin la "ñ"
            }

            if (progresoSecador >= valorMaximo)
            {
                jugador.CambiarEstado(ControladorJugador.EstadoMascota.Limpio);
            }
        }
    }

    IEnumerator MostrarAguaTemporal()
    {
        if (aguaEfecto != null) aguaEfecto.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        if (aguaEfecto != null) aguaEfecto.SetActive(false);
    }
}