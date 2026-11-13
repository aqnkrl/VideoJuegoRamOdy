using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorRegaloFinal : MonoBehaviour
{
    public GameObject panelRegalo;               // PanelRegalo
    public GameObject botonAbrir;                // botón ABRIR
    public GameObject objetoRegaloCerrado;       // ImagenRegaloCerrado
    public GameObject panelAccesoriosAbierto;    // "Objetos" (capas, antifaces, medalla)
    public GameObject botonEquipar;              // BotonEquipar
    public GameObject botonSiguienteNivel;       // BotonSiguienteNivel
    public string escenaSiguiente = "Escena6";

    bool yaMostrado;

    void Awake()
    {
        if (panelRegalo != null)
        {
            if (transform.IsChildOf(panelRegalo.transform))
                Debug.LogError("[GestorRegaloFinal] Este objeto está DENTRO de PanelRegalo. Sácalo del panel y ponlo en un objeto vacío activo.", this);

            panelRegalo.SetActive(false);
        }
        else
        {
            Debug.LogError("[GestorRegaloFinal] Falta asignar 'Panel Regalo' en el Inspector.", this);
        }
    }

    // La llama la puerta cuando los perros cruzan
    public void MostrarPanelFinal()
    {
        if (yaMostrado) return;
        yaMostrado = true;

        Debug.Log("[GestorRegaloFinal] Mostrando panel de regalo");

        if (objetoRegaloCerrado != null) objetoRegaloCerrado.SetActive(true);
        if (botonAbrir != null) botonAbrir.SetActive(true);
        if (panelAccesoriosAbierto != null) panelAccesoriosAbierto.SetActive(false);
        if (botonEquipar != null) botonEquipar.SetActive(false);
        if (botonSiguienteNivel != null) botonSiguienteNivel.SetActive(false);

        if (panelRegalo != null)
        {
            panelRegalo.SetActive(true);
            panelRegalo.transform.SetAsLastSibling();   // al frente del Canvas
        }
    }

    // On Click de ABRIR
    public void AbrirRegalo()
    {
        if (objetoRegaloCerrado != null) objetoRegaloCerrado.SetActive(false);
        if (botonAbrir != null) botonAbrir.SetActive(false);
        if (panelAccesoriosAbierto != null) panelAccesoriosAbierto.SetActive(true);
        if (botonEquipar != null) botonEquipar.SetActive(true);
    }

    // On Click de EQUIPAR
    public void EquiparRegalos()
    {
        if (panelAccesoriosAbierto != null) panelAccesoriosAbierto.SetActive(false);
        if (botonEquipar != null) botonEquipar.SetActive(false);
        if (botonSiguienteNivel != null) botonSiguienteNivel.SetActive(true);
    }

    // On Click de SIGUIENTE NIVEL
    public void IrAlSiguienteNivel()
    {
        SceneManager.LoadScene(escenaSiguiente);
    }
}