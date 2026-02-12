using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource sfxSource;
    public AudioSource musicSource;

    [Header("SFX")]
    public AudioClip shoot;
    public AudioClip explosion;
    public AudioClip ufo;
    public AudioClip invaderMove;

    void Awake()
    {
        Instance = this;
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }
}
