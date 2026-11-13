using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

// Nivel 3: "El Gran Día de Cuidado Completo"
// Un mismo personaje (la niña) da las indicaciones en varios momentos del nivel:
//   1) Inicio (Sección A)   2) Cambio a Sección B   3) Final (premios)
// Se reutiliza con IniciarConLineas(...) desde GestorNivel3 (o desde eventos).
public class DialogoNivel3 : MonoBehaviour
{
    [Header("UI (arrástralos desde el Canvas)")]
    public GameObject panelDialogo;
    public TMP_Text textoDialogo;
    public TMP_Text textoNombre;            // opcional
    public Button botonSiguiente;
    public TMP_Text textoBoton;             // opcional: el texto dentro del botón

    [Header("Contenido (instrucciones del Nivel 3)")]
    public string nombrePersonaje = "Karla";
    [TextArea(2, 5)]
    public string[] lineas =
    {
        "¡Hola! Hoy es el Gran Día de Cuidado Completo. Vamos a cuidar a Ody y a Ramón de la mañana a la noche.",
        "Por la mañana, arrastra la comida y el agua hasta sus platos.",
        "Después, Ramón quiere correr un poquito: desliza el dedo hacia arriba para que salte.",
        "Por la noche, desliza el cepillo sobre el lomo de Ody.",
        "Para terminar, arrastra a Ody y a Ramón hasta sus camitas. ¡Así ganarás la Gran Medalla de Cuidador Responsable y un disfraz especial!"
    };

    [Header("Opciones")]
    public bool iniciarAlComenzar = true;
    public float velocidadLetra = 0.03f;    // segundos por letra (0 = aparece todo de golpe)
    public KeyCode teclaContinuar = KeyCode.Space;
    public string textoSiguiente = "Siguiente";
    public string textoUltimo = "¡Entendido!";

    [Header("Bloquear mientras habla (arrastra los scripts de arrastre/salto/cepillo)")]
    public bool bloquearMovimiento = true;
    public MonoBehaviour[] scriptsABloquear;

    [Header("Cuando termina de hablar")]
    [Tooltip("Déjalo en FALSE para que el personaje siga disponible en las siguientes secciones. " +
             "GestorNivel3 lo pone en TRUE solo en el diálogo final.")]
    public bool desaparecerAlTerminar = false;
    public bool conFundido = true;          // se desvanece en vez de desaparecer de golpe
    public float duracionFundido = 0.8f;    // segundos que tarda en desvanecerse
    public UnityEvent alTerminar;

    [Header("Desaparecen junto con el diálogo (arrastra aquí a la niña)")]
    public GameObject[] objetosQueDesaparecen;

    int indice;
    bool activo;
    bool escribiendo;
    Coroutine rutina;
    string[] lineasActuales;

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

    // Muestra las líneas configuradas en el Inspector
    public void Iniciar()
    {
        IniciarConLineas(nombrePersonaje, lineas, desaparecerAlTerminar);
    }

    // Muestra otro diálogo (Sección B, final, etc.)
    public void IniciarConLineas(string nombre, string[] nuevasLineas, bool desaparecerAlFinal)
    {
        if (nuevasLineas == null || nuevasLineas.Length == 0)
        {
            Debug.LogWarning("[DialogoNivel3] No hay líneas de diálogo en '" + name + "'.", this);
            return;
        }
        if (panelDialogo == null || textoDialogo == null)
        {
            Debug.LogError("[DialogoNivel3] Falta arrastrar el Panel o el Texto en '" + name + "'.", this);
            return;
        }

        if (rutina != null) StopCoroutine(rutina);

        lineasActuales = nuevasLineas;
        desaparecerAlTerminar = desaparecerAlFinal;
        activo = true;
        indice = 0;

        // Si venía de un fundido, deja el panel visible de nuevo
        var grupo = panelDialogo.GetComponent<CanvasGroup>();
        if (grupo != null) grupo.alpha = 1f;

        panelDialogo.SetActive(true);
        if (textoNombre != null) textoNombre.text = nombre;
        BloquearMovimiento(true);
        MostrarLinea();
    }

    void MostrarLinea()
    {
        if (rutina != null) StopCoroutine(rutina);

        if (textoBoton != null)
            textoBoton.text = (indice >= lineasActuales.Length - 1) ? textoUltimo : textoSiguiente;

        rutina = StartCoroutine(Escribir(lineasActuales[indice]));
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
            textoDialogo.text = lineasActuales[indice];
            escribiendo = false;
            return;
        }

        indice++;
        if (indice >= lineasActuales.Length) Terminar();
        else MostrarLinea();
    }

    void Terminar()
    {
        activo = false;
        BloquearMovimiento(false);
        alTerminar.Invoke();

        // IMPORTANTE: no se desactiva nada ANTES de iniciar la corrutina,
        // porque este script puede estar dentro del propio PanelDialogo.
        if (desaparecerAlTerminar && conFundido && isActiveAndEnabled)
            StartCoroutine(Desvanecer());
        else
            Ocultar();
    }

    // Oculta el panel y, si se pidió, también el objeto que tiene este script
    void Ocultar()
    {
        if (panelDialogo != null) panelDialogo.SetActive(false);
        if (desaparecerAlTerminar)
        {
            if (objetosQueDesaparecen != null)
                foreach (var o in objetosQueDesaparecen)
                    if (o != null) o.SetActive(false);

            gameObject.SetActive(false);
        }
    }

    IEnumerator Desvanecer()
    {
        CanvasGroup grupo = null;
        if (panelDialogo != null)
        {
            grupo = panelDialogo.GetComponent<CanvasGroup>();
            if (grupo == null) grupo = panelDialogo.AddComponent<CanvasGroup>();
        }

        // Sprites a desvanecer: los de este objeto y los de los objetos que desaparecen (la niña)
        var sprites = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
        if (objetosQueDesaparecen != null)
            foreach (var o in objetosQueDesaparecen)
                if (o != null) sprites.AddRange(o.GetComponentsInChildren<SpriteRenderer>());

        if ((grupo == null && sprites.Count == 0) || duracionFundido <= 0f)
        {
            Ocultar();
            yield break;
        }

        float t = 0f;
        while (t < duracionFundido)
        {
            t += Time.deltaTime;
            float alfa = 1f - Mathf.Clamp01(t / duracionFundido);

            if (grupo != null) grupo.alpha = alfa;

            foreach (var sr in sprites)
            {
                if (sr == null) continue;
                Color c = sr.color;
                c.a = alfa;
                sr.color = c;
            }
            yield return null;
        }

        // Deja el panel listo por si se vuelve a usar
        if (grupo != null) grupo.alpha = 1f;

        Ocultar();
    }

    // Apaga/enciende los scripts indicados (arrastre, salto, cepillo...) mientras la niña habla
    void BloquearMovimiento(bool bloquear)
    {
        if (!bloquearMovimiento || scriptsABloquear == null) return;

        foreach (var s in scriptsABloquear)
            if (s != null) s.enabled = !bloquear;
    }
}
