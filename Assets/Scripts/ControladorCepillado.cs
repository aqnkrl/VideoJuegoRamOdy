using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

// Ponlo en Ody. La barra sube solo cuando la punta del cepillo se MUEVE sobre Ody.
// Mientras lo cepillas, Ody cambia a los sprites de "cepillado" (con brillitos).
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

    [Header("Al llenarse la barra")]
    public UnityEvent alCompletarCepillado;

    bool cepillando;
    bool completado;
    float tiempoSinCepillar;
    float tiempoAnim;
    Sprite spriteNormal;

    void Start()
    {
        if (barraCepillado != null)
        {
            barraCepillado.minValue = 0f;
            barraCepillado.maxValue = 1f;
            barraCepillado.value = 0f;
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

            barraCepillado.value += cepillo.MovimientoEsteFrame * velocidad;
            AnimarCepillado();

            if (barraCepillado.value >= barraCepillado.maxValue)
                Completar();
        }
        else if (cepillando)
        {
            tiempoSinCepillar += Time.deltaTime;
            if (tiempoSinCepillar >= toleranciaSinCepillar) TerminarCepillado();
        }
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
        alCompletarCepillado?.Invoke();
    }
}