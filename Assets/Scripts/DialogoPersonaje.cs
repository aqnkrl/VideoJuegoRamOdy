using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class DialogoPersonaje : MonoBehaviour
{
    [Header("UI (arrástralos desde el Canvas)")]
    public GameObject panelDialogo;
    public TMP_Text textoDialogo;
    public TMP_Text textoNombre;            // opcional
    public Button botonSiguiente;
    public TMP_Text textoBoton;             // opcional: el texto dentro del botón

    [Header("Contenido")]
    public string nombrePersonaje = "Personaje";
    [TextArea(2, 5)]
    public string[] lineas =
    {
        "¡Hola! Ahora toca cuidar el descanso de Ody y de Ramón.",
        "Las barras sobre cada uno muestran su comida, su agua y su sueño.",
        "Llena sus platos para subir la comida y el agua.",
        "Cuando tengan sueño, acuéstalos en su cama: Ramón con E (se mueve con W, A, S, D) y Ody con O (se mueve con las flechas).",
        "Para levantarlos, solo muévelos otra vez. ¡Cuídalos mucho!"
    };

    [Header("Opciones")]
    public bool iniciarAlComenzar = true;
    public float velocidadLetra = 0.03f;    // segundos por letra (0 = aparece todo de golpe)
    public KeyCode teclaContinuar = KeyCode.Space;
    public string textoSiguiente = "Siguiente";
    public string textoUltimo = "¡Entendido!";

    [Header("Bloquear a los perros mientras habla (arrastra Ody y Ramon)")]
    public bool bloquearMovimiento = true;
    public GameObject[] perros;

    [Header("Cuando termina de hablar")]
    public bool desaparecerAlTerminar = true;
    public bool conFundido = true;          // se desvanece en vez de desaparecer de golpe
    public float duracionFundido = 0.8f;    // segundos que tarda en desvanecerse
    public UnityEvent alTerminar;

    int indice;
    bool activo;
    bool escribiendo;
    Coroutine rutina;

    void Start()
    {
        if (botonSiguiente != null) botonSiguiente.onClick.AddListener(Siguiente);

        if (iniciarAlComenzar) Iniciar();
        else if (panelDialogo != null) panelDialogo.SetActive(false);
    }

    void Update()
    {
        if (activo && teclaContinuar != KeyCode.None && Input.GetKeyDown(teclaContinuar))
            Siguiente();
    }

    // Llámalo desde otro script o desde un botón para volver a mostrar el diálogo
    public void Iniciar()
    {
        if (lineas == null || lineas.Length == 0)
        {
            Debug.LogWarning("[DialogoPersonaje] No hay líneas de diálogo en '" + name + "'.", this);
            return;
        }
        if (panelDialogo == null || textoDialogo == null)
        {
            Debug.LogError("[DialogoPersonaje] Falta arrastrar el Panel o el Texto en '" + name + "'.", this);
            return;
        }

        activo = true;
        indice = 0;
        panelDialogo.SetActive(true);
        if (textoNombre != null) textoNombre.text = nombrePersonaje;
        BloquearMovimiento(true);
        MostrarLinea();
    }

    void MostrarLinea()
    {
        if (rutina != null) StopCoroutine(rutina);

        if (textoBoton != null)
            textoBoton.text = (indice >= lineas.Length - 1) ? textoUltimo : textoSiguiente;

        rutina = StartCoroutine(Escribir(lineas[indice]));
    }

    IEnumerator Escribir(string linea)
    {
        escribiendo = true;
        textoDialogo.text = "";

        if (velocidadLetra <= 0f)
        {
            textoDialogo.text = linea;
        }
        else
        {
            foreach (char letra in linea)
            {
                textoDialogo.text += letra;
                yield return new WaitForSecondsRealtime(velocidadLetra);
            }
        }
        escribiendo = false;
    }

    public void Siguiente()
    {
        if (!activo) return;

        // Si aún está escribiendo, primero muestra la línea completa
        if (escribiendo)
        {
            if (rutina != null) StopCoroutine(rutina);
            textoDialogo.text = lineas[indice];
            escribiendo = false;
            return;
        }

        indice++;
        if (indice >= lineas.Length) Terminar();
        else MostrarLinea();
    }

    void Terminar()
    {
        activo = false;
        if (panelDialogo != null) panelDialogo.SetActive(false);
        BloquearMovimiento(false);
        alTerminar.Invoke();

        if (desaparecerAlTerminar)
        {
            if (conFundido) StartCoroutine(Desvanecer());
            else gameObject.SetActive(false);
        }
    }

    IEnumerator Desvanecer()
    {
        var sprites = GetComponentsInChildren<SpriteRenderer>();
        if (sprites.Length == 0 || duracionFundido <= 0f)
        {
            gameObject.SetActive(false);
            yield break;
        }

        float t = 0f;
        while (t < duracionFundido)
        {
            t += Time.deltaTime;
            float alfa = 1f - Mathf.Clamp01(t / duracionFundido);
            foreach (var sr in sprites)
            {
                Color c = sr.color;
                c.a = alfa;
                sr.color = c;
            }
            yield return null;
        }
        gameObject.SetActive(false);
    }

    // Apaga los scripts de movimiento de los perros (no toca sus barras ni su descanso)
    void BloquearMovimiento(bool bloquear)
    {
        if (!bloquearMovimiento || perros == null) return;

        foreach (var perro in perros)
        {
            if (perro == null) continue;
            foreach (var s in perro.GetComponents<MonoBehaviour>())
            {
                if (s == null) continue;
                if (s is NecesidadesPerro || s is PerroDescanso) continue;   // estos siguen activos
                s.enabled = !bloquear;
            }
        }
    }
}