using UnityEngine;
using UnityEngine.UI;

public class NecesidadesPerro : MonoBehaviour
{
    [Header("Nombre base para buscar las barras (Ody o Ramon)")]
    public string nombreBase = "";   // vacío = usa el nombre de este objeto

    [Header("Se llenan solos al iniciar (déjalos vacíos si quieres arrastrar tú)")]
    public Slider barraComida;
    public Slider barraAgua;
    public Slider barraSueno;
    public RectTransform grupoBarras;

    [Header("Posición sobre el perro")]
    public Vector3 offset = new Vector3(0f, 1f, 0f);
    public Vector2 ajustePixeles = Vector2.zero;

    [Header("Valores (0 = vacío, 1 = lleno)")]
    public bool empezarVacio = true;
    [Range(0f, 1f)] public float comida = 0f;
    [Range(0f, 1f)] public float agua = 0f;
    [Range(0f, 1f)] public float sueno = 0f;

    [Header("Cuánto baja por segundo")]
    public float bajaComida = 0.005f;
    public float bajaAgua = 0.008f;
    public float bajaSueno = 0.004f;

    [Header("Cuánto sube el sueño por segundo al dormir")]
    public float subeSueno = 0.08f;

    [Header("Descanso (se detecta solo si el perro tiene PerroDescanso)")]
    public PerroDescanso descanso;

    [Header("Estado")]
    public bool durmiendo;

    [Header("Aspecto")]
    public bool ocultarHandle = true;
    public bool colorRojoAVerde = true;

    Camera cam;
    Canvas canvas;
    bool durmiendoAntes;

    void Start()
    {
        if (string.IsNullOrEmpty(nombreBase)) nombreBase = gameObject.name;
        cam = Camera.main;

        // Enlace con el script de acostarse
        if (descanso == null) descanso = GetComponent<PerroDescanso>();
        if (descanso == null) descanso = GetComponentInParent<PerroDescanso>();
        if (descanso == null)
            Debug.LogWarning("[NecesidadesPerro] '" + name + "' no tiene PerroDescanso: el sueño solo sube si llamas SetDurmiendo(true).", this);

        BuscarBarras();

        if (grupoBarras == null) Debug.LogError("[NecesidadesPerro] No encuentro el grupo de barras de '" + nombreBase + "'. Arrástralo a 'Grupo Barras'.", this);
        else
        {
            var c = grupoBarras.GetComponentInParent<Canvas>();
            if (c != null) canvas = c.rootCanvas;
            else Debug.LogError("[NecesidadesPerro] '" + grupoBarras.name + "' no está dentro de un Canvas.", this);
        }
        if (cam == null) Debug.LogError("[NecesidadesPerro] No hay cámara con tag MainCamera.", this);
        if (barraComida == null || barraAgua == null) Debug.LogWarning("[NecesidadesPerro] Faltan sliders de comida o agua en '" + nombreBase + "'.", this);
        if (barraSueno == null) Debug.LogWarning("[NecesidadesPerro] Falta el slider de sueño (BarraSueno" + nombreBase + ") en '" + nombreBase + "'.", this);

        if (empezarVacio) { comida = 0f; agua = 0f; sueno = 0f; }

        Configurar(barraComida);
        Configurar(barraAgua);
        Configurar(barraSueno);
        ActualizarBarras();

        Debug.Log("[NecesidadesPerro] Activo en '" + name + "' | grupo: " + (grupoBarras != null ? grupoBarras.name : "NINGUNO"), this);
    }

    void BuscarBarras()
    {
        if (grupoBarras == null)
        {
            var go = GameObject.Find("Barras" + nombreBase + "Group");
            if (go != null) grupoBarras = go.GetComponent<RectTransform>();
        }
        if (grupoBarras == null) return;

        if (barraComida == null) barraComida = Hijo("BarraHambre" + nombreBase);
        if (barraAgua == null) barraAgua = Hijo("BarraHidratacion" + nombreBase);
        if (barraSueno == null) barraSueno = Hijo("BarraSueno" + nombreBase);
    }

    Slider Hijo(string nombre)
    {
        var t = grupoBarras.Find(nombre);
        return t != null ? t.GetComponent<Slider>() : null;
    }

    void Update()
    {
        // El sueño sube mientras el perro está acostado
        if (descanso != null)
        {
            durmiendo = descanso.Acostado;
            if (durmiendo != durmiendoAntes)
            {
                Debug.Log("[NecesidadesPerro] " + name + (durmiendo ? " se acostó" : " se levantó"));
                durmiendoAntes = durmiendo;
            }
        }

        comida = Mathf.Clamp01(comida - bajaComida * Time.deltaTime);
        agua = Mathf.Clamp01(agua - bajaAgua * Time.deltaTime);

        float cambio = durmiendo ? subeSueno : -bajaSueno;
        sueno = Mathf.Clamp01(sueno + cambio * Time.deltaTime);

        ActualizarBarras();
    }

    void LateUpdate()
    {
        if (grupoBarras == null || canvas == null) return;
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        Vector2 pantalla = (Vector2)cam.WorldToScreenPoint(transform.position + offset) + ajustePixeles;

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            grupoBarras.position = pantalla;
        }
        else
        {
            Camera camUI = canvas.worldCamera != null ? canvas.worldCamera : cam;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvas.transform as RectTransform, pantalla, camUI, out Vector3 mundo);
            grupoBarras.position = mundo;
        }
    }

    // ---- Llámalos desde tus otros scripts ----
    public void Comer(float cantidad = 0.5f) { comida = Mathf.Clamp01(comida + cantidad); }
    public void Beber(float cantidad = 0.5f) { agua = Mathf.Clamp01(agua + cantidad); }
    public void SetDurmiendo(bool valor) { durmiendo = valor; }   // solo se usa si no hay PerroDescanso

    // ---- Internos ----
    void Configurar(Slider s)
    {
        if (s == null) return;
        s.minValue = 0f;
        s.maxValue = 1f;
        s.wholeNumbers = false;
        s.interactable = false;
        if (ocultarHandle && s.handleRect != null) s.handleRect.gameObject.SetActive(false);
    }

    void ActualizarBarras()
    {
        Pintar(barraComida, comida);
        Pintar(barraAgua, agua);
        Pintar(barraSueno, sueno);
    }

    void Pintar(Slider s, float valor)
    {
        if (s == null) return;
        s.value = valor;
        if (colorRojoAVerde && s.fillRect != null)
        {
            var img = s.fillRect.GetComponent<Image>();
            if (img != null) img.color = Color.Lerp(Color.red, Color.green, valor);
        }
    }
}