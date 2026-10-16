using UnityEngine;

public class PelotaArrastrable : MonoBehaviour
{
    public RamonTraePelota ramon;

    [HideInInspector] public bool bloqueada = false;

    private Collider2D col;
    private Camera cam;
    private bool arrastrando = false;
    private Vector2 desfase;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        cam = Camera.main;
    }

    void Update()
    {
        if (bloqueada) { arrastrando = false; return; }

        if (!arrastrando && BotonPresionado())
        {
            Vector2 p = PosicionMundo();
            if (col.OverlapPoint(p))
            {
                arrastrando = true;
                desfase = (Vector2)transform.position - p;
            }
        }
        else if (arrastrando && BotonMantenido())
        {
            Vector2 p = PosicionMundo() + desfase;
            transform.position = new Vector3(p.x, p.y, transform.position.z);
        }
        else if (arrastrando)
        {
            arrastrando = false;
            if (ramon != null) ramon.IrPorPelota(this);
        }
    }

    bool BotonPresionado()
    {
#if ENABLE_INPUT_SYSTEM
        return UnityEngine.InputSystem.Mouse.current != null &&
               UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame;
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

    bool BotonMantenido()
    {
#if ENABLE_INPUT_SYSTEM
        return UnityEngine.InputSystem.Mouse.current != null &&
               UnityEngine.InputSystem.Mouse.current.leftButton.isPressed;
#else
        return Input.GetMouseButton(0);
#endif
    }

    Vector2 PosicionMundo()
    {
#if ENABLE_INPUT_SYSTEM
        Vector2 pantalla = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
#else
        Vector2 pantalla = Input.mousePosition;
#endif
        Vector3 mundo = cam.ScreenToWorldPoint(new Vector3(pantalla.x, pantalla.y, -cam.transform.position.z));
        return new Vector2(mundo.x, mundo.y);
    }
}