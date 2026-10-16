using UnityEngine;
using System.Collections;

public class SaltoRamonCompleto : MonoBehaviour
{
    [Header("Sprites de Ramón")]
    public Sprite spriteNormal; // El sprite normal de Ramón caminando/parado
    public Sprite spriteSaltando; // El sprite cuando va en el aire

    private SpriteRenderer spriteRenderer;

    [Header("Configuración del Salto")]
    public float alturaSalto = 2f;      // Qué tan alto sube
    public float distanciaAvance = 2.5f; // Qué tan adelante avanza al saltar
    public float duracionSalto = 0.5f;  // Duración total del salto

    private bool estaSaltando = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteNormal == null && spriteRenderer != null)
        {
            spriteNormal = spriteRenderer.sprite; // Guarda el sprite actual por defecto
        }
    }

    void Update()
    {
        // Al presionar la barra espaciadora y si no está saltando ya
        if (Input.GetKeyDown(KeyCode.Space) && !estaSaltando)
        {
            StartCoroutine(RutinaSaltoConMovimiento());
        }
    }

    IEnumerator RutinaSaltoConMovimiento()
    {
        estaSaltando = true;

        // 1. Cambiar al sprite de salto
        if (spriteRenderer != null && spriteSaltando != null)
        {
            spriteRenderer.sprite = spriteSaltando;
        }

        Vector3 posInicial = transform.position;

        // Detectar la dirección actual: si el sprite está volteado hacia la izquierda (flipX = true), 
        // el salto avanza hacia la izquierda; de lo contrario, avanza hacia la derecha.
        float direccionX = (spriteRenderer != null && spriteRenderer.flipX) ? -distanciaAvance : distanciaAvance;

        Vector3 posDestino = posInicial + new Vector3(direccionX, 0, 0);

        float tiempo = 0;

        // Movimiento de arco (Subir y Bajar avanzando)
        while (tiempo < duracionSalto)
        {
            float progreso = tiempo / duracionSalto;

            // Fórmula matemática para hacer un arco (parábola) en Y mientras se desplaza en X
            float alturaActual = Mathf.Sin(progreso * Mathf.PI) * alturaSalto;

            transform.position = Vector3.Lerp(posInicial, posDestino, progreso) + new Vector3(0, alturaActual, 0);

            tiempo += Time.deltaTime;
            yield return null;
        }

        // Asegurar la posición final exacta
        transform.position = posDestino;

        // 2. Regresar al sprite normal
        if (spriteRenderer != null && spriteNormal != null)
        {
            spriteRenderer.sprite = spriteNormal;
        }

        estaSaltando = false;
    }
}