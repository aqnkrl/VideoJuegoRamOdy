using UnityEngine;

public class ControladorMusica : MonoBehaviour
{
    private static ControladorMusica instancia;
    public AudioSource audioSource;

    void Awake()
    {
        // Si ya existe otra instancia previa, destruimos esta copia nueva para evitar música duplicada
        if (instancia != null)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);

        // Si no asignaste el AudioSource en el Inspector, lo busca automáticamente en el mismo objeto
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Cargar el volumen guardado en PlayerPrefs al iniciar
        float volumenGuardado = PlayerPrefs.GetFloat("VolumenMusica", 1f);
        if (audioSource != null)
        {
            audioSource.volume = volumenGuardado;

            // Reproduce la música solo si no se está reproduciendo ya
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    // Método para cambiar el volumen desde cualquier escena o menú de ajustes
    public void CambiarVolumen(float nuevoVolumen)
    {
        if (audioSource != null)
        {
            audioSource.volume = nuevoVolumen;
        }
        PlayerPrefs.SetFloat("VolumenMusica", nuevoVolumen);
        PlayerPrefs.Save();
    }
}
