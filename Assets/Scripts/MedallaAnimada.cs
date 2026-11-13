using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Opcional: pon este script en el Image de la medalla y arrastra tus 3 sprites
/// (medalla normal y las dos con brillitos) para que parpadee.
/// </summary>
[RequireComponent(typeof(Image))]
public class MedallaAnimada : MonoBehaviour
{
    [SerializeField] Sprite[] cuadros;          // medalla, medalla brillo 1, medalla brillo 2
    [SerializeField] float segundosPorCuadro = 0.35f;

    Image imagen;
    float t;
    int i;

    void Awake() { imagen = GetComponent<Image>(); }

    void Update()
    {
        if (cuadros == null || cuadros.Length < 2) return;
        t += Time.unscaledDeltaTime;               // sigue animando aunque haya pausa
        if (t >= segundosPorCuadro)
        {
            t = 0f;
            i = (i + 1) % cuadros.Length;
            imagen.sprite = cuadros[i];
        }
    }
}
