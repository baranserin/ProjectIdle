using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

#region ProductData & UnlockCondition (AYNI DOSYADA)

// ProductType enum'un ProductConfig'te tanımlı olduğundan emin olun:
// public enum ProductType { Tea, Coffee, Dessert, Barista }

[Serializable]
public class ProductData
{
    public ProductConfig config;
    [NonSerialized] public int level;
    [NonSerialized] public float incomeMultiplier = 1f; // LevelBoost vb. ürün-özel çarpan

    [Header("UI")]
    public GameObject uiObject;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI levelText2;
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

        // Temel gelir (seviye büyümesi)
        double baseValue = config.baseIncome * Math.Pow(config.incomeGrowth, level);

        // Ürüne özel yerel çarpan (level boost vb.)
        double localMul = incomeMultiplier;

        // Tür-bazlı dekorasyon çarpanı
        float decoMul = 1f;
        if (IncomeManager.Instance != null)
            decoMul = IncomeManager.Instance.GetEffectiveDecorationMultiplier(config.productType);

        return baseValue * localMul * decoMul;
    }

    public void UpdateUI()
    {
        if (levelText != null && levelText2 != null)
        {
            levelText.text = $"{level}";
            levelText2.text = "lv\n" + $"{level}";
        }

        if (incomeText != null)
            incomeText.text = GetIncome().ToString("F1", CultureInfo.InvariantCulture);

        if (upgradeCostText != null)
        {
            if (IncomeManager.Instance != null)
            {
                int levelsToBuy = IncomeManager.Instance.currentBuyMode switch
                {
                    IncomeManager.BuyMode.x1 => 1,
                    IncomeManager.BuyMode.x10 => 10,
                    IncomeManager.BuyMode.x50 => 50,
                    IncomeManager.BuyMode.Max => Mathf.Max(1, IncomeManager.Instance.CalculateMaxBuyableLevels(this)),
                    _ => 1
                };

                double totalCost = IncomeManager.Instance.CalculateTotalCost(this, levelsToBuy);
                upgradeCostText.text = IncomeManager.FormatMoneyStatic(totalCost);
            }
        }
    }

    public void ResetToBase()
    {
        level = config.baseLevel;
    }

    public void CheckLevelBoosts()
    {
        foreach (var boost in config.levelBoosts)
        {
            if (boost != null && !Mathf.Approximately(boost.incomeMultiplier, 0f) && level == boost.requiredLevel)
            {
                incomeMultiplier *= boost.incomeMultiplier;
                Debug.Log($"{config.productName} {boost.requiredLevel} seviyeye ulaştı! Gelir {boost.incomeMultiplier}x oldu.");
            }
        }
    }
}
[System.Serializable]
public class UnlockCondition
{
    [Header("Gerekli Ürün ve Seviyesi")]
    public ProductConfig requiredProductConfig; // << buradan config seçilecek
    public int requiredLevel;

    [Header("Toplam Para Şartı")]
    public bool requireTotalMoney = false;
    public double requiredMoney;

    [Header("Prestige Seviye Şartı")]
    public bool requirePrestigeLevel = false;
    public int requiredPrestigeLevel;
}

#endregion

public class IncomeManager : MonoBehaviour
{
    public static IncomeManager Instance;
    public DecorationIncome decorationIncome;

    public enum BuyMode { x1, x10, x50, Max }
    public BuyMode currentBuyMode = BuyMode.x1;

    [Header("Ürünler")]
    public List<ProductData> products;

    [Header("Genel")]
    public double totalMoney = 10f;
    public double prestigeMultiplier = 1.1;
    public int prestigeLevel = 0;
    public int prestigePoint = 0;
    public float upgradeMultiplier = 0f;

    [Header("Global Multipliers")]
    public float globalIncomeMultiplier = 1f;   // Barista vb. global etkiler

    // Event/Boost çarpanı
    public float temporaryMultiplier = 1f;

