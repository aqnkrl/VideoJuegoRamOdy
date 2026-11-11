using UnityEngine;

public class GestorAudioGlobal : MonoBehaviour
{
    private static GestorAudioGlobal instancia;
    public AudioSource fuenteMusicaFondo;

    void Awake()
    {
        // Patrón Singleton para que solo exista un gestor de audio y sobreviva entre escenas
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (fuenteMusicaFondo == null)
        {
            fuenteMusicaFondo = GetComponent<AudioSource>();
        }

        // Cargar el volumen guardado previamente (por defecto 1f si es la primera vez)
        float volumenGuardado = PlayerPrefs.GetFloat("VolumenMusica", 1f);
        if (fuenteMusicaFondo != null)
        {
            fuenteMusicaFondo.volume = volumenGuardado;
            if (!fuenteMusicaFondo.isPlaying)
            {
                fuenteMusicaFondo.Play();
            }
        }
    }

    // Método global para cambiar el volumen desde cualquier escena o menú de ajustes
    public void CambiarVolumenMusica(float nuevoVolumen)
    {
        if (fuenteMusicaFondo != null)
        {
            fuenteMusicaFondo.volume = nuevoVolumen;
        }
        // Guardamos el valor en la memoria del juego
        PlayerPrefs.SetFloat("VolumenMusica", nuevoVolumen);
        PlayerPrefs.Save();
    }
}