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
        return level * GetMultiplier();
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

    [Header("Ürünler")]
    public List<ProductData> products;

    [Header("Genel")]
    public double totalMoney = 10f;
    public double prestigeMultiplier = 1.1;
    public int prestigeLevel = 0;
    public int prestigePoint = 0;
   

    [Header("UI")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI incomeText;
    public TextMeshProUGUI prestigeLevelText;
    public TextMeshProUGUI prestigePointText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        LoadData();
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
        double income = GetTotalIncome();
        totalMoney += income;

        if (incomeText != null)
            incomeText.text = FormatMoneyStatic(income) + "/s";

        UpdateUI();
    }

    public void ResetAllData()
    {
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
        totalMoney = 10f;
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
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
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
}
