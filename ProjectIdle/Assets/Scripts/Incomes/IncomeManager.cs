using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;



[Serializable]
public class ProductData
{
    public ProductConfig config;
    [NonSerialized] public int level;
    [NonSerialized] public float incomeMultiplier = 1f;

    [Header("UI")]
    public GameObject uiObject;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI incomeText;
    public TextMeshProUGUI upgradeCostText;

    public double GetUpgradeCost()
    {
        return config.baseUpgradeCost * Math.Pow(config.costGrowth, level);
    }

    public double GetIncome()
    {
        if (level == 0)
            return 0;

        return config.baseIncome * Math.Pow(config.incomeGrowth, level) + incomeMultiplier;
    }

    public void UpdateUI()
    {
        int levelsToBuy = 1;
        switch (IncomeManager.Instance.currentBuyMode)
        {
            case IncomeManager.BuyMode.x10: levelsToBuy = 10; break;
            case IncomeManager.BuyMode.x50: levelsToBuy = 50; break;
            case IncomeManager.BuyMode.Max:
                levelsToBuy = IncomeManager.Instance.CalculateMaxBuyableLevels(this);
                if (levelsToBuy == 0) levelsToBuy = 1; // Ensure at least 1 level is shown
                break;
        }
        double totalCost = IncomeManager.Instance.CalculateTotalCost(this, levelsToBuy);

        // Update UI
        if (levelText != null) levelText.text = $"Lvl {level}";
        if (upgradeCostText != null)
            upgradeCostText.text = IncomeManager.FormatMoneyStatic(totalCost);
        if (incomeText != null)
            incomeText.text = GetIncome().ToString("F1", CultureInfo.InvariantCulture) + "/s";
    }


    public void ResetToBase()
    {
        level = config.baseLevel;
    }
}

[System.Serializable]
public class UnlockCondition
{
    public string requiredProductName;
    public int requiredLevel;

    public bool requireTotalMoney = false;
    public double requiredMoney;

    public bool requirePrestigeLevel = false;
    public int requiredPrestigeLevel;
}

public class IncomeManager : MonoBehaviour
{
    public UpgradeButtonManager upgradeButtonManager;
    public static IncomeManager Instance;
    public DecorationIncome decorationIncome;

    public List<UpgradeConfig> upgradeFactor;

    public enum BuyMode
    {
        x1,
        x10,
        x50,
        Max
    }

    public BuyMode currentBuyMode = BuyMode.x1;


    [Header("Ürünler")]
    public List<ProductData> products;
    public List<UpgradeConfig> upgradeConfig;

    [Header("Genel")]
    public double totalMoney = 10f;
    public double prestigeMultiplier = 1.1;
    public int prestigeLevel = 0;
    public int prestigePoint = 0;
    public float upgradeMultiplier = 0f;

    // 🔥 Event sistemi için multiplier
    public float temporaryMultiplier = 1f;

