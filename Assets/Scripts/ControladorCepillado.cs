using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

// Ponlo en Ody. La barra sube solo cuando la punta del cepillo se MUEVE sobre Ody.
// Mientras lo cepillas, Ody cambia a los sprites de "cepillado" (con brillitos).
// La barra empieza vacía, nunca baja y su valor se conserva entre escenas.
public class ControladorCepillado : MonoBehaviour
{
    [Header("Referencias")]
    public CepilloArrastrable cepillo;
    public Slider barraCepillado;            // el slider nuevo del Canvas (BarraCepilladoOdy)
    public Collider2D colliderOdy;           // el collider de Ody
    public SpriteRenderer spriteOdy;         // el SpriteRenderer de Ody
    public Behaviour controlJugador;         // arrastra aquí el script ControlJugador de Ody

    [Header("Sprites de Ody cepillado")]
    public Sprite[] framesCepillado;         // ody_cepillado_1 a _4
    public float fps = 6f;

    [Header("Ajustes")]
    [Tooltip("Cuánto sube la barra (0 a 1) por cada unidad de movimiento del cepillo sobre Ody.")]
    public float velocidad = 0.25f;
    [Tooltip("Mínimo de movimiento por frame para contar como cepillado.")]
    public float movimientoMinimo = 0.002f;
    [Tooltip("Segundos de tolerancia antes de volver al sprite normal.")]
    public float toleranciaSinCepillar = 0.15f;

    [Header("Persistencia")]
    [Tooltip("Ody o Ramon. Vacío = usa el nombre de este objeto. Debe ser igual en todas las escenas.")]
    public string nombreBase = "Ody";
    public bool persistir = true;
    [Tooltip("Al iniciar el juego, la barra empieza vacía aunque haya datos guardados. Al cambiar de escena sí se conserva.")]
    public bool empezarVacioCadaSesion = true;
    [Tooltip("Si la barra ya estaba llena al cargar la escena, vuelve a disparar 'Al Completar Cepillado'.")]
    public bool dispararEventoAlCargarCompleto = false;

    [Header("Al llenarse la barra")]
    public UnityEvent alCompletarCepillado;

    // Perros que ya aparecieron en esta partida (se limpia al dar Play / abrir el juego)
    static readonly System.Collections.Generic.HashSet<string> sesionesIniciadas =
        new System.Collections.Generic.HashSet<string>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetEstatico() { sesionesIniciadas.Clear(); }

    float progreso;          // valor real de la barra: solo sube, nunca baja
    bool hayCambios;
    float tiempoGuardado;
    const float INTERVALO_GUARDADO = 1f;

    bool cepillando;
    bool completado;
    float tiempoSinCepillar;
    float tiempoAnim;
    Sprite spriteNormal;

    string Clave { get { return "Cepillado_" + nombreBase; } }

    void Start()
    {
        if (string.IsNullOrEmpty(nombreBase)) nombreBase = gameObject.name;

        if (barraCepillado != null)
        {
            barraCepillado.minValue = 0f;
            barraCepillado.maxValue = 1f;
        }

        // Primera vez en esta partida: borra lo guardado para que empiece vacía
        if (persistir && empezarVacioCadaSesion && sesionesIniciadas.Add(nombreBase))
            PlayerPrefs.DeleteKey(Clave);

        // Sin datos guardados = empieza vacía; con datos = continúa donde se quedó
        progreso = persistir ? Mathf.Clamp01(PlayerPrefs.GetFloat(Clave, 0f)) : 0f;
        Pintar();

        if (progreso >= 1f)
        {
            completado = true;
            if (dispararEventoAlCargarCompleto) alCompletarCepillado?.Invoke();
        }
    }

    void Update()
    {
        if (completado || cepillo == null) return;

        bool encima = cepillo.EstaArrastrando &&
                      colliderOdy.OverlapPoint(cepillo.punta.position);
        bool frotando = encima && cepillo.MovimientoEsteFrame > movimientoMinimo;

        if (frotando)
        {
            tiempoSinCepillar = 0f;
            if (!cepillando) EmpezarCepillado();

            // Solo suma: el progreso nunca disminuye
            progreso = Mathf.Clamp01(progreso + cepillo.MovimientoEsteFrame * velocidad);
            hayCambios = true;
            Pintar();
            AnimarCepillado();

            if (progreso >= 1f)
                Completar();
        }
        else if (cepillando)
        {
            tiempoSinCepillar += Time.deltaTime;
            if (tiempoSinCepillar >= toleranciaSinCepillar) TerminarCepillado();
        }

        if (persistir && hayCambios && Time.time - tiempoGuardado >= INTERVALO_GUARDADO)
            Guardar();
    }

    void EmpezarCepillado()
    {
        cepillando = true;
        tiempoAnim = 0f;
        spriteNormal = spriteOdy.sprite;
        if (controlJugador != null) controlJugador.enabled = false; // para que no pise los sprites
    }

    void AnimarCepillado()
    {
        if (framesCepillado == null || framesCepillado.Length == 0) return;
        tiempoAnim += Time.deltaTime * fps;
        int i = (int)tiempoAnim % framesCepillado.Length;
        spriteOdy.sprite = framesCepillado[i];
    }

    void TerminarCepillado()
    {
        cepillando = false;
        if (spriteNormal != null) spriteOdy.sprite = spriteNormal;
        if (controlJugador != null) controlJugador.enabled = true;
    }

    void Completar()
    {
        completado = true;
        TerminarCepillado();
        Guardar();
        alCompletarCepillado?.Invoke();
    }

    // Úsalo solo para "Nueva partida"
    public void ReiniciarProgreso()
    {
        progreso = 0f;
        completado = false;
        hayCambios = false;
        PlayerPrefs.DeleteKey(Clave);
        Pintar();
    }

    // ---- Internos ----
    void Pintar()
    {
        if (barraCepillado != null) barraCepillado.value = progreso;
    }

    void Guardar()
    {
        if (!persistir) return;
        PlayerPrefs.SetFloat(Clave, progreso);
        PlayerPrefs.Save();
        hayCambios = false;
        tiempoGuardado = Time.time;
    }

    void OnDisable()
    {
        // Se ejecuta al cambiar de escena
        if (cepillando) TerminarCepillado();
        if (hayCambios) Guardar();
    }

    void OnApplicationPause(bool pausa) { if (pausa && hayCambios) Guardar(); }
    void OnApplicationQuit() { if (hayCambios) Guardar(); }
}
