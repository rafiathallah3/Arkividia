using UnityEngine;
using TMPro;

public class AudioToggleButton : MonoBehaviour
{
    public enum AudioType { Music, Ambient, SFX }

    public AudioType audioType;
    public TMP_Text labelText;
    public Color onColor = Color.green;
    public Color offColor = Color.red;

    private MenuButton menuButton;

    void Start()
    {
        menuButton = GetComponent<MenuButton>();

        // Hook into the MenuButton's click event
        if (menuButton != null)
        {
            menuButton.OnClick.AddListener(OnClicked);
        }

        UpdateUI();
    }

    void OnClicked()
    {
        if (AudioManager.instance == null) return;

        switch (audioType)
        {
            case AudioType.Music:
                AudioManager.instance.ToggleMusic();
                break;
            case AudioType.Ambient:
                AudioManager.instance.ToggleAmbient();
                break;
            case AudioType.SFX:
                AudioManager.instance.ToggleSFX();
                break;
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (AudioManager.instance == null || labelText == null) return;

        bool isOn = false;

        switch (audioType)
        {
            case AudioType.Music:
                isOn = AudioManager.instance.musicOn;
                break;
            case AudioType.Ambient:
                isOn = AudioManager.instance.ambientOn;
                break;
            case AudioType.SFX:
                isOn = AudioManager.instance.sfxOn;
                break;
        }

        string statusText = isOn ? "[ON]" : "[OFF]";
        labelText.text = $"{statusText}";
        labelText.color = isOn ? onColor : offColor;
    }
}
