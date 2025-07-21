using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

[Serializable]
public class ProductData
{
    public ProductConfig config;

    [NonSerialized] public int level;

    [Header("UI")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI incomeText;
    public TextMeshProUGUI upgradeCostText;


    public double GetMultiplier()
    {
        return config.baseMultiplier * Math.Pow(config.multiplierGrowth, level );
    }

    public double GetUpgradeCost()
    {
        return config.baseUpgradeCost * Math.Pow(config.costGrowth, level);
    }

    public double GetIncome()
    {

        if (level == 0)
        {
            return 0;
        }
        if(level == 1)
        {
            return config.baseIncome;
        }
        return (config.baseIncome + (config.incomeGrowth * level));
    }

    public void UpdateUI()
    {
        if (levelText != null)
            levelText.text = $"{config.productName} Lv.{level}";
        if (incomeText != null)
            incomeText.text = GetIncome().ToString("F1") + "/s";
        if (upgradeCostText != null)
            upgradeCostText.text = IncomeManager.FormatMoneyStatic(GetUpgradeCost());
    }

    public void ResetToBase()
    {
        level = config.baseLevel;   
    }
}



public class IncomeManager : MonoBehaviour
{

    public static IncomeManager Instance;

    public List<UpgradeConfig> upgradeFactor;

    [Header("Ürünler")]
    public List<ProductData> products;
    public ProductConfig upgradeConfig;

    [Header("Genel")]
    public double totalMoney = 1000f;
    public double prestigeMultiplier = 1.1;
    public int prestigeLevel = 0;
    public int prestigePoint = 0;
    public float upgradeMultiplier = 0f;
   

    [Header("UI")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI incomeText;
    public TextMeshProUGUI prestigeLevelText;
    public TextMeshProUGUI prestigePointText;

    [Header("Sesler")]                   
    public AudioSource successSound;      
    public AudioSource failSound;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        LoadData();
        InactiveIncome();
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

    void GeneratePassiveIncome()
    {
        double income = GetTotalIncome() * upgradeMultiplier;
        totalMoney += income;

        if (incomeText != null)
            incomeText.text = FormatMoneyStatic(income) + "/s";

        UpdateUI();
    }

    public void ResetAllData()
    {
        foreach (var config in upgradeFactor)
        {
            PlayerPrefs.DeleteKey("Upgrade_Buyed_" + config.upgradeName);
        }

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        LoadData();
    }
    double GetTotalIncome()
    {
        double total = 0;
        foreach (var p in products)
            total += p.GetIncome();

        return total;
    }

    public void UpgradeProduct(int index)
    {
        if (index < 0 || index >= products.Count)
            return;

        var p = products[index];
        double cost = p.GetUpgradeCost();

        if (totalMoney >= cost)
        {
            totalMoney -= cost;
            p.level++;
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
        // Diğer ürünleri yükledikten sonra:
        foreach (var config in upgradeFactor)
        {
            string key = "Upgrade_Buyed_" + config.upgradeName;
            if (PlayerPrefs.GetInt(key, 0) == 1)
            {
                upgradeMultiplier += config.upgradeFactor;
            }
        }

        for (int i = 0; i < products.Count; i++)
        {
            string key = $"Product_{i}_Level";

            if (PlayerPrefs.HasKey(key))
            {
                products[i].level = PlayerPrefs.GetInt(key);
            }
            else
            {
                products[i].level = products[i].config.baseLevel;
            }
        }

        totalMoney = Convert.ToDouble(PlayerPrefs.GetString("TotalMoney", "10"));
        prestigeMultiplier = Convert.ToDouble(PlayerPrefs.GetString("PrestigeMultiplier", "1"));
        prestigeLevel = PlayerPrefs.GetInt("PrestigeLevel",0);
    }

    public void InactiveIncome()
    {
        if (PlayerPrefs.HasKey("lastExitTime"))
        {
            string lastTimeStr = PlayerPrefs.GetString("lastExitTime");
            DateTime lastTime = DateTime.Parse(lastTimeStr);
            TimeSpan fark = DateTime.Now - lastTime;

            double secondsAway = fark.TotalSeconds;

            double incomePerSecond = GetTotalIncome() * upgradeMultiplier * prestigeMultiplier;
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
            upgradeMultiplier += config.upgradeFactor;
            SaveUpgrade(config);
        }
    }

    private void SaveUpgrade(UpgradeConfig config)
    {
        string key = "Upgrade_Buyed_" + config.upgradeName;
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
    }
}
