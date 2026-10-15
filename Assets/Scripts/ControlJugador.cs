using UnityEngine;

public class ControlJugador : MonoBehaviour
{
    public float velocidad = 4f;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 movimiento;

    // Definimos los estados del baño
    public enum EstadoMascota { Sucio, Enjabonado, Mojado, Limpio }
    [Header("Estado Actual de Ody")]
    public EstadoMascota estadoActual = EstadoMascota.Sucio;

    [Header("1. Sprites Ody Sucio")]
    public Sprite sucioFrente;
    public Sprite sucioEspalda;
    public Sprite sucioPerfil;

    [Header("2. Sprites Ody Enjabonado (Espuma)")]
    public Sprite espumaFrente;
    public Sprite espumaEspalda;
    public Sprite espumaPerfil;

    [Header("3. Sprites Ody Mojado")]
    public Sprite mojadoFrente;
    public Sprite mojadoEspalda;
    public Sprite mojadoPerfil;

    [Header("4. Sprites Ody Limpio")]
    public Sprite limpioFrente;
    public Sprite limpioEspalda;
    public Sprite limpioPerfil;

    // Sprites activos según la dirección
    private Sprite spriteFrente;
    private Sprite spriteEspalda;
    private Sprite spritePerfil;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        ActualizarConjuntoSprites();
    }

    void Update()
    {
        // Leemos las flechas del teclado
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

    // Llama a esta función cuando Ody cambie de estado en el baño
    public void CambiarEstado(EstadoMascota nuevoEstado)
    {
        estadoActual = nuevoEstado;
        ActualizarConjuntoSprites();
    }

    void ActualizarConjuntoSprites()
    {
        switch (estadoActual)
        {
            case EstadoMascota.Sucio:
                spriteFrente = sucioFrente;
                spriteEspalda = sucioEspalda;
                spritePerfil = sucioPerfil;
                break;
            case EstadoMascota.Enjabonado:
                spriteFrente = espumaFrente;
                spriteEspalda = espumaEspalda;
                spritePerfil = espumaPerfil;
                break;
            case EstadoMascota.Mojado:
                spriteFrente = mojadoFrente;
                spriteEspalda = mojadoEspalda;
                spritePerfil = mojadoPerfil;
                break;
            case EstadoMascota.Limpio:
                spriteFrente = limpioFrente;
                spriteEspalda = limpioEspalda;
                spritePerfil = limpioPerfil;
                break;
        }

        // Forzamos actualizar el sprite actual si está quieto
        if (sr != null && spriteFrente != null)
        {
            sr.sprite = spriteFrente;
        }
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