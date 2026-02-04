using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioMixer audioMixer;

    const string MUSIC = "MusicVolume";
    const string SFX = "SFXVolume";
    const string AMBIENT = "AmbientVolume";

    public bool musicOn = true;
    public bool sfxOn = true;
    public bool ambientOn = true;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ===== TOGGLES =====

    public void ToggleMusic()
    {
        musicOn = !musicOn;
        audioMixer.SetFloat(MUSIC, musicOn ? 0f : -80f);
        PlayerPrefs.SetInt(MUSIC, musicOn ? 1 : 0);
    }

    public void ToggleSFX()
    {
        sfxOn = !sfxOn;
        audioMixer.SetFloat(SFX, sfxOn ? 0f : -80f);
        PlayerPrefs.SetInt(SFX, sfxOn ? 1 : 0);
    }

    public void ToggleAmbient()
    {
        ambientOn = !ambientOn;
        audioMixer.SetFloat(AMBIENT, ambientOn ? 0f : -80f);
        PlayerPrefs.SetInt(AMBIENT, ambientOn ? 1 : 0);
    }

    // ===== LOAD =====

    void LoadSettings()
    {
        musicOn = PlayerPrefs.GetInt(MUSIC, 1) == 1;
        sfxOn = PlayerPrefs.GetInt(SFX, 1) == 1;
        ambientOn = PlayerPrefs.GetInt(AMBIENT, 1) == 1;

        audioMixer.SetFloat(MUSIC, musicOn ? 0f : -80f);
        audioMixer.SetFloat(SFX, sfxOn ? 0f : -80f);
        audioMixer.SetFloat(AMBIENT, ambientOn ? 0f : -80f);
    }
}
