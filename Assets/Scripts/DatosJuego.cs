using UnityEngine;

public class DatosJuego : MonoBehaviour
{
    public static DatosJuego Instance;

    [Header("Progreso de Ody (Baño)")]
    public float progresoJabon = 0f;
    public float progresoRegadera = 0f;
    public float progresoSecador = 0f;
    public bool banoCompletado = false;

    [Header("Progreso de Ramón (Jardín)")]
    public float progresoPelota = 0f;
    public float progresoSalto = 0f;
    public bool jardinCompletado = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Hace que este objeto no se destruya al cambiar de escena
        }
        else
        {
            Destroy(gameObject);
        }
    }
}