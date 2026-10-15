using UnityEngine;
using UnityEngine.UI;

public class ArrastrarObjeto : MonoBehaviour
{
    private Vector3 posicionInicial;
    private bool estaArrastrando = false;
    private Camera camaraPrincipal;

    [Header("Configuración de Interacción")]
    public string etiquetaObjetivo;    // Ejemplo: "PlatoComidaOdy" o "PlatoAguaOdy"
    public Sprite spriteLleno;         // Sprite del plato lleno
    public SpriteRenderer platoVisual; // SpriteRenderer del plato vacío

    [Header("UI Barra de Estado")]
    public Slider barraAsociada;       // Barra que se llenará

    void Start()
    {
        camaraPrincipal = Camera.main;
        posicionInicial = transform.position;
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

        if (platoVisual != null)
        {
            // Calcula la distancia al plato asignado
            float distancia = Vector2.Distance(transform.position, platoVisual.transform.position);

            // Ampliamos un poco el rango a 2.5f por si acaso cuesta atinarle exactamente
            bool distanciaValida = distancia < 2.5f;
            bool etiquetaValida = string.IsNullOrEmpty(etiquetaObjetivo) || platoVisual.CompareTag(etiquetaObjetivo);

            if (distanciaValida && etiquetaValida)
            {
                // 1. Cambiar el sprite del plato en el suelo
                if (spriteLleno != null)
                {
                    platoVisual.sprite = spriteLleno;
                }

                // 2. Llenar la barra
                if (barraAsociada != null)
                {
                    barraAsociada.value = 1f;
                }

                // 3. Avanzar nivel
                if (GestorNivel.Instancia != null)
                {
                    GestorNivel.Instancia.AvanzarProgreso();
                }

                // 4. Desactivar objeto
                gameObject.SetActive(false);
                return;
            }
            else
            {
                Debug.Log("No se pudo soltar. Distancia: " + distancia + " (Válida < 2.5). Etiqueta correcta: " + etiquetaValida);
            }
        }

        // Si falló, regresa a su posición inicial
        transform.position = posicionInicial;
    }
}