using UnityEngine;

public class PlatoMascota : MonoBehaviour
{
    public enum Tipo { Comida, Agua }

    [Header("Qué es y a quién alimenta")]
    public Tipo tipo;
    public NecesidadesPerro perro;                     // Ody o Ramon
    [Range(0f, 1f)] public float cantidad = 0.5f;      // cuánto sube la barra cada vez

    // Llama a este método cuando pongas las croquetas o el agua en el plato
    public void Llenar()
    {
        if (perro == null)
        {
            Debug.LogError("[PlatoMascota] El plato '" + name + "' no tiene perro asignado.", this);
            return;
        }
        if (tipo == Tipo.Comida) perro.Comer(cantidad);
        else perro.Beber(cantidad);
    }

    // Para probar sin jugar: clic derecho en el componente → "Probar Llenar" (en Play)
    [ContextMenu("Probar Llenar")]
    void Probar() { Llenar(); }
}