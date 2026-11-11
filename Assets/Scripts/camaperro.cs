using UnityEngine;

// Va en cada cama (azul, naranja o las que agregues).
[RequireComponent(typeof(Collider2D))]
public class CamaPerro : MonoBehaviour
{
    [Tooltip("Vacío = cualquier perro puede usarla. Si arrastras a Ramón u Ody, solo él.")]
    public PerroDescanso duenoExclusivo;

    [Tooltip("Opcional: punto donde se acuesta el perro. Si está vacío usa el centro de la cama.")]
    public Transform puntoDescanso;

    [Tooltip("Opcional: un objeto hijo con el aviso (por ejemplo una 'E') que se muestra al acercarse.")]
    public GameObject avisoTecla;

    public SpriteRenderer SpriteCama { get; private set; }

    void Awake()
    {
        SpriteCama = GetComponent<SpriteRenderer>();
        GetComponent<Collider2D>().isTrigger = true;
        if (avisoTecla != null) avisoTecla.SetActive(false);
    }

    public bool PuedeUsar(PerroDescanso perro)
    {
        return duenoExclusivo == null || duenoExclusivo == perro;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Toqué la cama con: " + other.name);
        var perro = other.GetComponentInParent<PerroDescanso>();
        if (perro == null || !PuedeUsar(perro)) return;

        perro.camaCercana = this;
        if (avisoTecla != null && !perro.Acostado) avisoTecla.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var perro = other.GetComponentInParent<PerroDescanso>();
        if (perro == null) return;

        if (perro.camaCercana == this) perro.camaCercana = null;
        if (avisoTecla != null) avisoTecla.SetActive(false);
    }
}