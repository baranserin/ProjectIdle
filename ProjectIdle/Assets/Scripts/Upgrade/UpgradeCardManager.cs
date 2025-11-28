using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UpgradeCardData
{
    public Sprite objectImage;
    public string objectName;
    public int price;

    public ProductType targetType;
    public float multiplier;

    public TMP_Text descriptionText;

    [Header("Image Adjustments")]
    public Vector2 imageOffset = Vector2.zero;
    public float imageScale = 0.33f;

    [HideInInspector] public bool isBought = false;
}

public class UpgradeCardManager : MonoBehaviour
{
    [Header("Card Data")]
    public UpgradeCardData[] upgrades;

    [Header("UI References")]
    public Image objectImageUI;
    public TMP_Text objectNameUI;
    public TMP_Text priceTextUI;
    public Image[] pageIndicators;

    [Header("Page Indicator Sprites")]
    public Sprite dotOn;
    public Sprite dotOff;

    private int currentIndex = 0;

    #region Unity

    void Start()
    {
        LoadUpgradesFromSave();

        int startIndex = FindFirstAvailableIndex();
        ShowCard(startIndex);
    }

    #endregion

    #region Public UI Methods

    public void ShowCard(int index)
    {
        if (index < 0 || index >= upgrades.Length) return;

        currentIndex = index;

        var upgrade = upgrades[index];

        // Görsel
        objectImageUI.sprite = upgrade.objectImage;
        objectImageUI.SetNativeSize();
        RectTransform rt = objectImageUI.rectTransform;
        rt.anchoredPosition = upgrade.imageOffset;
        rt.localScale = Vector3.one * upgrade.imageScale;

        // İsim & fiyat / durum
        objectNameUI.text = upgrade.objectName;

        // Açıklama textlerini yönet
        foreach (var upg in upgrades)
        {
            if (upg.descriptionText != null)
                upg.descriptionText.gameObject.SetActive(false);
        }

        if (upgrade.descriptionText != null)
            upgrade.descriptionText.gameObject.SetActive(true);

        UpdatePageIndicators();
        UpdateCardVisual(currentIndex);
    }

    public void NextCard()
    {
        if (upgrades.Length == 0) return;

        int nextIndex = (currentIndex + 1) % upgrades.Length;
        ShowCard(nextIndex);
    }

    public void PreviousCard()
    {
        if (upgrades.Length == 0) return;

        int prevIndex = (currentIndex - 1 + upgrades.Length) % upgrades.Length;
        ShowCard(prevIndex);
    }

    public void BuyUpgrade()
    {
        var upgrade = upgrades[currentIndex];

        // 1) Sıralı açılma kontrolü
        if (!IsUnlocked(currentIndex))
        {
            Debug.Log("❌ Bu upgrade henüz kilitli. Önce önceki upgrade'i satın almalısın.");
            return;
        }

        // 2) Zaten alınmış mı?
        if (upgrade.isBought)
        {
            Debug.Log("Upgrade zaten alınmış!");
            return;
        }

        // 3) Para kontrolü
        if (IncomeManager.Instance.totalMoney >= upgrade.price)
        {
            IncomeManager.Instance.totalMoney -= upgrade.price;
            IncomeManager.Instance.ApplyCategoryUpgrade(upgrade.targetType, upgrade.multiplier);

            upgrade.isBought = true;
            SaveSingleUpgrade(currentIndex);

            Debug.Log($"✅ Bought upgrade: {upgrade.objectName}, applied x{upgrade.multiplier} to {upgrade.targetType}");
            IncomeManager.Instance.UpdateUI();

            UpdateCardVisual(currentIndex);

            // İstersen otomatik olarak bir sonraki uygun upgrade'e geç
            int nextIndex = FindFirstAvailableIndex();
            ShowCard(nextIndex);
        }
        else
        {
            Debug.Log("❌ Yeterli paran yok!");
        }
    }

    #endregion

    #region Visual Helpers

    void UpdatePageIndicators()
    {
        for (int i = 0; i < pageIndicators.Length; i++)
        {
            if (i < upgrades.Length)
                pageIndicators[i].sprite = (i == currentIndex) ? dotOn : dotOff;
        }
    }

    /// <summary>
    /// Kartın durumuna göre (kilitli / alınmış / alınmamış) görünümü ayarla.
    /// </summary>
    private void UpdateCardVisual(int index)
    {
        var upgrade = upgrades[index];
        bool unlocked = IsUnlocked(index);

        if (!unlocked)
        {
            // Kilitliyse
            priceTextUI.text = "LOCKED";
            priceTextUI.color = Color.gray;
            objectImageUI.color = new Color(1f, 1f, 1f, 0.4f);
            return;
        }

        // Kilit açık ama satın alınmamış veya alınmış
        objectImageUI.color = Color.white;

        if (upgrade.isBought)
        {
            priceTextUI.text = "BOUGHT";
            priceTextUI.color = Color.green;
        }
        else
        {
            priceTextUI.text = upgrade.price.ToString();
            priceTextUI.color = Color.white;
        }
    }

    #endregion

    #region Unlock Logic (Sırayla açılma)

    /// <summary>
    /// index = 0 her zaman açık. Diğerleri bir önceki upgrade alındıysa açılır.
    /// </summary>
    private bool IsUnlocked(int index)
    {
        if (index <= 0) return true;
        return upgrades[index - 1].isBought;
    }

    /// <summary>
    /// İlk uygun (kilidi açık ve satın alınmamış) upgrade indexini bul.
    /// Hepsi alınmışsa 0'ı döndür.
    /// </summary>
    private int FindFirstAvailableIndex()
    {
        for (int i = 0; i < upgrades.Length; i++)
        {
            if (IsUnlocked(i) && !upgrades[i].isBought)
                return i;
        }

        // Hepsi alınmışsa en son kartı gösterebilirsin, ama ben 0'a döndürüyorum.
        return 0;
    }

    #endregion

    #region Save & Load (Aldığım upgradeleri kaydet)

    private string GetUpgradeKey(int index)
    {
        // İstersen oyun ismi vs ekleyebilirsin: "MyGame_UpgradeBought_" + index
        return "UpgradeBought_" + index;
    }

    /// <summary>
    /// Tüm upgradelerin durumunu PlayerPrefs'ten yükler.
    /// </summary>
    private void LoadUpgradesFromSave()
    {
        for (int i = 0; i < upgrades.Length; i++)
        {
            string key = GetUpgradeKey(i);
            int value = PlayerPrefs.GetInt(key, 0); // 0: alınmamış, 1: alınmış
            upgrades[i].isBought = (value == 1);
        }
    }

    /// <summary>
    /// Sadece tek bir upgrade'in satın alma durumunu kaydeder.
    /// Upgrade alınırken çağrıyoruz.
    /// </summary>
    private void SaveSingleUpgrade(int index)
    {
        string key = GetUpgradeKey(index);
        PlayerPrefs.SetInt(key, upgrades[index].isBought ? 1 : 0);
        PlayerPrefs.Save();
    }

    #endregion
}
