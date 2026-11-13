using UnityEngine;
using UnityEngine.SceneManagement;

public class VolverAEscena0 : MonoBehaviour
{
    public void Volver()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Escena0");
    }
}