    [Header("UI")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI incomeText;
    public TextMeshProUGUI prestigeLevelText;
    public TextMeshProUGUI prestigePointText;
    public TextMeshProUGUI PassiveIncomeText;

    [Header("Sesler")]
    public AudioSource successSound;
    public AudioSource failSound;

    public double income;
    public double clickValue = 1;

    public GameObject PassiveIncomePanel;
    public GameObject collect1xButton;
    public GameObject collect2xButton;

    double offlineEarning;

    // 🔹 Tür-bazlı dekorasyon çarpan tablosu
    private float globalDecorationMultiplier = 1f;                // applyToAllTypes ile gelenler
    private readonly Dictionary<ProductType, float> typeDecorationMultiplier = new();

    private readonly HashSet<ProductType> unlockedMachines = new();



    void Awake()
    {
        Instance = this;
        InitTypeMultipliers();
    }

    private void InitTypeMultipliers()
    {
        typeDecorationMultiplier.Clear();
        foreach (ProductType t in Enum.GetValues(typeof(ProductType)))
            typeDecorationMultiplier[t] = 1f;
        globalDecorationMultiplier = 1f;
    }
    void Start()
    {
        LoadData();
        InactiveIncome();
        CheckUnlocks();
        InvokeRepeating(nameof(GeneratePassiveIncome), 1f, 1f);
        UpdateUI();

        // 🔹 Show upgrade arrows at startup (even if popup is inactive)
        StartCoroutine(InitializeUpgradeArrowsAfterStartup());

        // DecorationIncome.Start() içinde satın alınmış dekorasyonlar yeniden uygulanır.
    }

    private IEnumerator InitializeUpgradeArrowsAfterStartup()
    {
        // wait 1 frame to ensure allProductCards & IncomeManager are ready
        yield return null;

        // find all ProductPurchasePanels, even if inactive
        var panels = FindObjectsOfType<ProductPurchasePanel>(true);
        foreach (var panel in panels)
        {
            panel.UpdateUpgradeArrow();
        }
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

        foreach (var p in products)
        {
            p.level = p.config.baseLevel;
            p.incomeMultiplier = 1f;
        }

        totalMoney = 10f;
        prestigeLevel = 0;
        prestigePoint = 0;

        // Dekorasyon ve tür çarpanlarını sıfırla
        ResetDecorationMultipliers();
        if (decorationIncome != null)
            decorationIncome.ResetDecorations();

        UpdateUI();

        Debug.Log("🔁 ResetAllData tamamlandı.");
    }

    double GetTotalIncome()
    {
        double total = 0;
        foreach (var p in products)
            total += p.GetIncome();

        // Prestige + event + global çarpanları burada uygulanır
        total *= prestigeMultiplier;
        total *= temporaryMultiplier;
        total *= globalIncomeMultiplier;

        return total;
    }

    public void AddMoney(double amount)
    {
        totalMoney += amount;
        UpdateUI();
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

        // 🔴 Eğer ürün kilitliyse → upgrade YASAK, level artmasın, görünürlük değişmesin
        if (!IsProductUnlocked(p))
        {
            Debug.Log($"❌ {p.config.productName} is locked, cannot upgrade.");
            if (failSound != null)
                failSound.Play();
            return;
        }

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
            p.CheckLevelBoosts();

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
            PlayerPrefs.SetFloat($"Product_{i}_Multiplier", products[i].incomeMultiplier);
        }
        PlayerPrefs.SetString("TotalMoney", totalMoney.ToString());
        PlayerPrefs.SetString("PrestigeMultiplier", prestigeMultiplier.ToString());
        PlayerPrefs.SetInt("PrestigeLevel", prestigeLevel);
        PlayerPrefs.SetFloat("GlobalIncomeMultiplier", globalIncomeMultiplier);
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

            products[i].incomeMultiplier = PlayerPrefs.GetFloat($"Product_{i}_Multiplier", 1f);
        }

        totalMoney = Convert.ToDouble(PlayerPrefs.GetString("TotalMoney", "10"));
        prestigeMultiplier = Convert.ToDouble(PlayerPrefs.GetString("PrestigeMultiplier", "1"));
        prestigeLevel = PlayerPrefs.GetInt("PrestigeLevel", 0);
        globalIncomeMultiplier = PlayerPrefs.GetFloat("GlobalIncomeMultiplier", 1f);

