using UnityEngine;

/// <summary>
/// Guarda qué regalos ya se obtuvieron/equiparon. Es estática, así que
/// sobrevive al cambiar de escena (y con PlayerPrefs también al cerrar el juego).
/// No se agrega a ningún objeto: se usa directo, por ejemplo InventarioJuego.Medalla.
/// </summary>
public static class InventarioJuego
{
    const string kRamon = "disfraz_ramon";
    const string kOdy = "disfraz_ody";
    const string kMedalla = "medalla_jugador";

    public static bool DisfrazRamon
    {
        get => PlayerPrefs.GetInt(kRamon, 0) == 1;
        set { PlayerPrefs.SetInt(kRamon, value ? 1 : 0); PlayerPrefs.Save(); }
    }

    public static bool DisfrazOdy
    {
        get => PlayerPrefs.GetInt(kOdy, 0) == 1;
        set { PlayerPrefs.SetInt(kOdy, value ? 1 : 0); PlayerPrefs.Save(); }
    }

    public static bool Medalla
    {
        get => PlayerPrefs.GetInt(kMedalla, 0) == 1;
        set { PlayerPrefs.SetInt(kMedalla, value ? 1 : 0); PlayerPrefs.Save(); }
    }

    /// <summary>Útil para probar: borra los regalos (llámalo desde un botón o al iniciar el juego).</summary>
    public static void Reiniciar()
    {
        PlayerPrefs.DeleteKey(kRamon);
        PlayerPrefs.DeleteKey(kOdy);
        PlayerPrefs.DeleteKey(kMedalla);
        PlayerPrefs.Save();
    }
}
