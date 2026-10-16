using UnityEngine;

public class PuertaNivel : MonoBehaviour
{
    [Header("Niveles")]
    public GameObject nivelActual;      // Nivel1
    public GameObject nivelSiguiente;   // Nivel2

    [Header("Personajes")]
    public Transform[] personajes;      // los dos personajes
    public Transform puntoInicio;       // dónde aparecen en Nivel2
    public int personajesNecesarios = 2;

    private int personajesEnPuerta = 0;
    private bool yaCambio = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        personajesEnPuerta++;
        IntentarCambiar();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        personajesEnPuerta--;
    }

    void IntentarCambiar()
    {
        if (yaCambio) return;
        if (personajesEnPuerta < personajesNecesarios) return;
        if (!GestorNivel.Instancia.nivelCompletado) return;

        yaCambio = true;

        foreach (Transform p in personajes)
            p.position = puntoInicio.position;

        nivelActual.SetActive(false);
        nivelSiguiente.SetActive(true);
    }
}