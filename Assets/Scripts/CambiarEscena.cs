using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    [Header("Nombre de la escena a la que deseas viajar")]
    public string nombreEscenaDestino;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si el objeto que tocó la puerta es el jugador (puedes usar una etiqueta o el nombre)
        if (collision.CompareTag("Player") || collision.name.Contains("Ody") || collision.name.Contains("Ramon"))
        {
            if (!string.IsNullOrEmpty(nombreEscenaDestino))
            {
                SceneManager.LoadScene(nombreEscenaDestino);
            }
            else
            {
                Debug.LogWarning("El nombre de la escena de destino está vacío.");
            }
        }
    }
}