    [Header("UI")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI incomeText;
    public TextMeshProUGUI prestigeLevelText;
    public TextMeshProUGUI prestigePointText;

    [Header("Sesler")]
    public AudioSource successSound;
    public AudioSource failSound;

    [Header("Unlockable UI Elements")]
    public GameObject orangeTeaUpgradeButton;

    public double income;
    public double clickValue = 1;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        LoadData();
        InactiveIncome();
        CheckUnlocks();
        InvokeRepeating(nameof(GeneratePassiveIncome), 1f, 1f);
        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveData();
            Debug.Log("⏸️ Uygulama duraklatıldı, veriler kaydedildi.");
        }
    }

    void GeneratePassiveIncome()
    {
        income = GetTotalIncome();

        if (BoostIncome.Instance != null && BoostIncome.Instance.IsBoostActive())
        {
            income *= 2;
        }

        totalMoney += income;

        if (incomeText != null)
            incomeText.text = FormatMoneyStatic(income) + "/s";

        UpdateUI();
    }

    public void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        upgradeFactor.Clear();

        foreach (var p in products)
        {
            p.level = p.config.baseLevel;
            p.incomeMultiplier = 1f;
        }

        totalMoney = 10f;
        prestigeLevel = 0;
        prestigePoint = 0;
        decorationIncome.ResetDecorations();
        upgradeButtonManager.ResetButtons();
        UpdateUI();

        Debug.Log("🔁 ResetAllData tamamlandı.");
    }

    double GetTotalIncome()
    {
        double total = 0;
        foreach (var p in products)
            total += p.GetIncome();

        // 🔥 Event çarpanı da uygula
        return total * temporaryMultiplier;
    }

    public void AddMoney(double amount)
    {
        totalMoney += amount;
        UpdateUI();
    }

    public void CheckUnlocks()
    {
        foreach (var product in products)
        {
            var config = product.config;

            if (!config.isLockedInitially || product.uiObject == null)
                continue;

            if (product.uiObject.activeSelf)
                continue;

            bool allConditionsMet = true;

            foreach (var condition in config.unlockConditions)
            {
                if (!string.IsNullOrEmpty(condition.requiredProductName))
                {
                    var requiredProduct = products.Find(p => p.config.productName == condition.requiredProductName);
                    if (requiredProduct == null || requiredProduct.level < condition.requiredLevel)
                    {
                        allConditionsMet = false;
                        break;
                    }
                }

                if (condition.requireTotalMoney && totalMoney < condition.requiredMoney)
                {
                    allConditionsMet = false;
                    break;
                }

                if (condition.requirePrestigeLevel && prestigeLevel < condition.requiredPrestigeLevel)
                {
                    allConditionsMet = false;
                    break;
                }
            }

            if (allConditionsMet)
            {
                product.uiObject.SetActive(true);
            }
        }
    }
    public int CalculateMaxBuyableLevels(ProductData p)
    {
        int maxLevels = 0;
        double availableMoney = totalMoney;

        while (true)
        {
            double cost = p.config.baseUpgradeCost * Math.Pow(p.config.costGrowth, p.level + maxLevels);
            if (availableMoney >= cost)
            {
                availableMoney -= cost;
                maxLevels++;
            }
            else break;
        }

        return maxLevels;
    }
    public double CalculateTotalCost(ProductData p, int levelsToBuy)
    {
        double totalCost = 0;
        for (int i = 0; i < levelsToBuy; i++)
        {
            totalCost += p.config.baseUpgradeCost * Math.Pow(p.config.costGrowth, p.level + i);
        }
        return totalCost;
    }

    public void UpgradeProduct(int index)
    {
        if (index < 0 || index >= products.Count)
            return;

        var p = products[index];

        int levelsToBuy = 1;
        switch (currentBuyMode)
        {
            case BuyMode.x1: levelsToBuy = 1; break;
            case BuyMode.x10: levelsToBuy = 10; break;
            case BuyMode.x50: levelsToBuy = 50; break;
            case BuyMode.Max: levelsToBuy = CalculateMaxBuyableLevels(p); break;
        }

        double totalCost = CalculateTotalCost(p, levelsToBuy);

        if (totalMoney >= totalCost)
        {
            totalMoney -= totalCost;
            p.level += levelsToBuy;

            CheckUnlocks();
            p.UpdateUI();
            UpdateUI();

            if (successSound != null)
                successSound.Play();
        }
        else
        {
            if (failSound != null)
                failSound.Play();
        }
    }
    public void SetBuyMode(int mode)
    {
        mode = Mathf.Clamp(mode, 0, 3);
        currentBuyMode = (BuyMode)mode;
        foreach (var p in products)
            p.UpdateUI();
    }

    public void UpdateUI()
    {
        if (moneyText != null)
            moneyText.text = FormatMoneyStatic(totalMoney);
        if (prestigeLevelText != null)
            prestigeLevelText.text = prestigeLevel.ToString();
        if (prestigePointText != null)
            prestigePointText.text = prestigePoint.ToString();
        foreach (var p in products)
            p.UpdateUI();
    }

    public static string FormatMoneyStatic(double amount)
    {
        if (amount >= 1_000_000_000) return (amount / 1_000_000_000).ToString("F1") + "B";
        if (amount >= 1_000_000) return (amount / 1_000_000).ToString("F1") + "M";
        if (amount >= 1_000) return (amount / 1_000).ToString("F1") + "K";
        return Math.Floor(amount).ToString();
    }

    public void Prestige()
    {
        prestigeLevel++;
        foreach (var p in products)
            p.ResetToBase();
        totalMoney = 1000f;
        prestigePoint = 3 * prestigeLevel;
        UpdateUI();
        SaveData();
    }

    public void SaveData()
    {
        for (int i = 0; i < products.Count; i++)
        {
            PlayerPrefs.SetInt($"Product_{i}_Level", products[i].level);
        }
        PlayerPrefs.SetString("TotalMoney", totalMoney.ToString());
        PlayerPrefs.SetString("PrestigeMultiplier", prestigeMultiplier.ToString());
        PlayerPrefs.SetInt("PrestigeLevel", prestigeLevel);
        string timeNow = DateTime.Now.ToString("O");
        PlayerPrefs.SetString("lastExitTime", timeNow);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        for (int i = 0; i < products.Count; i++)
        {
            string key = $"Product_{i}_Level";

            if (PlayerPrefs.HasKey(key))
                products[i].level = PlayerPrefs.GetInt(key);
            else
                products[i].level = products[i].config.baseLevel;
        }

        totalMoney = Convert.ToDouble(PlayerPrefs.GetString("TotalMoney", "10"));
        prestigeMultiplier = Convert.ToDouble(PlayerPrefs.GetString("PrestigeMultiplier", "1"));
        prestigeLevel = PlayerPrefs.GetInt("PrestigeLevel", 0);
        upgradeButtonManager.CreateButtonsFromConfigs();
    }

    public void InactiveIncome()
    {
        if (PlayerPrefs.HasKey("lastExitTime"))
        {
            string lastTimeStr = PlayerPrefs.GetString("lastExitTime");
            DateTime lastTime = DateTime.Parse(lastTimeStr);
            TimeSpan fark = DateTime.Now - lastTime;

            double secondsAway = fark.TotalSeconds;

            double incomePerSecond = GetTotalIncome() * prestigeMultiplier;
            double offlineEarning = incomePerSecond * secondsAway;

            totalMoney += offlineEarning;

            Debug.Log($"Sen yokken {secondsAway:F0} saniye geçti. Kazanılan para: {FormatMoneyStatic(offlineEarning)}");
        }
    }

    public void ApplyUpgrade(UpgradeConfig config)
    {
        if (!upgradeFactor.Contains(config))
        {
            upgradeFactor.Add(config);
            SaveUpgrade(config);

            if (!string.IsNullOrEmpty(config.targetProductName))
            {
                var product = products.Find(p => p.config.productName == config.targetProductName);
                if (product != null)
                {
                    product.incomeMultiplier += config.upgradeFactor;
                    Debug.Log($"🔹 Upgrade uygulandı: {product.config.productName} x{config.upgradeFactor}");
                    product.UpdateUI();
                }
            }
            else
            {
                foreach (var product in products)
                {
                    product.incomeMultiplier += config.upgradeFactor;
                    Debug.Log($"🔸 Global upgrade: {product.config.productName} x{config.upgradeFactor}");
                    product.UpdateUI();
                }
            }

            UpdateUI();
        }
    }

    public void AddDecorationMultiplier(float multiplier)
    {
        foreach (var product in products)
        {
            product.incomeMultiplier += multiplier;
            product.UpdateUI();
        }

        UpdateUI();
    }

    private void SaveUpgrade(UpgradeConfig config)
    {
        string key = "Upgrade_Buyed_" + config.upgradeName;
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
    }
}
