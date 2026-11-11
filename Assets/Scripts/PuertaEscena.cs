using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Va en la puerta. Cuando cualquier perro (Ramón u Ody) la toca, carga otra escena.
[RequireComponent(typeof(Collider2D))]
public class PuertaEscena : MonoBehaviour
{
    [Tooltip("Nombre exacto de la escena a la que lleva la puerta (igual que en Build Profiles, sin .unity).")]
    public string nombreEscena = "Escena3";

    [Tooltip("Segundos de espera antes de cambiar de escena (0 = al instante).")]
    public float retraso = 0f;

    bool cambiando;

    void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (cambiando) return;

        // Solo reacciona a los perros (los que tienen el script PerroDescanso)
        var perro = other.GetComponentInParent<PerroDescanso>();
        if (perro == null) return;

        StartCoroutine(Cambiar());
    }

    IEnumerator Cambiar()
    {
        cambiando = true;
        if (retraso > 0f) yield return new WaitForSeconds(retraso);

        if (!Application.CanStreamedLevelBeLoaded(nombreEscena))
        {
            Debug.LogError("No se encontró la escena '" + nombreEscena + "'. " +
                           "Revisa el nombre y que esté agregada en File > Build Profiles (Scene List).");
            cambiando = false;
            yield break;
        }

        SceneManager.LoadScene(nombreEscena);
    }
}