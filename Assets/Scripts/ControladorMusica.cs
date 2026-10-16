using UnityEngine;

public class ControladorMusica : MonoBehaviour
{
    private static ControladorMusica instancia;

    void Awake()
    {
        // Si ya existe una música sonando, destruye el duplicado para que no se traslapen
        if (instancia != null)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        // Hace que este objeto de audio NO se destruya al cambiar de escena
        DontDestroyOnLoad(gameObject);
    }
}