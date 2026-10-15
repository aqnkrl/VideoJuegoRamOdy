using UnityEngine;

public class ControlJugadorN1 : MonoBehaviour
{
    public float velocidad = 4f;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 movimiento;

    [Header("Sprites de Dirección Ody (Nivel 1)")]
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
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.UpArrow)) moveY = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) moveY = -1f;
        if (Input.GetKey(KeyCode.LeftArrow)) moveX = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) moveX = 1f;

        movimiento = new Vector2(moveX, moveY).normalized;

        CambiarSprite();
    }

    void FixedUpdate()
    {
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