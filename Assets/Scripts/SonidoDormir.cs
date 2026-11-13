using UnityEngine;

public class SonidoDormir : MonoBehaviour
{
    public static SonidoDormir Instance;

    public AudioSource audioSource;
    int perrosDormidos = 0;

    void Awake()
    {
        Instance = this;
    }

    public void PerroSeAcosto()
    {
        perrosDormidos++;
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public void PerroSeLevanto()
    {
        perrosDormidos = Mathf.Max(0, perrosDormidos - 1);
        if (perrosDormidos == 0)
            audioSource.Stop();
    }
}