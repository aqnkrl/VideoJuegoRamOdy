using UnityEngine;

public class ControlRamon : MonoBehaviour
{
    public float velocidad = 4f;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 movimiento;

    [Header("Sprites de Dirección Ramón")]
    public Sprite spriteFrente;
    public Sprite spriteEspalda;
    public Sprite spritePerfil;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Leemos directamente las teclas W, A, S, D en Update
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W)) moveY = 1f;
        if (Input.GetKey(KeyCode.S)) moveY = -1f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = 1f;

        movimiento = new Vector2(moveX, moveY).normalized;

        CambiarSprite();
    }

    void FixedUpdate()
    {
        // Asignamos la velocidad directamente a la física del Rigidbody
        rb.linearVelocity = movimiento * velocidad;
    }

    void CambiarSprite()
    {
        if (sr == null) return;

        if (movimiento.y > 0)
        {
            if (spriteEspalda != null) sr.sprite = spriteEspalda;
        }
        else if (movimiento.y < 0)
        {
            if (spriteFrente != null) sr.sprite = spriteFrente;
        }
        else if (movimiento.x > 0)
        {
            if (spritePerfil != null)
            {
                sr.sprite = spritePerfil;
                sr.flipX = false;
            }
        }
        else if (movimiento.x < 0)
        {
            if (spritePerfil != null)
            {
                sr.sprite = spritePerfil;
                sr.flipX = true;
            }
        }
    }
}