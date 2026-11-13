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

    [Header("Cuánto baja por segundo (queda limitado por el mínimo alcanzado)")]
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
    [Tooltip("Las barras usan el mismo estilo que BarraCepilladoOdy: se respeta el aspecto del Slider (círculo, color y sprites), sin ocultar el handle ni pintar de rojo a verde.")]
    public bool estiloCepillado = true;

    [Header("Persistencia")]
    public bool persistir = true;
    [Tooltip("Al iniciar el juego, las barras empiezan vacías aunque haya datos guardados. Al cambiar de escena sí se conservan.")]
    public bool empezarVacioCadaSesion = true;

    // Perros que ya aparecieron en esta partida (se limpia al dar Play / abrir el juego)
    static readonly System.Collections.Generic.HashSet<string> sesionesIniciadas =
        new System.Collections.Generic.HashSet<string>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetEstatico() { sesionesIniciadas.Clear(); }

    Camera cam;
    Canvas canvas;
    bool durmiendoAntes;

    // Pisos: el valor más alto alcanzado. Nunca bajan.
    float minComida, minAgua, minSueno;
    bool hayCambios;
    float tiempoGuardado;
    const float INTERVALO_GUARDADO = 1f;   // segundos entre guardados

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
        // Primera vez que aparece este perro en esta partida: borra lo guardado
        if (persistir && empezarVacioCadaSesion && sesionesIniciadas.Add(nombreBase))
            ReiniciarProgreso();
        Cargar();   // si hay datos guardados, pisan los valores iniciales

        Configurar(barraComida);
        Configurar(barraAgua);
        Configurar(barraSueno);
        ActualizarBarras();

        Debug.Log("[NecesidadesPerro] " + nombreBase + " | empezarVacio=" + empezarVacio +
                  " | sesionVacia=" + empezarVacioCadaSesion +
                  " | comida=" + comida + " agua=" + agua + " sueno=" + sueno +
                  " | pisos=" + minComida + "/" + minAgua + "/" + minSueno +
                  " | guardado=" + PlayerPrefs.GetFloat(Clave("comida"), -1f) +
                  " | grupo: " + (grupoBarras != null ? grupoBarras.name : "NINGUNO"), this);
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

                // Sonido de dormir: suena mientras al menos un perro esté acostado
                if (SonidoDormir.Instance != null)
                {
                    if (durmiendo) SonidoDormir.Instance.PerroSeAcosto();
                    else SonidoDormir.Instance.PerroSeLevanto();
                }

                durmiendoAntes = durmiendo;
            }
        }

        // La baja nunca puede dejar el valor por debajo del piso alcanzado
        comida = Mathf.Max(minComida, Mathf.Clamp01(comida - bajaComida * Time.deltaTime));
        agua = Mathf.Max(minAgua, Mathf.Clamp01(agua - bajaAgua * Time.deltaTime));

        float cambio = durmiendo ? subeSueno : -bajaSueno;
        sueno = Mathf.Max(minSueno, Mathf.Clamp01(sueno + cambio * Time.deltaTime));

        // Todo valor alcanzado se vuelve el nuevo mínimo
        ActualizarPisos();

        if (persistir && hayCambios && Time.time - tiempoGuardado >= INTERVALO_GUARDADO)
            Guardar();

        if (comida > 0.9f && Time.timeSinceLevelLoad < 3f)
            Debug.LogWarning("[NecesidadesPerro] " + nombreBase + " comida=" + comida + " a los " + Time.timeSinceLevelLoad + "s", this);

        ActualizarBarras();
    }

    void OnDisable()
    {
        // Si el perro se desactiva mientras dormía, restamos del contador
        if (durmiendoAntes)
        {
            if (SonidoDormir.Instance != null) SonidoDormir.Instance.PerroSeLevanto();
            durmiendoAntes = false;
        }

        // Se ejecuta al cambiar de escena
        ActualizarPisos();
        Guardar();
    }

    void OnApplicationPause(bool pausa)
    {
        if (pausa) { ActualizarPisos(); Guardar(); }   // móvil
    }

    void OnApplicationQuit()
    {
        ActualizarPisos();
        Guardar();
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
    public void Comer(float cantidad = 0.5f)
    {
        comida = Mathf.Clamp01(comida + cantidad);
        ActualizarPisos();
        Guardar();
    }

    public void Beber(float cantidad = 0.5f)
    {
        agua = Mathf.Clamp01(agua + cantidad);
        ActualizarPisos();
        Guardar();
    }

    public void SetDurmiendo(bool valor) { durmiendo = valor; }   // solo se usa si no hay PerroDescanso

    // Úsalo solo para reiniciar la partida completa (botón "Nueva partida")
    public void ReiniciarProgreso()
    {
        minComida = minAgua = minSueno = 0f;
        comida = agua = sueno = 0f;
        PlayerPrefs.DeleteKey(Clave("comida"));
        PlayerPrefs.DeleteKey(Clave("agua"));
        PlayerPrefs.DeleteKey(Clave("sueno"));
        hayCambios = false;
        ActualizarBarras();
    }

    // ---- Persistencia ----
    string Clave(string barra) { return "Necesidades_" + nombreBase + "_" + barra; }

    void ActualizarPisos()
    {
        if (comida > minComida) { minComida = comida; hayCambios = true; }
        if (agua > minAgua) { minAgua = agua; hayCambios = true; }
        if (sueno > minSueno) { minSueno = sueno; hayCambios = true; }
    }

    void Guardar()
    {
        if (!persistir || string.IsNullOrEmpty(nombreBase)) return;
        PlayerPrefs.SetFloat(Clave("comida"), minComida);
        PlayerPrefs.SetFloat(Clave("agua"), minAgua);
        PlayerPrefs.SetFloat(Clave("sueno"), minSueno);
        PlayerPrefs.Save();
        hayCambios = false;
        tiempoGuardado = Time.time;
    }

    void Cargar()
    {
        if (!persistir) return;
        minComida = PlayerPrefs.GetFloat(Clave("comida"), 0f);
        minAgua = PlayerPrefs.GetFloat(Clave("agua"), 0f);
        minSueno = PlayerPrefs.GetFloat(Clave("sueno"), 0f);

        comida = Mathf.Max(comida, minComida);
        agua = Mathf.Max(agua, minAgua);
        sueno = Mathf.Max(sueno, minSueno);
    }

    // ---- Internos ----
    void Configurar(Slider s)
    {
        if (s == null) return;
        s.minValue = 0f;
        s.maxValue = 1f;
        s.wholeNumbers = false;
        s.interactable = false;

        if (estiloCepillado)
        {
            // Sin transición para que "no interactuable" no tiñe de gris el handle ni el relleno
            s.transition = Selectable.Transition.None;
            if (s.handleRect != null) s.handleRect.gameObject.SetActive(true);
        }
        else if (ocultarHandle && s.handleRect != null)
        {
            s.handleRect.gameObject.SetActive(false);
        }
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
        if (colorRojoAVerde && !estiloCepillado && s.fillRect != null)
        {
            var img = s.fillRect.GetComponent<Image>();
            if (img != null) img.color = Color.Lerp(Color.red, Color.green, valor);
        }
    }
}
