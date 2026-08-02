using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Background Music Clip")]
    public AudioClip backgroundMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Sahneler arasý geçiþte müzik kesilmesin
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Müzik çalmaya baþlasýn mý kontrolü ve baþlatma (Awake anýnda)
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

    public void SetMusicState(bool isOn)
    {
        if (musicSource != null)
        {
            musicSource.mute = !isOn;

            // Eðer müzik bir þekilde durmuþsa tekrar tetikle
            if (isOn && !musicSource.isPlaying && backgroundMusic != null)
            {
                musicSource.Play();
            }
        }
    }
}