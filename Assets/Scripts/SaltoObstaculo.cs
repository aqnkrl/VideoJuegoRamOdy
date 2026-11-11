using UnityEngine;
using System.Collections;

public class SaltoRamonCompleto : MonoBehaviour
{
    [Header("Sprites de Ramón")]
    public Sprite spriteNormal;
    public Sprite spriteSaltando;

    private SpriteRenderer spriteRenderer;

    [Header("Configuración del Salto")]
    public float alturaSalto = 2f;
    public float distanciaAvance = 2.5f;
    public float duracionSalto = 0.5f;

    [Header("Efecto de Sonido (Salto)")]
    public AudioSource fuenteAudio;
    public AudioClip sonidoSalto;

    private bool estaSaltando = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteNormal == null && spriteRenderer != null)
        {
            spriteNormal = spriteRenderer.sprite;
        }

        if (fuenteAudio == null)
        {
            fuenteAudio = GetComponent<AudioSource>();
            if (fuenteAudio == null) fuenteAudio = gameObject.AddComponent<AudioSource>();
        }
        fuenteAudio.playOnAwake = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !estaSaltando)
        {
            StartCoroutine(RutinaSaltoConMovimiento());
        }
    }

    public void EjecutarSaltoExterno()
    {
        if (!estaSaltando)
        {
            StartCoroutine(RutinaSaltoConMovimiento());
        }
    }

    IEnumerator RutinaSaltoConMovimiento()
    {
        estaSaltando = true;

        // REPRODUCIR EL SONIDO DE SALTO JUSTO CUANDO RAMÓN COMIENZA A ELEVARSE
        if (fuenteAudio != null && sonidoSalto != null)
        {
            fuenteAudio.PlayOneShot(sonidoSalto);
        }

        if (spriteRenderer != null && spriteSaltando != null)
        {
            spriteRenderer.sprite = spriteSaltando;
        }

        Vector3 posInicial = transform.position;
        float direccionX = (spriteRenderer != null && spriteRenderer.flipX) ? -distanciaAvance : distanciaAvance;
        Vector3 posDestino = posInicial + new Vector3(direccionX, 0, 0);

        float tiempo = 0;

        while (tiempo < duracionSalto)
        {
            float progreso = tiempo / duracionSalto;
            float alturaActual = Mathf.Sin(progreso * Mathf.PI) * alturaSalto;

            transform.position = Vector3.Lerp(posInicial, posDestino, progreso) + new Vector3(0, alturaActual, 0);

            tiempo += Time.deltaTime;
            yield return null;
        }

        transform.position = posDestino;

        if (spriteRenderer != null && spriteNormal != null)
        {
            spriteRenderer.sprite = spriteNormal;
        }

        // --- SUMAR 1 AL SLIDER DE SALTO ---
        ControladorJardin jardin = Object.FindFirstObjectByType<ControladorJardin>();
        if (jardin != null)
        {
            jardin.SumarProgresoSalto();
        }

        estaSaltando = false;
    }
}
