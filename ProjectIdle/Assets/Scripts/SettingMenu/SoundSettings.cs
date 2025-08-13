using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsController : MonoBehaviour
{
    [Header("Müzik Ayarlarý")]
    public Button musicButton;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    public AudioSource musicSource;

    [Header("Ses Efekt Ayarlarý")]
    public Button sfxButton;
    public Sprite sfxOnSprite;
    public Sprite sfxOffSprite;

    private bool isMusicOn;
    private bool isSfxOn;

    void Start()
    {
        // Kaydedilmiþ deðerleri yükle (Varsayýlan: açýk)
        isMusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        isSfxOn = PlayerPrefs.GetInt("SfxOn", 1) == 1;

        // Buton ikonlarýný güncelle
        UpdateMusicUI();
        UpdateSfxUI();

        // Butonlara týklama olaylarýný ekle
        musicButton.onClick.AddListener(ToggleMusic);
        sfxButton.onClick.AddListener(ToggleSfx);
    }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        PlayerPrefs.SetInt("MusicOn", isMusicOn ? 1 : 0);
        PlayerPrefs.Save();

        UpdateMusicUI();
    }

    public void ToggleSfx()
    {
        isSfxOn = !isSfxOn;
        PlayerPrefs.SetInt("SfxOn", isSfxOn ? 1 : 0);
        PlayerPrefs.Save();

        UpdateSfxUI();
    }

    private void UpdateMusicUI()
    {
        musicButton.image.sprite = isMusicOn ? musicOnSprite : musicOffSprite;
        if (isMusicOn)
            musicSource.Play();
        else
            musicSource.Pause();
    }

    private void UpdateSfxUI()
    {
        sfxButton.image.sprite = isSfxOn ? sfxOnSprite : sfxOffSprite;
        AudioListener.volume = isSfxOn ? 1f : 0f;
    }
}
