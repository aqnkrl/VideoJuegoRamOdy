using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Ponlo en el objeto del cepillo (necesita SpriteRenderer y un Collider2D).
// Se agarra con el mouse (o el dedo) y se suelta en cualquier parte.
// Funciona con el Input Manager clásico y con el Input System nuevo.
public class CepilloArrastrable : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Hijo vacío colocado sobre las cerdas del cepillo.")]
    public Transform punta;

    [Header("Ajustes")]
    public int ordenDibujoAlArrastrar = 50;
    public bool volverAlSoltar = false;

    public bool EstaArrastrando { get; private set; }
    public float MovimientoEsteFrame { get; private set; }

    Camera cam;
    SpriteRenderer sr;
    Collider2D col;
    Vector3 offset, posAnterior, posInicial;
    int ordenOriginal;

    void Awake()
    {
        cam = Camera.main;
        if (cam == null && Camera.allCamerasCount > 0) cam = Camera.allCameras[0];
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        posInicial = transform.position;
        if (sr != null) ordenOriginal = sr.sortingOrder;
        if (punta == null) punta = transform;

        if (col == null) Debug.LogError("CepilloArrastrable: falta un Collider2D en " + name, this);
        if (cam == null) Debug.LogError("CepilloArrastrable: no encuentro la cámara (tag MainCamera).", this);
    }

    // ---------- entrada (clásica o nueva) ----------
    Vector2 PosPantalla()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
#else
        return Input.mousePosition;
#endif
    }
    bool ClicAbajo()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#else
        return Input.GetMouseButtonDown(0);
#endif
    }
    bool ClicMantenido()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null && Mouse.current.leftButton.isPressed;
#else
        return Input.GetMouseButton(0);
#endif
    }

    Vector3 MundoDelMouse()
    {
        Vector3 p = cam.ScreenToWorldPoint(PosPantalla());
        p.z = 0f;
        return p;
    }

    // ---------- lógica ----------
    void Update()
    {
        if (cam == null || col == null) return;

        if (!EstaArrastrando)
        {
            MovimientoEsteFrame = 0f;
            if (ClicAbajo())
            {
                Vector3 m = MundoDelMouse();
                // Revisa todos los colliders bajo el mouse; así otro collider encima no estorba.
                foreach (Collider2D c in Physics2D.OverlapPointAll(m))
                {
                    if (c == col) { Empezar(m); break; }
                }
            }
            return;
        }

        if (!ClicMantenido()) { Soltar(); return; }

        Vector3 nueva = MundoDelMouse() + offset;
        nueva.z = transform.position.z;
        transform.position = nueva;

        MovimientoEsteFrame = Vector3.Distance(nueva, posAnterior);
        posAnterior = nueva;
    }

    void Empezar(Vector3 mouseMundo)
    {
        EstaArrastrando = true;
        offset = transform.position - mouseMundo;
        posAnterior = transform.position;
        if (sr != null) sr.sortingOrder = ordenDibujoAlArrastrar;
    }

    void Soltar()
    {
        EstaArrastrando = false;
        MovimientoEsteFrame = 0f;
        if (sr != null) sr.sortingOrder = ordenOriginal;
        if (volverAlSoltar) transform.position = posInicial;
    }
}
