using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SecuenciaFinal : MonoBehaviour
{
    [System.Serializable]
    public class Paso
    {
        public GameObject[] elementos;      // lo que aparece en este paso
        public float duracion = 3f;         // segundos que se queda visible
        public bool desaparecer = true;     // si se desvanece al terminar su tiempo
    }

    [Header("Pasos en orden")]
    public Paso[] pasos;

    [Header("Tiempos de animación")]
    public float duracionAparicion = 0.4f;
    public float duracionDesaparicion = 0.5f;

    private Dictionary<GameObject, Vector3> escalas = new Dictionary<GameObject, Vector3>();

    private void Awake()
    {
        foreach (Paso p in pasos)
            foreach (GameObject e in p.elementos)
                if (e != null && !escalas.ContainsKey(e))
                    escalas[e] = e.transform.localScale;
    }

    private void OnEnable()
    {
        foreach (Paso p in pasos)
            foreach (GameObject e in p.elementos)
                if (e != null) e.SetActive(false);

        StartCoroutine(Secuencia());
    }

    private IEnumerator Secuencia()
    {
        for (int i = 0; i < pasos.Length; i++)
        {
            // Muestra el paso actual
            foreach (GameObject e in pasos[i].elementos)
                if (e != null) StartCoroutine(Aparecer(e));

            // El último paso se queda en pantalla
            if (i == pasos.Length - 1) yield break;

            yield return new WaitForSecondsRealtime(pasos[i].duracion);

            // Desvanece el paso antes de pasar al siguiente
            if (pasos[i].desaparecer)
                yield return Desvanecer(pasos[i].elementos);
        }
    }

    private CanvasGroup ObtenerGrupo(GameObject objeto)
    {
        CanvasGroup grupo = objeto.GetComponent<CanvasGroup>();
        if (grupo == null) grupo = objeto.AddComponent<CanvasGroup>();
        return grupo;
    }

    private IEnumerator Aparecer(GameObject objeto)
    {
        Vector3 escalaFinal = escalas.ContainsKey(objeto) ? escalas[objeto] : Vector3.one;
        CanvasGroup grupo = ObtenerGrupo(objeto);

        grupo.alpha = 1f;
        objeto.transform.localScale = Vector3.zero;
        objeto.SetActive(true);

        float t = 0f;
        while (t < duracionAparicion)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duracionAparicion);
            float rebote = 1f + 0.15f * Mathf.Sin(p * Mathf.PI);
            objeto.transform.localScale = escalaFinal * (p * rebote);
            yield return null;
        }
        objeto.transform.localScale = escalaFinal;
    }

    private IEnumerator Desvanecer(GameObject[] objetos)
    {
        float t = 0f;
        while (t < duracionDesaparicion)
        {
            t += Time.unscaledDeltaTime;
            float alpha = 1f - Mathf.Clamp01(t / duracionDesaparicion);

            foreach (GameObject e in objetos)
                if (e != null) ObtenerGrupo(e).alpha = alpha;

            yield return null;
        }

        foreach (GameObject e in objetos)
            if (e != null) e.SetActive(false);
    }
}