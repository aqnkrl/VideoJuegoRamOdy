using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorRegalo : MonoBehaviour
{
    public GameObject objetoRegaloCerrado;
    public GameObject panelAccesoriosAbierto; // Muestra la pañoleta y el moño
    public GameObject botonEquipar;
    public GameObject botonSiguienteNivel;

    public void AbrirRegalo()
    {
        if (objetoRegaloCerrado != null) objetoRegaloCerrado.SetActive(false);
        if (panelAccesoriosAbierto != null) panelAccesoriosAbierto.SetActive(true);
        if (botonEquipar != null) botonEquipar.SetActive(true);
    }

    public void EquiparAccesorios()
    {
        Debug.Log("¡Accesorios equipados en Ody y Ramón!");
        if (botonSiguienteNivel != null) botonSiguienteNivel.SetActive(true);
    }

    public void IrAlSiguienteNivel()
    {
        SceneManager.LoadScene("Escena2"); // Nombre de tu escena del baño
    }
}