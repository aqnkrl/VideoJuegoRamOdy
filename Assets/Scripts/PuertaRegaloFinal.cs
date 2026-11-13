using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PuertaPanelRegalo : MonoBehaviour
{
    [Header("Panel")]
    public GestorRegaloFinal gestor;

    [Header("Perros (si los dejas vacíos se buscan por nombre: Ody y Ramon)")]
    public GameObject ody;
    public GameObject ramon;
    public bool requiereAmbosPerros = true;
    public bool recordarLlegada = true;
    public bool congelarPerros = true;

    bool odyEnPuerta, ramonEnPuerta, disparado;

    void Start()
    {
        if (gestor == null) gestor = FindFirstObjectByType<GestorRegaloFinal>();
        if (ody == null) ody = GameObject.Find("Ody");
        if (ramon == null) ramon = GameObject.Find("Ramon");

        if (gestor == null) Debug.LogError("[PuertaPanelRegalo] No encuentro GestorRegaloFinal en la escena.", this);
        if (ody == null) Debug.LogWarning("[PuertaPanelRegalo] No encuentro a Ody.", this);
        if (ramon == null) Debug.LogWarning("[PuertaPanelRegalo] No encuentro a Ramon.", this);

        var col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
            Debug.LogWarning("[PuertaPanelRegalo] El Collider 2D de la puerta no tiene 'Is Trigger' marcado.", this);
    }

    static bool EsDe(Collider2D c, GameObject perro)
    {
        return perro != null && (c.gameObject == perro || c.transform.IsChildOf(perro.transform));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("[PuertaPanelRegalo] Entró al trigger: " + other.name);
        Marcar(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        Marcar(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (recordarLlegada) return;
        if (EsDe(other, ody)) odyEnPuerta = false;
        if (EsDe(other, ramon)) ramonEnPuerta = false;
    }

    void Marcar(Collider2D other)
    {
        if (disparado) return;
        if (EsDe(other, ody)) odyEnPuerta = true;
        if (EsDe(other, ramon)) ramonEnPuerta = true;

        bool listo = requiereAmbosPerros ? (odyEnPuerta && ramonEnPuerta)
                                         : (odyEnPuerta || ramonEnPuerta);
        if (!listo) return;

        disparado = true;
        Debug.Log("[PuertaPanelRegalo] Perros en la puerta: mostrando panel");

        if (congelarPerros)
        {
            Congelar(ody);
            Congelar(ramon);
        }

        if (gestor != null) gestor.MostrarPanelFinal();
    }

    static void Congelar(GameObject perro)
    {
        if (perro == null) return;
        foreach (var m in perro.GetComponentsInChildren<MonoBehaviour>())
            if (m != null) m.enabled = false;
        var rb = perro.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }
}