using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ControladorBano : MonoBehaviour
{
    [Header("Referencia al Jugador (Ody)")]
    public ControlJugador jugador;

    [Header("Efecto de Agua")]
    public GameObject aguaEfecto;

    [Header("Sliders Individuales")]
    public Slider sliderJabon;
    public Slider sliderRegadera;
    public Slider sliderSecador;

    private float progresoJabon = 0f;
    private float progresoRegadera = 0f;
    private float progresoSecador = 0f;
    private float clicsNecesarios = 3f;

    void Start()
    {
        if (aguaEfecto != null) aguaEfecto.SetActive(false);

        ConfigurarSlider(sliderJabon);
        ConfigurarSlider(sliderRegadera);
        ConfigurarSlider(sliderSecador);
    }

    void ConfigurarSlider(Slider slider)
    {
        if (slider != null)
        {
            slider.minValue = 0;
            slider.maxValue = clicsNecesarios;
            slider.value = 0;
        }
    }

    public void InteractuarConHerramienta(string nombreHerramienta)
    {
        if (jugador == null) return;

        // 1. JABÓN (Solo si está Sucio)
        if (nombreHerramienta == "Jabon" && jugador.estadoActual == ControlJugador.EstadoMascota.Sucio)
        {
            progresoJabon++;
            if (sliderJabon != null) sliderJabon.value = progresoJabon;

            if (progresoJabon >= clicsNecesarios)
            {
                jugador.CambiarEstado(ControlJugador.EstadoMascota.Enjabonado);
            }
        }
        // 2. REGADERA (Solo si está Enjabonado)
        else if (nombreHerramienta == "Regadera" && jugador.estadoActual == ControlJugador.EstadoMascota.Enjabonado)
        {
            StartCoroutine(MostrarAguaTemporal());
            progresoRegadera++;
            if (sliderRegadera != null) sliderRegadera.value = progresoRegadera;

            if (progresoRegadera >= clicsNecesarios)
            {
                jugador.CambiarEstado(ControlJugador.EstadoMascota.Mojado);
            }
        }
        // 3. SECADOR (Solo si está Mojado)
        else if (nombreHerramienta == "Secador" && jugador.estadoActual == ControlJugador.EstadoMascota.Mojado)
        {
            progresoSecador++;
            if (sliderSecador != null) sliderSecador.value = progresoSecador;

            if (progresoSecador >= clicsNecesarios)
            {
                jugador.CambiarEstado(ControlJugador.EstadoMascota.Limpio);
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