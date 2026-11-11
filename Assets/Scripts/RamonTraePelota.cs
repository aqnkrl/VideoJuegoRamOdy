using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RamonTraePelota : MonoBehaviour
{
    public float velocidad = 4f;
    public float distanciaRecoger = 0.3f;
    public Vector2 posicionPelotaAlLlevar = new Vector2(0.4f, 0f);
    public float tiempoMaximo = 15f;

    [Tooltip("Scripts de Ramón que se apagan mientras trae la pelota")]
    public string[] scriptsADesactivar = { "ControlRamon", "SeguirObjetivo" };

    [Header("Sprites al ir por la pelota")]
    public Sprite[] correrDerecha;
    public Sprite[] correrIzquierda;

    [Header("Sprites al llevar la pelota (opcional)")]
    public Sprite[] llevarDerecha;
    public Sprite[] llevarIzquierda;

    public float cuadrosPorSegundo = 10f;

    [Header("Efecto de Sonido (Pelota)")]
    public AudioSource fuenteAudio;
    public AudioClip sonidoPelota; // El chillido o sonido de la pelota al ir por ella

    private SpriteRenderer sprite;
    private Animator animador;
    private Rigidbody2D rb;
    private Vector2 posicionInicial;
    private bool ocupado = false;
    private bool llevandoPelota = false;
    private bool haciaIzquierda = false;
    private float reloj = 0f;
    private Transform pelotaLlevada;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        animador = GetComponentInChildren<Animator>();

        if (fuenteAudio == null)
        {
            fuenteAudio = GetComponent<AudioSource>();
            if (fuenteAudio == null) fuenteAudio = gameObject.AddComponent<AudioSource>();
        }
        fuenteAudio.playOnAwake = false;
    }

    void Start()
    {
        posicionInicial = Posicion();
    }

    Vector2 Posicion()
    {
        return rb != null ? rb.position : (Vector2)transform.position;
    }

    public void IrPorPelota(PelotaArrastrable pelota)
    {
        if (ocupado) return;
        StartCoroutine(Traer(pelota));
    }

    IEnumerator Traer(PelotaArrastrable pelota)
    {
        ocupado = true;
        pelota.bloqueada = true;

        List<Behaviour> apagados = new List<Behaviour>();
        foreach (string nombre in scriptsADesactivar)
        {
            Behaviour b = GetComponent(nombre) as Behaviour;
            if (b != null && b.enabled)
            {
                b.enabled = false;
                apagados.Add(b);
            }
        }
        if (rb != null) rb.linearVelocity = Vector2.zero;

        Sprite spriteOriginal = sprite != null ? sprite.sprite : null;
        if (animador != null) animador.enabled = false;

        // REPRODUCIR EL SONIDO DE LA PELOTA CUANDO RAMÓN SALE CORRIENDO POR ELLA
        if (fuenteAudio != null && sonidoPelota != null)
        {
            fuenteAudio.PlayOneShot(sonidoPelota);
        }

        // 1. Ir hasta la pelota
        llevandoPelota = false;
        yield return IrA(pelota.transform.position, distanciaRecoger);

        // 2. Recogerla
        Transform padreOriginal = pelota.transform.parent;
        pelota.transform.SetParent(transform);
        pelotaLlevada = pelota.transform;
        llevandoPelota = true;
        ColocarPelota();

        // 3. Volver al punto de inicio
        yield return IrA(posicionInicial, 0.05f);

        // 4. Soltarla
        pelota.transform.SetParent(padreOriginal, true);
        pelotaLlevada = null;
        llevandoPelota = false;

        // --- SUMAR 1 AL SLIDER DE PELOTA ---
        ControladorJardin jardin = Object.FindFirstObjectByType<ControladorJardin>();
        if (jardin != null)
        {
            jardin.SumarProgresoPelota();
        }

        if (sprite != null) sprite.sprite = spriteOriginal;
        if (animador != null) animador.enabled = true;

        foreach (Behaviour b in apagados) b.enabled = true;
        pelota.bloqueada = false;
        ocupado = false;
    }

    IEnumerator IrA(Vector2 destino, float margen)
    {
        float t = 0f;
        while (Vector2.Distance(Posicion(), destino) > margen && t < tiempoMaximo)
        {
            Vector2 actual = Posicion();
            Vector2 nueva = Vector2.MoveTowards(actual, destino, velocidad * Time.fixedDeltaTime);

            if (Mathf.Abs(nueva.x - actual.x) > 0.001f)
                haciaIzquierda = nueva.x < actual.x;

            if (rb != null) rb.MovePosition(nueva);
            else transform.position = nueva;

            Animar();
            ColocarPelota();

            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    Sprite[] ElegirCuadros()
    {
        Sprite[] lista = null;
        if (llevandoPelota)
            lista = haciaIzquierda ? llevarIzquierda : llevarDerecha;
        if (lista == null || lista.Length == 0)
            lista = haciaIzquierda ? correrIzquierda : correrDerecha;
        return lista;
    }

    void Animar()
    {
        if (sprite == null) return;
        Sprite[] cuadros = ElegirCuadros();
        if (cuadros == null || cuadros.Length == 0) return;

        reloj += Time.fixedDeltaTime;
        int i = Mathf.FloorToInt(reloj * cuadrosPorSegundo) % cuadros.Length;
        sprite.sprite = cuadros[i];
    }

    void ColocarPelota()
    {
        if (pelotaLlevada == null) return;
        float x = haciaIzquierda ? -posicionPelotaAlLlevar.x : posicionPelotaAlLlevar.x;
        pelotaLlevada.localPosition = new Vector3(x, posicionPelotaAlLlevar.y, 0f);
    }
}
