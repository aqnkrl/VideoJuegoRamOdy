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

    private bool jabonCompletado = false;
    private bool regaderaCompletada = false;
    private bool secadorCompletado = false;

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

        float margen = 0.01f;
        jabonCompletado = progresoJabon >= valorMaximo - margen;
        regaderaCompletada = jabonCompletado && progresoRegadera >= valorMaximo - margen;
        secadorCompletado = regaderaCompletada && progresoSecador >= valorMaximo - margen;
    }

    void ConfigurarSlider(Slider slider, float valorInicial)
    {
        if (slider != null)
        {
            slider.minValue = 0f;
            slider.maxValue = valorMaximo;
            slider.value = valorInicial;
        }
    }

    public void InteractuarConHerramienta(string nombreHerramienta, float pasoForzado = 0f)
    {
        if (jugador == null) return;

        float incremento = pasoForzado > 0f ? pasoForzado : Time.deltaTime * 3f;
        float margen = 0.01f;

        // 1. JABÓN
        if (nombreHerramienta == "Jabon" && !jabonCompletado)
        {
            progresoJabon = Mathf.Min(progresoJabon + incremento, valorMaximo);
            if (sliderJabon != null) sliderJabon.value = progresoJabon;

            if (DatosJuego.Instance != null) DatosJuego.Instance.progresoJabon = progresoJabon;

            if (progresoJabon >= valorMaximo - margen)
            {
                progresoJabon = valorMaximo;
                if (sliderJabon != null) sliderJabon.value = valorMaximo;
                if (DatosJuego.Instance != null) DatosJuego.Instance.progresoJabon = valorMaximo;

                jabonCompletado = true;
                jugador.CambiarEstado(ControladorJugador.EstadoMascota.Enjabonado);
            }
        }
        // 2. REGADERA (Funciona incluso si Ody se mueve un poco, con solo tocarlo)
        else if (nombreHerramienta == "Regadera" && jabonCompletado && !regaderaCompletada)
        {
            if (aguaEfecto != null && !aguaEfecto.activeSelf) StartCoroutine(MostrarAguaTemporal());

            progresoRegadera = Mathf.Min(progresoRegadera + incremento, valorMaximo);
            if (sliderRegadera != null) sliderRegadera.value = progresoRegadera;

            if (DatosJuego.Instance != null) DatosJuego.Instance.progresoRegadera = progresoRegadera;

            if (progresoRegadera >= valorMaximo - margen)
            {
                progresoRegadera = valorMaximo;
                if (sliderRegadera != null) sliderRegadera.value = valorMaximo;
                if (DatosJuego.Instance != null) DatosJuego.Instance.progresoRegadera = valorMaximo;

                regaderaCompletada = true;
                jugador.CambiarEstado(ControladorJugador.EstadoMascota.Mojado);
            }
        }
        // 3. SECADOR
        else if (nombreHerramienta == "Secador" && regaderaCompletada && !secadorCompletado)
        {
            progresoSecador = Mathf.Min(progresoSecador + incremento, valorMaximo);
            if (sliderSecador != null) sliderSecador.value = progresoSecador;

            if (DatosJuego.Instance != null)
            {
                DatosJuego.Instance.progresoSecador = progresoSecador;
                DatosJuego.Instance.banoCompletado = true; // Corregido sin la "ñ"
            }

            if (progresoSecador >= valorMaximo - margen)
            {
                progresoSecador = valorMaximo;
                if (sliderSecador != null) sliderSecador.value = valorMaximo;
                if (DatosJuego.Instance != null) DatosJuego.Instance.progresoSecador = valorMaximo;

                secadorCompletado = true;
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