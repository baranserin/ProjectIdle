using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkinSelector : MonoBehaviour
{
    [Header("UI")]
    public Image targetImage;
    public Button buyButton;

    [Header("Preview Settings")]
    public float previewDuration = 2f;

    private Sprite currentSprite;   // Satın alınmış (kalıcı)
    private Sprite previewSprite;   // Geçici seçim
    private Coroutine previewCoroutine;

    private const string SAVE_KEY = "SelectedSkin";

    void Start()
    {
        LoadSkin();
        buyButton.interactable = false;
    }

    // RENK BUTONLARINA BAĞLANACAK
    public void PreviewSkin(Sprite selectedSprite)
    {
        if (previewCoroutine != null)
            StopCoroutine(previewCoroutine);

        previewSprite = selectedSprite;
        targetImage.sprite = previewSprite;

        buyButton.interactable = true;

        previewCoroutine = StartCoroutine(PreviewTimer());
    }

    IEnumerator PreviewTimer()
    {
        yield return new WaitForSeconds(previewDuration);

        // SADECE sprite geri döner, BUY kalır
        ResetSpriteOnly();
    }

    void ResetSpriteOnly()
    {
        targetImage.sprite = currentSprite;
    }

    // BUY BUTONU
    public void BuySkin()
    {
        if (previewCoroutine != null)
            StopCoroutine(previewCoroutine);

        currentSprite = previewSprite;
        targetImage.sprite = currentSprite;

        SaveSkin(currentSprite.name);

        buyButton.interactable = false;
    }

    // PANEL KAPANIRSA / İPTAL
    public void CancelAll()
    {
        if (previewCoroutine != null)
            StopCoroutine(previewCoroutine);

        targetImage.sprite = currentSprite;
        buyButton.interactable = false;
    }

    // =====================
    // SAVE / LOAD
    // =====================

    void SaveSkin(string skinName)
    {
        PlayerPrefs.SetString(SAVE_KEY, skinName);
        PlayerPrefs.Save();
    }

    void LoadSkin()
    {
        string savedSkin = PlayerPrefs.GetString(SAVE_KEY, "");

        if (!string.IsNullOrEmpty(savedSkin))
        {
            Sprite loadedSprite = Resources.Load<Sprite>("Skins/" + savedSkin);

            if (loadedSprite != null)
            {
                currentSprite = loadedSprite;
                targetImage.sprite = currentSprite;
                return;
            }
        }

        // Eğer kayıt yoksa veya bulunamazsa
        currentSprite = targetImage.sprite;
    }
}
