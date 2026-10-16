using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Ponlo en cada obstaculo (necesita un Collider2D).
// Al hacer click / tocar el obstaculo, Ramon va hasta el y lo brinca.
[RequireComponent(typeof(Collider2D))]
public class ObstaculoSaltable : MonoBehaviour
{
    public RamonSalto ramon;

    [Tooltip("A que distancia del centro del obstaculo Ramon se detiene a tomar impulso")]
    public float distanciaDespegue = 1.2f;

    [Tooltip("A que distancia del centro cae del otro lado")]
    public float distanciaAterrizaje = 1.2f;

    Collider2D col;
    Camera cam;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        cam = Camera.main;
        if (ramon == null) ramon = FindFirstObjectByType<RamonSalto>();
    }

    void Update()
    {
        if (ramon == null || cam == null) return;
        if (!ClickEsteFrame(out Vector2 pantalla)) return;

        Vector2 mundo = cam.ScreenToWorldPoint(pantalla);
        if (col.OverlapPoint(mundo))
            ramon.Saltar(this);
    }

    bool ClickEsteFrame(out Vector2 pos)
    {
#if ENABLE_INPUT_SYSTEM
        var m = Mouse.current;
        if (m != null && m.leftButton.wasPressedThisFrame)
        {
            pos = m.position.ReadValue();
            return true;
        }
        var t = Touchscreen.current;
        if (t != null && t.primaryTouch.press.wasPressedThisFrame)
        {
            pos = t.primaryTouch.position.ReadValue();
            return true;
        }
#else
        if (Input.GetMouseButtonDown(0))
        {
            pos = Input.mousePosition;
            return true;
        }
#endif
        pos = default;
        return false;
    }
}