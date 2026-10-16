using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroduccionJuego : MonoBehaviour
{
    [Header("UI del Diálogo")]
    public TextMeshProUGUI textoHabitante; // Muestra "Karla" o "Amiga"
    public TextMeshProUGUI textoDialogo;   // Muestra la frase
    public TextMeshProUGUI textoBoton;     // Arrastra aquí el texto dentro del Botón Siguiente
    public GameObject panelDialogo;

    [Header("Velocidad de Escritura")]
    public float velocidadTexto = 0.03f;
    private bool escribiendo = false;
    private int indiceActual = 0;

    [System.Serializable]
    public struct LineaDialogo
    {
        public string hablante;
        [TextArea(2, 4)]
        public string texto;
    }

    [Header("Secuencia de Historia")]
    public LineaDialogo[] secuenciaDialogo;

    void Start()
    {
        secuenciaDialogo = new LineaDialogo[]
        {
            new LineaDialogo {
                hablante = "Karla",
                texto = "¡Bienvenidos a RamOdy! Este juego está inspirado en nuestras propias mascotas..."
            },
            new LineaDialogo {
                hablante = "Yaretzi",
                texto = "¡Así que prepárate! Ody y Ramón son divinos."
            },
            new LineaDialogo {
                hablante = "Karla",
                texto = "Tu misión es cuidarlos, darles su agua, sus croquetas y dejarlos al 100% para las sorpresas."
            },
            new LineaDialogo {
                hablante = "Yaretzi",
                texto = "Cada cosa que hagas por ellos cuenta. ¡A ver si logras consentirlos a los dos! ¡A jugar!"
            }
        };

        // Al inicio nos aseguramos de que el botón diga "Siguiente"
        if (textoBoton != null)
        {
            textoBoton.text = "Siguiente";
        }

        MostrarSiguienteLinea();
    }

    public void AvanzarDialogo()
    {
        if (escribiendo)
        {
            StopAllCoroutines();
            textoDialogo.text = secuenciaDialogo[indiceActual - 1].texto;
            escribiendo = false;
        }
        else
        {
            MostrarSiguienteLinea();
        }
    }

    void MostrarSiguienteLinea()
    {
        if (indiceActual < secuenciaDialogo.Length)
        {
            if (textoHabitante != null)
            {
                textoHabitante.text = secuenciaDialogo[indiceActual].hablante + ":";
            }

            // Si llegamos a la última frase, cambiamos el texto del botón a "¡Comenzar!"
            if (indiceActual == secuenciaDialogo.Length - 1 && textoBoton != null)
            {
                textoBoton.text = "¡Comenzar!";
            }

            StartCoroutine(EscribirEfecto(secuenciaDialogo[indiceActual].texto));
            indiceActual++;
        }
        else
        {
            // Al hacer clic en "¡Comenzar!" pasa a la Escena1
            SceneManager.LoadScene("Escena1");
        }
    }

    IEnumerator EscribirEfecto(string frase)
    {
        escribiendo = true;
        textoDialogo.text = "";

        foreach (char letra in frase.ToCharArray())
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadTexto);
        }

        escribiendo = false;
    }
}