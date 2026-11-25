using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorRegalo : MonoBehaviour
{
    public GameObject objetoRegaloCerrado;
    public GameObject panelAccesoriosAbierto;
    public GameObject botonEquipar;
    public GameObject botonSiguienteNivel;

    [Header("Configuración de Nivel")]
    [Tooltip("Escribe exactamente el nombre de la escena a la que debe llevar este botón (Ej: Escena2, Escena3, Escena4)")]
    public string nombreSiguienteEscena = "Escena2"; // Valor por defecto

    public void AbrirRegalo()
    {
        if (objetoRegaloCerrado != null)
            objetoRegaloCerrado.SetActive(false);

        if (panelAccesoriosAbierto != null)
            panelAccesoriosAbierto.SetActive(true);

        if (botonEquipar != null)
            botonEquipar.SetActive(true);
    }

    public void EquiparAccesorios()
    {
        Debug.Log("¡Accesorios confirmados!");

        if (panelAccesoriosAbierto != null)
            panelAccesoriosAbierto.SetActive(false);

        if (botonEquipar != null)
            botonEquipar.SetActive(false);

        if (botonSiguienteNivel != null)
            botonSiguienteNivel.SetActive(true);
    }

    public void IrAlSiguienteNivel()
    {
        // Carga la escena configurada en el inspector de este nivel en específico
        if (!string.IsNullOrEmpty(nombreSiguienteEscena))
        {
            SceneManager.LoadScene(nombreSiguienteEscena);
        }
        else
        {
            Debug.LogError("¡No se ha configurado el nombre de la siguiente escena en el inspector!");
        }
    }
}