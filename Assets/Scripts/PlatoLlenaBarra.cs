using UnityEngine;

public class PlatoLlenaBarra : MonoBehaviour
{
    public enum Tipo { Auto, Comida, Agua }

    [Header("Normalmente no hay que tocar nada: se detecta por el nombre del plato")]
    public Tipo tipo = Tipo.Auto;            // Auto: si el nombre dice "Agua" o "Comida"
    public NecesidadesPerro perro;           // Auto: si el nombre termina en Ody o Ramon
    [Range(0f, 1f)] public float cantidad = 0.5f;

    [Header("Solo si tu comida/agua es un objeto aparte que se activa")]
    public GameObject objetoLleno;

    SpriteRenderer sr;
    Sprite spriteInicial;
    bool estabaLleno;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null && objetoLleno == null)
        {
            Debug.LogError("[PlatoLlenaBarra] '" + name + "' no tiene Sprite Renderer.", this);
            enabled = false;
            return;
        }
        if (sr != null) spriteInicial = sr.sprite;

        string n = name.ToLower();

        if (tipo == Tipo.Auto)
        {
            if (n.Contains("agua")) tipo = Tipo.Agua;
            else if (n.Contains("comida") || n.Contains("croqueta")) tipo = Tipo.Comida;
            else Debug.LogError("[PlatoLlenaBarra] No sé si '" + name + "' es de comida o agua. Elige el Tipo a mano.", this);
        }

        if (perro == null)
        {
            foreach (var p in FindObjectsByType<NecesidadesPerro>(FindObjectsSortMode.None))
                if (n.Contains(p.name.ToLower())) perro = p;
        }
        if (perro == null)
            Debug.LogError("[PlatoLlenaBarra] '" + name + "' no tiene perro. Arrástralo a 'Perro'.", this);
        else
            Debug.Log("[PlatoLlenaBarra] '" + name + "' → " + tipo + " de " + perro.name, this);

        estabaLleno = EstaLleno();
    }

    bool EstaLleno()
    {
        if (objetoLleno != null) return objetoLleno.activeInHierarchy;
        return sr != null && sr.sprite != spriteInicial;
    }

    void Update()
    {
        bool lleno = EstaLleno();
        if (lleno && !estabaLleno) Aplicar();
        estabaLleno = lleno;
    }

    void Aplicar()
    {
        if (perro == null) return;
        if (tipo == Tipo.Comida) perro.Comer(cantidad);
        else if (tipo == Tipo.Agua) perro.Beber(cantidad);
    }

    [ContextMenu("Probar (sube la barra)")]
    void Probar() { Aplicar(); }
}