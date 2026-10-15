using UnityEngine;
using UnityEngine.UI;

public class ArrastrarMundo2D : MonoBehaviour
{
    private Vector3 posicionInicial;
    private bool estaArrastrando = false;
    private Camera camaraPrincipal;

    [Header("Configuración del Destino")]
    public Transform platoDestino;     // El plato exacto en la escena
    public Sprite spritePlatoLleno;    // Sprite cuando el plato se llena
    private SpriteRenderer spriteRendererPlato;

    [Header("Barra de Estado UI")]
    public Slider barraAsociada;       // La barra que se llenará

    [Header("Efecto de Sonido (SFX)")]
    public AudioSource fuenteAudio;    // El componente AudioSource
    public AudioClip sonidoExito;      // El audio de comer o beber

    void Start()
    {
        camaraPrincipal = Camera.main;
        posicionInicial = transform.position;

        if (platoDestino != null)
        {
            spriteRendererPlato = platoDestino.GetComponent<SpriteRenderer>();
        }
    }

    void OnMouseDown()
    {
        estaArrastrando = true;
    }

    void OnMouseDrag()
    {
        if (estaArrastrando)
        {
            Vector3 mousePos = camaraPrincipal.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            transform.position = mousePos;
        }
    }

    void OnMouseUp()
    {
        estaArrastrando = false;

        if (platoDestino != null)
        {
            float distancia = Vector2.Distance(transform.position, platoDestino.position);

            // Si lo sueltas cerca del plato (menos de 2.5 unidades)
            if (distancia < 2.5f)
            {
                // 1. Cambiar el sprite del plato
                if (spriteRendererPlato != null && spritePlatoLleno != null)
                {
                    spriteRendererPlato.sprite = spritePlatoLleno;
                }

                // 2. Llenar la barra
                if (barraAsociada != null)
                {
                    barraAsociada.value = 1f;
                }

                // 3. Reproducir sonido
                if (fuenteAudio != null && sonidoExito != null)
                {
                    fuenteAudio.PlayOneShot(sonidoExito);
                }

                // 4. Avanzar nivel
                if (GestorNivel.Instancia != null)
                {
                    GestorNivel.Instancia.AvanzarProgreso();
                }

                // 5. Desactivar objeto
                gameObject.SetActive(false);
                return;
            }
        }

        // Si falló, regresa a su posición inicial
        transform.position = posicionInicial;
    }
}