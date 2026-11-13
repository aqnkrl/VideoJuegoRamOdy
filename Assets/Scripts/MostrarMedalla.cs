using UnityEngine;

/// <summary>
/// Ponlo en un Image del Canvas con el sprite de la medalla (en Escena6 y las que sigan).
/// Solo se ve si el jugador ya ganó la medalla.
/// </summary>
public class MostrarMedalla : MonoBehaviour
{
    void Awake()
    {
        gameObject.SetActive(InventarioJuego.Medalla);
    }
}
