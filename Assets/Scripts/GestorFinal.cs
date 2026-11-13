using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GestorFinal : MonoBehaviour
{
    [Header("Barra de Ramon (usa solo una)")]
    public Slider barraSlider;
    public Image barraImagen;

    [Header("Panel final")]
    public GameObject panelFinal;
    public RectTransform medalla;

    [Header("Opciones")]
    public bool pausarJuego = true;
    public string escenaMenu = "";

    private bool yaMostrado;

    private void Start()
    {
        panelFinal.SetActive(false);
    }

    private void Update()
    {
        if (yaMostrado) return;

        bool llena = false;

        if (barraSlider != null)
            llena = barraSlider.value >= barraSlider.maxValue;
        else if (barraImagen != null)
            llena = barraImagen.fillAmount >= 1f;

        if (llena)
            StartCoroutine(MostrarFinal());
    }

    private IEnumerator MostrarFinal()
    {
        yaMostrado = true;
        panelFinal.SetActive(true);

        if (medalla != null)
        {
            float t = 0f;
            float duracion = 0.6f;
            medalla.localScale = Vector3.zero;

            while (t < duracion)
            {
                t += Time.unscaledDeltaTime;
                float p = Mathf.Clamp01(t / duracion);
                float escala = 1f + 0.2f * Mathf.Sin(p * Mathf.PI);
                medalla.localScale = Vector3.one * (p * escala);
                yield return null;
            }
            medalla.localScale = Vector3.one;
        }

        if (pausarJuego)
            Time.timeScale = 0f;
    }

    // Para el botón del panel
    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(escenaMenu))
            SceneManager.LoadScene(escenaMenu);
    }
}