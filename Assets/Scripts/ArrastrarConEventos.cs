using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArrastrarConEventos : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 posicionInicial;
    private Camera camaraPrincipal;
    private Canvas canvasPrincipal;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    [Header("Configuración del Destino")]
    public Transform platoDestino;     // Arrastra aquí el plato correspondiente en la escena
    public Sprite spritePlatoLleno;    // El sprite que mostrará el plato al llenarse
    private SpriteRenderer spriteRendererPlato;

    [Header("Barra de Estado UI")]
    public Slider barraAsociada;       // La barra de slider que se llenará

    [Header("Efecto de Sonido (SFX)")]
    public AudioSource fuenteAudio;    // El AudioSource que está en este mismo objeto

    void Awake()
    {
        camaraPrincipal = Camera.main;
        canvasPrincipal = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (platoDestino != null)
        {
            spriteRendererPlato = platoDestino.GetComponent<SpriteRenderer>();
        }

        if (fuenteAudio == null)
        {
            fuenteAudio = GetComponent<AudioSource>();
        }
    }

    void Start()
    {
        posicionInicial = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        posicionInicial = transform.position;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 posicionMundo = camaraPrincipal.ScreenToWorldPoint(Input.mousePosition);
        posicionMundo.z = 0f;
        transform.position = posicionMundo;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (platoDestino != null)
        {
            float distancia = Vector2.Distance(transform.position, platoDestino.position);

            if (distancia < 2.5f)
            {
                // 1. Cambiar el sprite del plato al llenarse
                if (spriteRendererPlato != null && spritePlatoLleno != null)
                {
                    spriteRendererPlato.sprite = spritePlatoLleno;
                }

                // 2. Llenar la barra asociada
                if (barraAsociada != null)
                {
                    barraAsociada.value = 1f;
                }

                // 3. REPRODUCIR EL SONIDO EXACTAMENTE UNA VEZ
                if (fuenteAudio != null && fuenteAudio.clip != null)
                {
                    fuenteAudio.PlayOneShot(fuenteAudio.clip);
                }

                // 4. Avanzar nivel si el gestor existe
                if (GestorNivel.Instancia != null)
                {
                    GestorNivel.Instancia.AvanzarProgreso();
                }

                // 5. Desactivar el objeto esperando 3.5 segundos para que el audio de la comida suene completo
                StartCoroutine(DesactivarConRetraso(1.5f));
                return;
            }
        }

        transform.position = posicionInicial;
    }

    private System.Collections.IEnumerator DesactivarConRetraso(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        gameObject.SetActive(false);
    }
}