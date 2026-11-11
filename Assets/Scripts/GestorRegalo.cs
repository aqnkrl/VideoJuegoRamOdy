using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorRegalo : MonoBehaviour
{
    public GameObject objetoRegaloCerrado;
    public GameObject panelAccesoriosAbierto; // Objeto "Objetos" que contiene ambas imágenes (Moño y Pañoleta)
    public GameObject botonEquipar;
    public GameObject botonSiguienteNivel;

    public void AbrirRegalo()
    {
        if (objetoRegaloCerrado != null)
            objetoRegaloCerrado.SetActive(false);

        // Muestra el panel con AMBAS imágenes (Moño y Pañoleta juntas)
        if (panelAccesoriosAbierto != null)
            panelAccesoriosAbierto.SetActive(true);

        if (botonEquipar != null)
            botonEquipar.SetActive(true);
    }

    public void EquiparAccesorios()
    {
        Debug.Log("¡Accesorios confirmados para el Nivel 2!");

        // Oculta la caja y las imágenes del premio
        if (panelAccesoriosAbierto != null)
            panelAccesoriosAbierto.SetActive(false);

        if (botonEquipar != null)
            botonEquipar.SetActive(false);

        // Muestra el botón para avanzar de nivel
        if (botonSiguienteNivel != null)
            botonSiguienteNivel.SetActive(true);
    }

    public void IrAlSiguienteNivel()
    {
        SceneManager.LoadScene("Escena2"); // Avanza a la escena del baño
    }
}
