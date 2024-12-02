using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    // Awake: Aplicación de patrón Singleton para crear una única instancia de la clase
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // PlayClip: Reproducción de un sonido determinado
    public void PlayClip(AudioClip clip, AudioSource source)
    {
        source.clip = clip;
        source.Play();
    }

    // PlayRandomClip: Reproducción de un sonido aleatorio de una lista de sonidos
    public void PlayRandomClip(AudioClip[] clips, AudioSource source)
    {
        int randomindex = Random.Range(0, clips.Length);

        source.clip = clips[randomindex];
        source.Play();
    }
}
