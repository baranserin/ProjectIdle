using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

[System.Serializable]
public class UpgradeCardData
{
    public Sprite objectImage;
    public string objectName;
    public double price;
    [HideInInspector] public double snapshotPrice;
    [HideInInspector] public bool snapshotInitialized = false;
    public ProductType targetType;
    public float multiplier;

    public TMP_Text descriptionText;

    [Header("Image Adjustments")]
    public Vector2 imageOffset = Vector2.zero;
    public float imageScale = 0.33f;

    [Header("Machine Upgrade")]
    public bool unlocksMachineForThisType = false;

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

    void Start()
    {
        LoadUpgradesFromSave();
        int startIndex = FindFirstAvailableIndex();
        ShowCard(startIndex);
    }

    public void ShowCard(int index)
    {
        if (index < 0 || index >= upgrades.Length) return;

        currentIndex = index;
        var upgrade = upgrades[index];

        objectImageUI.sprite = upgrade.objectImage;
        objectImageUI.SetNativeSize();
        RectTransform rt = objectImageUI.rectTransform;
        rt.anchoredPosition = upgrade.imageOffset;
        rt.localScale = Vector3.one * upgrade.imageScale;

        objectNameUI.text = upgrade.objectName;

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
    double GetSnapshotPrice(int index)
    {
        var upgrade = upgrades[index];

        if (upgrade.snapshotInitialized)
            return upgrade.snapshotPrice;

        double categoryIncome = IncomeManager.Instance.GetCategoryIncome(upgrade.targetType);

        double[] targetSeconds =
        {
        60,120,300,600,1200,
        2400,4800,9000,18000,36000
    };

        double price = categoryIncome * targetSeconds[index];

        upgrade.snapshotPrice = price;
        upgrade.snapshotInitialized = true;

        PlayerPrefs.SetString("UpgradeSnapshot_" + index, price.ToString());

        return price;
    }
    public void BuyUpgrade()
    {
        var upgrade = upgrades[currentIndex];

        if (!IsUnlocked(currentIndex))
        {
            Debug.Log("❌ Bu upgrade henüz kilitli.");
            return;
        }

        if (upgrade.isBought) return;

        double price = GetSnapshotPrice(currentIndex);
        if (IncomeManager.Instance.totalMoney >= price)
        {
            IncomeManager.Instance.totalMoney -= price;
            IncomeManager.Instance.ApplyCategoryUpgrade(upgrade.targetType, upgrade.multiplier);

            upgrade.isBought = true;
            SaveSingleUpgrade(currentIndex);

            if (upgrade.unlocksMachineForThisType && IncomeManager.Instance != null)
            {
                IncomeManager.Instance.UnlockMachine(upgrade.targetType);
            }

            IncomeManager.Instance.UpdateUI();
            UpdateCardVisual(currentIndex);

            int nextIndex = FindFirstAvailableIndex();
            ShowCard(nextIndex);
        }
        else
        {
            Debug.Log("❌ Yeterli paran yok!");
        }
    }

    public void NextCard() { int next = (currentIndex + 1) % upgrades.Length; ShowCard(next); }
    public void PreviousCard() { int prev = (currentIndex - 1 + upgrades.Length) % upgrades.Length; ShowCard(prev); }

    private void UpdateCardVisual(int index)
    {
        var upgrade = upgrades[index];
        bool unlocked = IsUnlocked(index);

        if (!unlocked)
        {
            priceTextUI.text = "LOCKED";
            priceTextUI.color = Color.gray;
            objectImageUI.color = new Color(1f, 1f, 1f, 0.4f);
            return;
        }

        objectImageUI.color = Color.white;
        if (upgrade.isBought)
        {
            priceTextUI.text = "BOUGHT";
            priceTextUI.color = Color.green;
        }
        else
        {
            // Debug için fiyatın ne geldiğini konsola yazdıralım
            // Debug.Log($"Card: {upgrade.objectName}, Price: {upgrade.price}");

            double price = GetSnapshotPrice(index);
            priceTextUI.text = FormatMoney(price);
            priceTextUI.color = Color.white;
        }
    }

    public string FormatMoney(double amount)
    {
        // Eğer miktar 0'dan küçük veya çok yakınsa 0 göster
        if (amount <= 0) return "0";

        if (amount >= 1e12d) return (amount / 1e12d).ToString("F2") + "T";
        if (amount >= 1e9d) return (amount / 1e9d).ToString("F2") + "B";
        if (amount >= 1e6d) return (amount / 1e6d).ToString("F2") + "M";
        if (amount >= 1e3d) return (amount / 1e3d).ToString("F2") + "K";

        return amount.ToString("F0"); // 1000 altındaki sayılar için virgülsüz
    }

    private bool IsUnlocked(int index)
    {
        if (index <= 0) return true;
        return upgrades[index - 1].isBought;
    }

    private int FindFirstAvailableIndex()
    {
        for (int i = 0; i < upgrades.Length; i++)
        {
            if (IsUnlocked(i) && !upgrades[i].isBought) return i;
        }
        return 0;
    }

    void UpdatePageIndicators()
    {
        for (int i = 0; i < pageIndicators.Length; i++)
        {
            if (i < upgrades.Length)
                pageIndicators[i].sprite = (i == currentIndex) ? dotOn : dotOff;
        }
    }

    #region Save & Load

    public void LoadUpgradesFromSave()
    {
        
        for (int i = 0; i < upgrades.Length; i++)
        {
            upgrades[i].isBought = (PlayerPrefs.GetInt("UpgradeBought_" + i, 0) == 1);
            string key = "UpgradeSnapshot_" + i;

            if (PlayerPrefs.HasKey(key))
            {
                upgrades[i].snapshotPrice = double.Parse(PlayerPrefs.GetString(key));
                upgrades[i].snapshotInitialized = true;
            }
        }
    }

    private void SaveSingleUpgrade(int index)
    {
        PlayerPrefs.SetInt("UpgradeBought_" + index, upgrades[index].isBought ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ResetUpgrades()
    {
        for (int i = 0; i < upgrades.Length; i++)
        {
            upgrades[i].isBought = false;
            PlayerPrefs.DeleteKey("UpgradeBought_" + i);
        }
        PlayerPrefs.Save();
        ShowCard(0);
    }

    #endregion
}