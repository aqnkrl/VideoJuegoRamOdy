using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonInicio : MonoBehaviour
{
    public void IrAInicio()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Escena0");
    }
}
