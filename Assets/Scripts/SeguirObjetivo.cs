using UnityEngine;

public class SeguirObjetivo : MonoBehaviour
{
    [Header("Configuración de Seguimiento")]
    public Transform objetivo; // El perrito (Ody o Ramón)
    public Vector3 desfasamiento = new Vector3(0, 1.2f, 0); // Altura sobre la cabeza

    void LateUpdate()
    {
        if (objetivo != null)
        {
            // Actualiza la posición de la barra a la posición del perrito + el desfasamiento
            transform.position = objetivo.position + desfasamiento;
        }
    }
}