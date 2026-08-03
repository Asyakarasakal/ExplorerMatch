using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip buttonClickSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitMusic();
    }

    private void InitMusic()
    {
        bool isMusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;

        if (musicSource != null)
        {
            musicSource.mute = !isMusicOn;

            if (backgroundMusic != null)
            {
                musicSource.clip = backgroundMusic;
                musicSource.loop = true;
                musicSource.Play();
            }
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        bool isSFXOn = PlayerPrefs.GetInt("SFXOn", 1) == 1;
        if (isSFXOn && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayButtonClickSound()
    {
        if (buttonClickSound != null)
        {
            PlaySFX(buttonClickSound);
        }
    }

    public void SetMusicState(bool isOn)
    {
        if (musicSource != null)
        {
            musicSource.mute = !isOn;

            if (isOn && !musicSource.isPlaying && backgroundMusic != null)
            {
                musicSource.Play();
            }
        }
    }
}