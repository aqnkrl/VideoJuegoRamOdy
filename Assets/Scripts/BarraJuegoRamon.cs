using UnityEngine;
using UnityEngine.UI;

public class BarraJuegoRamon : MonoBehaviour
{
    public static BarraJuegoRamon Instancia;

    [Header("Referencias")]
    public Slider barra;
    public Image relleno;              // opcional: el Fill del slider, para cambiar de color

    [Header("Ajustes")]
    public float incrementoPorJuego = 0.2f;
    public float velocidadLlenado = 1.5f;

    float valorObjetivo;

    void Awake()
    {
        Instancia = this;
        if (barra == null) barra = GetComponent<Slider>();
        barra.minValue = 0f;
        barra.maxValue = 1f;
        barra.value = 0f;
        valorObjetivo = 0f;
    }

    void Update()
    {
        barra.value = Mathf.MoveTowards(barra.value, valorObjetivo, velocidadLlenado * Time.deltaTime);

        if (relleno != null)
            relleno.color = Color.Lerp(Color.red, Color.green, barra.value);
    }

    public void RegistrarJuego()
    {
        valorObjetivo = Mathf.Clamp01(valorObjetivo + incrementoPorJuego);
    }

    public void LlenarContinuo(float cantidadPorSegundo)
    {
        valorObjetivo = Mathf.Clamp01(valorObjetivo + cantidadPorSegundo * Time.deltaTime);
    }
}