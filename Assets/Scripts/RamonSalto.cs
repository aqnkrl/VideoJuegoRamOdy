using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RamonSalto : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 4f;
    public float alturaSalto = 0.8f;
    public float duracionSalto = 0.6f;

    [Header("Referencias")]
    public SpriteRenderer sr;
    public Animator animator;
    [Tooltip("Opcional: hijo que lleva el sprite. Si lo dejas vacio, se levanta todo Ramon.")]
    public Transform visual;

    [Header("Sprites del salto")]
    public Sprite[] framesSaltoDerecha;
    public Sprite[] framesSaltoIzquierda;

    [Header("Scripts de Ramon que se apagan durante el salto")]
    public string[] scriptsADesactivar = { "ControlRamon", "SeguirObjetivo" };

    bool saltando;
    bool mirandoDerecha;

    void Awake()
    {
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    public void Saltar(ObstaculoSaltable obstaculo)
    {
        if (!saltando) StartCoroutine(Rutina(obstaculo));
    }

    IEnumerator Rutina(ObstaculoSaltable o)
    {
        saltando = true;

        var apagados = new List<MonoBehaviour>();
        foreach (string n in scriptsADesactivar)
        {
            var mb = GetComponent(n) as MonoBehaviour;
            if (mb != null && mb.enabled) { mb.enabled = false; apagados.Add(mb); }
        }

        Vector2 centro = o.transform.position;
        Vector2 dir = centro - (Vector2)transform.position;
        dir = Mathf.Abs(dir.x) >= Mathf.Abs(dir.y)
            ? new Vector2(Mathf.Sign(dir.x), 0f)
            : new Vector2(0f, Mathf.Sign(dir.y));
        if (dir.x != 0f) mirandoDerecha = dir.x > 0f;

        Vector2 despegue = centro - dir * o.distanciaDespegue;
        Vector2 aterrizaje = centro + dir * o.distanciaAterrizaje;

        while (Vector2.Distance(transform.position, despegue) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, despegue, velocidad * Time.deltaTime);
            yield return null;
        }
        transform.position = despegue;

        if (animator != null) animator.enabled = false;
        var colisionadores = GetComponents<Collider2D>();
        foreach (var c in colisionadores) c.enabled = false;

        Sprite[] frames = mirandoDerecha ? framesSaltoDerecha : framesSaltoIzquierda;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duracionSalto;
            float k = Mathf.Clamp01(t);
            float arco = Mathf.Sin(Mathf.PI * k) * alturaSalto;

            Vector2 basePos = Vector2.Lerp(despegue, aterrizaje, k);
            if (visual != null)
            {
                transform.position = basePos;
                visual.localPosition = new Vector3(0f, arco, 0f);
            }
            else
            {
                transform.position = new Vector3(basePos.x, basePos.y + arco, transform.position.z);
            }

            if (sr != null && frames != null && frames.Length > 0)
                sr.sprite = frames[Mathf.Min(frames.Length - 1, (int)(k * frames.Length))];

            yield return null;
        }

        transform.position = new Vector3(aterrizaje.x, aterrizaje.y, transform.position.z);
        if (visual != null) visual.localPosition = Vector3.zero;
        foreach (var c in colisionadores) c.enabled = true;
        if (animator != null) animator.enabled = true;
        foreach (var mb in apagados) mb.enabled = true;

        // --- LLENAR SLIDER DE SALTO EN EL JARDÍN ---
        ControladorJardin jardin = Object.FindFirstObjectByType<ControladorJardin>();
        if (jardin != null)
        {
            jardin.SumarProgresoSalto();
        }

        saltando = false;
    }
}