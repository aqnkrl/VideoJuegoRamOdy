using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorRegalo : MonoBehaviour
{
    public GameObject objetoRegaloCerrado;
    public GameObject panelAccesoriosAbierto; // Muestra la pañoleta y el moño
    public GameObject botonEquipar;
    public GameObject botonSiguienteNivel;

    [Header("Accesorios puestos en los personajes (desactivados al inicio)")]
    public GameObject accesorioOdy;
    public GameObject accesorioRamon;

    public void AbrirRegalo()
    {
        if (objetoRegaloCerrado != null) objetoRegaloCerrado.SetActive(false);
        if (panelAccesoriosAbierto != null) panelAccesoriosAbierto.SetActive(true);
        if (botonEquipar != null) botonEquipar.SetActive(true);
    }

    public void EquiparAccesorios()
    {
        Debug.Log("¡Accesorios equipados en Ody y Ramón!");

        if (accesorioOdy != null) accesorioOdy.SetActive(true);
        if (accesorioRamon != null) accesorioRamon.SetActive(true);

        if (panelAccesoriosAbierto != null) panelAccesoriosAbierto.SetActive(false);
        if (botonEquipar != null) botonEquipar.SetActive(false);
        if (botonSiguienteNivel != null) botonSiguienteNivel.SetActive(true);
    }

    public void IrAlSiguienteNivel()
    {
        SceneManager.LoadScene("Escena2"); // Nombre de tu escena del baño
    }
}