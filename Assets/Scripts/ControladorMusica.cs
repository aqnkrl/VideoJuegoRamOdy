using UnityEngine;

public class ControladorMusica : MonoBehaviour
{
    private static ControladorMusica instancia;
    public AudioSource audioSource;

    void Awake()
    {
        if (instancia != null)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Cargar volumen guardado al iniciar
        float volumenGuardado = PlayerPrefs.GetFloat("VolumenMusica", 1f);
        if (audioSource != null)
        {
            audioSource.volume = volumenGuardado;
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    // Método para cambiar el volumen desde cualquier escena
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