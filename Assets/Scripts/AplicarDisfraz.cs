using UnityEngine;

/// <summary>
/// Ponlo en Ramón y en Ody (en TODAS las escenas donde aparezcan).
/// Al iniciar revisa el inventario: si ya ganó el disfraz, se lo pone.
/// Dos formas de ponerlo (usa la que te acomode, o las dos):
///  A) disfrazHijo: un objeto hijo del perro con un SpriteRenderer (la capa), desactivado por defecto.
///  B) controladorConDisfraz: un Animator Override Controller con las animaciones del perro YA vestido.
/// </summary>
public class AplicarDisfraz : MonoBehaviour
{
    public enum Perro { Ramon, Ody }

    [SerializeField] Perro perro;

    [Header("Opción A: capa como objeto hijo")]
    [SerializeField] GameObject disfrazHijo;

    [Header("Opción B: animaciones con el disfraz puesto")]
    [SerializeField] Animator animator;
    [SerializeField] RuntimeAnimatorController controladorConDisfraz;

    void Start()
    {
        bool lotiene = perro == Perro.Ramon ? InventarioJuego.DisfrazRamon : InventarioJuego.DisfrazOdy;
        if (lotiene) Aplicar();
    }

    public void Aplicar()
    {
        if (disfrazHijo) disfrazHijo.SetActive(true);
        if (animator && controladorConDisfraz) animator.runtimeAnimatorController = controladorConDisfraz;
    }
}
