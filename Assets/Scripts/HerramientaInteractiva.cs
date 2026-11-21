using UnityEngine;

public class HerramientaInteractiva : MonoBehaviour
{
    [Header("Escribe exactamente: Jabon, Regadera o Secador")]
    public string nombreHerramienta;
    public ControladorBano controlador;

    private Vector3 posicionInicial;
    private bool estaArrastrando = false;
    private Camera camaraPrincipal;
    private Rigidbody2D rb2d;
    private bool estaTocandoOdy = false;

    [Header("Efectos de Sonido (SFX en Bucle)")]
    public AudioSource fuenteAudio;    // El componente AudioSource de esta herramienta
    public AudioClip sonidoHerramienta; // El sonido de jabón, agua o secador

    void Start()
    {
        posicionInicial = transform.position;
        camaraPrincipal = Camera.main;

        rb2d = GetComponent<Rigidbody2D>();
        if (rb2d == null) rb2d = gameObject.AddComponent<Rigidbody2D>();
        rb2d.bodyType = RigidbodyType2D.Kinematic;

        BoxCollider2D colisionador = GetComponent<BoxCollider2D>();
        if (colisionador == null) colisionador = gameObject.AddComponent<BoxCollider2D>();
        colisionador.isTrigger = true;

        // Configurar AudioSource automáticamente
        if (fuenteAudio == null)
        {
            fuenteAudio = GetComponent<AudioSource>();
            if (fuenteAudio == null)
            {
                fuenteAudio = gameObject.AddComponent<AudioSource>();
            }
        }

        fuenteAudio.clip = sonidoHerramienta;
        fuenteAudio.loop = true; // Hacemos que el sonido sea continuo mientras se usa
        fuenteAudio.playOnAwake = false;
    }

    private Camera ObtenerCamara()
    {
        if (camaraPrincipal == null)
        {
            camaraPrincipal = Camera.main;
        }
        return camaraPrincipal;
    }

    void OnMouseDown()
    {
        estaArrastrando = true;
    }

    void OnMouseDrag()
    {
        Camera cam = ObtenerCamara();
        if (estaArrastrando && cam != null)
        {
            Vector3 posicionMouse = cam.ScreenToWorldPoint(Input.mousePosition);
            posicionMouse.z = transform.position.z;
            transform.position = posicionMouse;
        }
    }

    void OnMouseUp()
    {
        DetenerSonido();
        estaArrastrando = false;

        // Si soltaste la herramienta justo encima de Ody, aplica progreso adicional de forma estable
        if (estaTocandoOdy && controlador != null)
        {
            float pasoAlSoltar = controlador.valorMaximo * 0.5f;
            controlador.InteractuarConHerramienta(nombreHerramienta, pasoAlSoltar);
        }

        transform.position = posicionInicial;
        estaTocandoOdy = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<ControladorJugador>() != null)
        {
            estaTocandoOdy = true;

            if (controlador != null && estaArrastrando)
            {
                controlador.InteractuarConHerramienta(nombreHerramienta);

                // Reproducir el sonido continuo mientras estás tallando a Ody
                if (fuenteAudio != null && !fuenteAudio.isPlaying && sonidoHerramienta != null)
                {
                    fuenteAudio.Play();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<ControladorJugador>() != null)
        {
            estaTocandoOdy = false;
            DetenerSonido(); // Si separas la herramienta de Ody, el sonido se corta al instante
        }
    }

    private void DetenerSonido()
    {
        if (fuenteAudio != null && fuenteAudio.isPlaying)
        {
            fuenteAudio.Stop();
        }
    }

    void Update()
    {
        // Si sueltas el clic del ratón en cualquier momento, aseguramos que se apague el audio
        if (!Input.GetMouseButton(0) && fuenteAudio != null && fuenteAudio.isPlaying)
        {
            DetenerSonido();
        }
    }
}