using UnityEngine;

public class HerramientaInteractiva : MonoBehaviour
{
    [Header("Escribe exactamente: Jabon, Regadera o Secador")]
    public string nombreHerramienta;
    public ControladorBano controlador;

    void OnMouseDown()
    {
        if (controlador != null)
        {
            controlador.InteractuarConHerramienta(nombreHerramienta);
        }
    }
}