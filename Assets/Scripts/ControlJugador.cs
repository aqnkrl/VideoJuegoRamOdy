using UnityEngine;

public class ControladorJugador : MonoBehaviour
{
    public enum EstadoMascota
    {
        Sucio,
        Enjabonado,
        Mojado,
        Limpio
    }

    [Header("Estado Actual de Ody")]
    public EstadoMascota estadoActual = EstadoMascota.Sucio;

    // Estructura para agrupar las direcciones por cada estado
    [System.Serializable]
    public struct SpritesDireccion
    {
        public Sprite frente;
        public Sprite espalda;
        public Sprite lado;
    }

    [Header("Sprites por cada Modo / Estado")]
    public SpritesDireccion odySucio;
    public SpritesDireccion odyEnjabonado;
    public SpritesDireccion odyMojado;
    public SpritesDireccion odyLimpio;

    private SpriteRenderer spriteRenderer;
    private string direccionActual = "frente"; // Controla si mira al frente, espalda o lado

    [Header("Configuración de Movimiento")]
    public float velocidad = 3f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ActualizarSpriteVisual();
    }

    void Update()
    {
        // Movimiento exclusivo con las FLECHAS del teclado (Ignora totalmente W, A, S, D)
        float movimientoX = 0f;
        float movimientoY = 0f;

        if (Input.GetKey(KeyCode.LeftArrow)) movimientoX = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) movimientoX = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) movimientoY = -1f;
        if (Input.GetKey(KeyCode.UpArrow)) movimientoY = 1f;

        Vector3 desplazamiento = new Vector3(movimientoX, movimientoY, 0) * velocidad * Time.deltaTime;
        transform.position += desplazamiento;

        // Detectar hacia dónde se mueve Ody para cambiar la dirección visual
        if (movimientoX != 0 || movimientoY != 0)
        {
            if (Mathf.Abs(movimientoX) > Mathf.Abs(movimientoY))
            {
                direccionActual = "lado";
                spriteRenderer.flipX = (movimientoX < 0); // Voltea el sprite si va a la izquierda
            }
            else
            {
                if (movimientoY > 0)
                {
                    direccionActual = "espalda";
                }
                else if (movimientoY < 0)
                {
                    direccionActual = "frente";
                }
            }
        }

        // Actualiza el sprite en tiempo real según estado y movimiento
        ActualizarSpriteVisual();
    }

    public void CambiarEstado(EstadoMascota nuevoEstado)
    {
        estadoActual = nuevoEstado;
        ActualizarSpriteVisual();
        Debug.Log("¡Estado de Ody cambiado a: " + estadoActual + "!");
    }

    void ActualizarSpriteVisual()
    {
        if (spriteRenderer == null) return;

        // Seleccionar el grupo de sprites según el estado actual de Ody
        SpritesDireccion grupoActual = odySucio;

        switch (estadoActual)
        {
            case EstadoMascota.Sucio:
                grupoActual = odySucio;
                break;
            case EstadoMascota.Enjabonado:
                grupoActual = odyEnjabonado;
                break;
            case EstadoMascota.Mojado:
                grupoActual = odyMojado;
                break;
            case EstadoMascota.Limpio:
                grupoActual = odyLimpio;
                break;
        }

        // Seleccionar el sprite correcto según la dirección en la que se mueve/mira
        Sprite spriteAUsar = grupoActual.frente;

        if (direccionActual == "espalda")
        {
            spriteAUsar = grupoActual.espalda;
        }
        else if (direccionActual == "lado")
        {
            spriteAUsar = grupoActual.lado;
        }
        else
        {
            spriteAUsar = grupoActual.frente;
        }

        if (spriteAUsar != null)
        {
            spriteRenderer.sprite = spriteAUsar;
        }
    }
}