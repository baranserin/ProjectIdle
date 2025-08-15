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

    [Header("Bildirim Ayarlarý")]
    public Button NtfButton;
    public Sprite NtfOnSprite;
    public Sprite NtfOffSprite;

    private bool isMusicOn;
    private bool isSfxOn;
    private bool isNtfOn;

    void Start()
    {
        // Kaydedilmiþ deðerleri yükle (Varsayýlan: açýk)
        isMusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        isSfxOn = PlayerPrefs.GetInt("SfxOn", 1) == 1;
        isNtfOn = PlayerPrefs.GetInt("NtfOn", 1) == 1;
        // Buton ikonlarýný güncelle
        UpdateMusicUI();
        UpdateSfxUI();
        UpdateNtfUI();

        // Butonlara týklama olaylarýný ekle
        musicButton.onClick.AddListener(ToggleMusic);
        sfxButton.onClick.AddListener(ToggleSfx);
        NtfButton.onClick.AddListener(ToggleNtf);
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

    public void ToggleNtf()
    {
        isNtfOn= !isNtfOn;
        PlayerPrefs.SetInt("NtfOn", isNtfOn ? 1 : 0);
        PlayerPrefs.Save();
        UpdateNtfUI();
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

    private void UpdateNtfUI()
    {
        NtfButton.image.sprite= isNtfOn ? NtfOnSprite : NtfOffSprite;
    }
}
