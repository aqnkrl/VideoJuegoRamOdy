using System.Collections;
using UnityEngine;

public enum TipoPerro { Ramon, Ody, Personalizado }

// Va en cada perro (Ramón y Ody).
public class PerroDescanso : MonoBehaviour
{
    [Header("Qué perro es (define sus teclas)")]
    [Tooltip("Ramón: E para acostarse, W A S D para levantarse. Ody: O para acostarse, las flechas para levantarse.")]
    public TipoPerro perro = TipoPerro.Ramon;

    [Header("Solo si eliges 'Personalizado'")]
    public KeyCode teclaAcostar = KeyCode.E;
    public KeyCode[] teclasLevantar;
    public bool acostarseAlTocar = false;        // true = se acuesta solo al tocar la cama

    [Header("Referencias")]
    public SpriteRenderer sr;                    // el SpriteRenderer del perro
    public MonoBehaviour[] scriptsADesactivar;   // ControlRamon / ControlJugador / SeguirObjetivo...
    public Rigidbody2D rb;                       // opcional
    public Animator animador;                    // opcional (si usas Animator)

    [Header("Sprites (los solo_...)")]
    public Sprite spriteAcostado;                // solo_xxx_acostado
    public Sprite[] framesDurmiendo;             // solo_xxx_durmiendo_1 y _2
    public float segundosParaDormir = 2f;        // tiempo acostado antes de dormirse
    public float velocidadRespiracion = 0.6f;    // segundos entre durmiendo_1 y _2

    [Header("Ayuda")]
    public bool mostrarMensajes = true;          // avisa en la consola por qué no se acuesta

    [HideInInspector] public CamaPerro camaCercana;
    public bool Acostado { get; private set; }

    bool flipAnterior;
    Sprite spriteAnterior;
    int ordenAnterior;
    int capaAnterior;
    float bloqueoHasta;
    Coroutine rutina;

    void Awake()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        // Si el objeto se llama Ody o Ramon, elige las teclas correctas aunque el desplegable esté mal
        if (perro != TipoPerro.Personalizado)
        {
            string n = name.ToLower();
            if (n.Contains("ody")) perro = TipoPerro.Ody;
            else if (n.Contains("ramon") || n.Contains("ramón")) perro = TipoPerro.Ramon;
        }

        switch (perro)
        {
            case TipoPerro.Ramon:
                teclaAcostar = KeyCode.E;
                teclasLevantar = new[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
                break;
            case TipoPerro.Ody:
                teclaAcostar = KeyCode.O;
                teclasLevantar = new[] { KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow };
                break;
                // Personalizado: usa lo que pusiste en el Inspector
        }

        if (mostrarMensajes)
            Debug.Log(name + " es " + perro + " y se acuesta con la tecla " + teclaAcostar);
    }

    void Update()
    {
        if (!Acostado)
        {
            bool quiere = acostarseAlTocar || Input.GetKeyDown(teclaAcostar);

            if (camaCercana != null && quiere)
            {
                Acostarse(camaCercana);
            }
            else if (mostrarMensajes && camaCercana == null && Input.GetKeyDown(teclaAcostar))
            {
                Debug.Log(name + ": presionaste " + teclaAcostar + " pero no hay una cama cerca. " +
                          "Revisa: Rigidbody2D y Collider2D en el perro, Is Trigger en la cama y 'Dueno Exclusivo'.");
            }
        }
        else
        {
            if (Input.GetKeyDown(teclaAcostar)) { Levantarse(); return; }
            foreach (var t in teclasLevantar)
                if (Input.GetKeyDown(t)) { Levantarse(); return; }
        }
    }

    void Acostarse(CamaPerro cama)
    {
        if (Time.time < bloqueoHasta) return;
        if (!cama.PuedeUsar(this))
        {
            if (mostrarMensajes) Debug.Log(name + ": esta cama es de otro perro.");
            return;
        }
        Acostado = true;

        spriteAnterior = sr.sprite;
        ordenAnterior = sr.sortingOrder;
        capaAnterior = sr.sortingLayerID;

        foreach (var s in scriptsADesactivar) if (s != null) s.enabled = false;
        if (animador != null) animador.enabled = false;
        if (rb != null)
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector2.zero;
#else
            rb.velocity = Vector2.zero;
#endif
            rb.simulated = false;
        }

        // El sprite durmiendo mira a un solo lado: quitar el volteo mientras duerme
        flipAnterior = sr.flipX;
        sr.flipX = false;

        // Centrar el perro en la cama (los sprites solo_ ya están alineados con la cama)
        Transform punto = cama.puntoDescanso != null ? cama.puntoDescanso : cama.transform;
        transform.position = new Vector3(punto.position.x, punto.position.y, transform.position.z);

        // Dibujarlo encima de la cama
        if (cama.SpriteCama != null)
        {
            sr.sortingLayerID = cama.SpriteCama.sortingLayerID;
            sr.sortingOrder = cama.SpriteCama.sortingOrder + 1;
        }

        if (rutina != null) StopCoroutine(rutina);
        rutina = StartCoroutine(Dormir());
    }

    public void Levantarse()
    {
        if (!Acostado) return;
        Acostado = false;
        if (rutina != null) StopCoroutine(rutina);

        sr.flipX = flipAnterior;
        sr.sprite = spriteAnterior;
        sr.sortingOrder = ordenAnterior;
        sr.sortingLayerID = capaAnterior;

        if (rb != null) rb.simulated = true;
        if (animador != null) animador.enabled = true;
        foreach (var s in scriptsADesactivar) if (s != null) s.enabled = true;

        bloqueoHasta = Time.time + 1f; // evita volver a acostarse al instante
    }

    IEnumerator Dormir()
    {
        sr.sprite = spriteAcostado;
        yield return new WaitForSeconds(segundosParaDormir);

        int i = 0;
        while (true)
        {
            if (framesDurmiendo != null && framesDurmiendo.Length > 0)
                sr.sprite = framesDurmiendo[i % framesDurmiendo.Length];
            i++;
            yield return new WaitForSeconds(velocidadRespiracion);
        }
    }
}