        // Tür çarpan tablolarını her açılışta sıfırla.
        // DecorationIncome.Start() satın alınmış dekorasyonları yeniden uygular.
        ResetDecorationMultipliers();
        LoadMachineUnlocks();

    }

    public void InactiveIncome()
    {
        if (PlayerPrefs.HasKey("lastExitTime"))
        {
            string lastTimeStr = PlayerPrefs.GetString("lastExitTime");
            DateTime lastTime = DateTime.Parse(lastTimeStr);
            TimeSpan fark = DateTime.Now - lastTime;

            double secondsAway = fark.TotalSeconds;
            double incomePerSecond = GetTotalIncome();
            offlineEarning = incomePerSecond * secondsAway;

            if (PassiveIncomeText != null)
                PassiveIncomeText.text = "Coffees were sold, your vault is full! " + FormatMoneyStatic(offlineEarning);

            PassiveIncomePanel.SetActive(offlineEarning > 0);
        }
    }

    public void CollectOffline1x()
    {
        totalMoney += offlineEarning;
        offlineEarning = 0;

        if (PassiveIncomeText != null)
            PassiveIncomeText.text = "Offline Kazanç toplandı!";

        PassiveIncomePanel.SetActive(false);
        UpdateUI();
    }

    public void CollectOffline2x()
    {
        totalMoney += offlineEarning * 2;
        offlineEarning = 0;

        if (PassiveIncomeText != null)
            PassiveIncomeText.text = "Offline Kazanç 2x toplandı!";

        PassiveIncomePanel.SetActive(false);
        UpdateUI();
    }

    // 🔻 Geriye uyumluluk — eski çağrılar tüm türlere uygular
    public void AddDecorationMultiplier(float multiplier)
    {
        AddDecorationMultiplier(multiplier, null, true);
    }

    // 🔸 Yeni API: Tür-bazlı veya global dekorasyon çarpanı ekle
    public void AddDecorationMultiplier(float multiplier, List<ProductType> types, bool applyToAll)
    {
        if (applyToAll || types == null || types.Count == 0)
        {
            globalDecorationMultiplier *= multiplier;
        }
        else
        {
            foreach (var t in types)
            {
                if (!typeDecorationMultiplier.ContainsKey(t))
                    typeDecorationMultiplier[t] = 1f;

                typeDecorationMultiplier[t] *= multiplier;
            }
        }

        RecomputeAllIncomes();
    }

    // 🔸 Tür-bazlı çarpanları sıfırla (DecorationIncome yüklerken/Reset’te çağırır)
    public void ResetDecorationMultipliers()
    {
        InitTypeMultipliers();
        RecomputeAllIncomes();
    }

    // 🔸 Bir ürün için efektif dekorasyon çarpanı
    public float GetEffectiveDecorationMultiplier(ProductType type)
    {
        float perType = typeDecorationMultiplier.TryGetValue(type, out var m) ? m : 1f;
        return globalDecorationMultiplier * perType;
    }

    // Çarpan değişimlerinde UI’ı tazelemek yeterli
    private void RecomputeAllIncomes()
    {
        UpdateUI();
    }

    public void ApplyGlobalIncomeMultiplier(float mult)
    {
        globalIncomeMultiplier *= mult;   // örn. 1.2f => +%20
        UpdateUI();
        SaveData();
    }

    public void ApplyCategoryUpgrade(ProductType type, float multiplier)
    {
        if (type == ProductType.Barista)
        {
            ApplyGlobalIncomeMultiplier(multiplier);
            return;
        }

        foreach (var p in products)
        {
            if (p.config.productType == type)
            {
                p.incomeMultiplier *= multiplier; // ürün yerel çarpanı
                p.UpdateUI();
            }
        }

        UpdateUI();
    }

    // 🔹 İç helper: index ile aç (products listesindeki sıra)
    public void UnlockProductByIndex(int index, bool save = true)
    {
        if (index < 0 || index >= products.Count)
        {
            Debug.LogWarning($"UnlockProductByIndex: invalid index {index}");
            return;
        }

        var product = products[index];

        if (product.uiObject != null)
            product.uiObject.SetActive(true);

        // PlayerPrefs ile kalıcı işaretle
        PlayerPrefs.SetInt($"Product_{index}_ForcedUnlocked", 1);

        if (save)
            PlayerPrefs.Save();

        Debug.Log($"[IncomeManager] Forced unlocked product by index {index}: {product.config.productName}");
    }

    // 🔹 Tek bir ProductConfig ile aç
    public void UnlockProductByConfig(ProductConfig config, bool save = true)
    {
        if (config == null)
        {
            Debug.LogWarning("UnlockProductByConfig: config is null.");
            return;
        }

        int index = products.FindIndex(p => p.config == config);
        if (index == -1)
        {
            Debug.LogWarning($"UnlockProductByConfig: config '{config.productName}' not found in products list.");
            return;
        }

        UnlockProductByIndex(index, save);
    }

    // 🔹 Birden fazla config’i aynı anda aç
    public void UnlockProductsByConfigs(IEnumerable<ProductConfig> configs)
    {
        if (configs == null)
            return;

        bool changed = false;

        foreach (var cfg in configs)
        {
            if (cfg == null) continue;

            int index = products.FindIndex(p => p.config == cfg);
            if (index == -1)
            {
                Debug.LogWarning($"UnlockProductsByConfigs: config '{cfg.productName}' not found in products list.");
                continue;
            }

            UnlockProductByIndex(index, false); // her seferinde Save çağırma
            changed = true;

            Debug.Log($"✔ PRODUCT UNLOCKED → {cfg.productName}");
        }

        if (changed)
            PlayerPrefs.Save();
    }

    // 🔹 Bu ürün şu an unlocked mı? (sadece config ve şartlara göre karar verir)
    private bool IsProductUnlocked(ProductData product)
    {
        var config = product.config;

        // 1️⃣ Eğer bu ürün makineye bağlıysa, önce makineye bak
        if (config.requiresMachine)
        {
            // O ürünün türüne ait makine açılmamışsa → direkt false
            if (!IsMachineUnlocked(config.productType))
                return false;
        }

        // 2️⃣ Makine engeli yoksa (ya da makine zaten açıksa) normal unlock mantığına geç

        // Hiç kilitli değilse (isLockedInitially = false) → her zaman açık
        if (!config.isLockedInitially)
            return true;

        bool allConditionsMet = true;

        foreach (var condition in config.unlockConditions)
        {
            // A) Belirli bir ürünün belirli seviyeye gelmesi
            if (condition.requiredProductConfig != null)
            {
                var requiredProduct = products.Find(p => p.config == condition.requiredProductConfig);
                if (requiredProduct == null || requiredProduct.level < condition.requiredLevel)
                {
                    allConditionsMet = false;
                    break;
                }
            }

            // B) Toplam para şartı
            if (condition.requireTotalMoney && totalMoney < condition.requiredMoney)
            {
                allConditionsMet = false;
                break;
            }

            // C) Prestige seviye şartı
            if (condition.requirePrestigeLevel && prestigeLevel < condition.requiredPrestigeLevel)
            {
                allConditionsMet = false;
                break;
            }
        }

        return allConditionsMet;
    }



    public void CheckUnlocks()
    {
        for (int i = 0; i < products.Count; i++)
        {
            var product = products[i];
            if (product.uiObject == null)
                continue;

            bool unlocked = IsProductUnlocked(product);

            // Ürün unlocked ise UI’ı aç, değilse kapalı kalsın
            product.uiObject.SetActive(unlocked);

            if (unlocked)
            {
                Debug.Log($"UNLOCKED by conditions → {product.config.productName}");
            }
        }
    }

    private void LoadMachineUnlocks()
    {
        unlockedMachines.Clear();

        foreach (ProductType t in Enum.GetValues(typeof(ProductType)))
        {
            int flag = PlayerPrefs.GetInt($"Machine_{t}_Unlocked", 0);
            if (flag == 1)
                unlockedMachines.Add(t);
        }
    }

    private void SaveMachineUnlock(ProductType type)
    {
        PlayerPrefs.SetInt($"Machine_{type}_Unlocked", 1);
        PlayerPrefs.Save();
    }

    // Makineyi kalıcı olarak açan fonksiyon
    public void UnlockMachine(ProductType type)
    {
        if (!unlockedMachines.Contains(type))
        {
            unlockedMachines.Add(type);
            SaveMachineUnlock(type);
            CheckUnlocks(); // ürünlerin UI’ını tekrar değerlendir
            Debug.Log($"[IncomeManager] Machine unlocked for type: {type}");
        }
    }

    // Bu tür için makine açık mı?
    public bool IsMachineUnlocked(ProductType type)
    {
        return unlockedMachines.Contains(type);
    }


}




