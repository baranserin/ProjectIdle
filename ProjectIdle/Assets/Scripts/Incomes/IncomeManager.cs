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
    public GameObject uiObject;
    public ProductConfig config;
    [NonSerialized] public int level;
    [NonSerialized] public float incomeMultiplier = 1f; // LevelBoost vb. ürün-özel çarpan

    [Header("UI")]
    public GameObject upgradeArrow;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI levelText2;
    public TextMeshProUGUI upgradeCostText;

    public double GetUpgradeCost()
    {
        if (level == 0) return config.baseUpgradeCost;

        double waveOffset = config.costSineAmplitude * Math.Sin((level * config.costSineFrequency) + config.costSinePhase);

        double effectiveLevel = level + waveOffset;

        return config.baseUpgradeCost * Math.Pow(config.costGrowth, effectiveLevel);
    }

    public double GetIncome()
    {
        if (level == 0) return 0;

        if (level == 1) return config.baseIncome;

        double waveOffset = config.incomeSineAmplitude * Math.Sin(level * config.incomeSineFrequency);

        double effectiveLevel = level + waveOffset;

        double baseValue = config.baseIncome * Math.Pow(config.incomeGrowth, effectiveLevel);

        double localMul = incomeMultiplier;

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
[Serializable]
public class MachineLockData
{
    public string machineName; // Sadece Inspector'da takip etmek için
    public ProductType type;
    public double price;
    public GameObject lockObject;
    public GameObject buyButton;
    public TextMeshProUGUI priceText;
    public GameObject firstProductUI;
}
#endregion

public class IncomeManager : MonoBehaviour
{
    public static IncomeManager Instance;
    public DecorationIncome decorationIncome;
    public UpgradeCardManager upgradeCardManager;

    public enum BuyMode { x1, x10, x50, Max }
    public BuyMode currentBuyMode = BuyMode.x1;

    [Header("Ürünler")]
    public List<ProductData> products;

    [Header("Genel")]
    public double totalMoney = 110f;
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



    [Header("Machine Locks")]
    public List<MachineLockData> machineLocks;


    private bool TeaBought;
    private bool CoffeeBought;
    private bool DessertBought;
    private const string TEA_KEY = "TeaBought";
    private const string COFFEE_KEY = "CoffeeBought";
    private const string DESSERT_KEY = "DessertBought";

    [Header("Sesler")]
    public AudioSource successSound;
    public AudioSource failSound;

    public double income;

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
        LoadMachineStates();
        SyncMachineProductUIs();
        InvokeRepeating(nameof(GeneratePassiveIncome), 1f, 1f);
        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
        RefreshUpgradeArrows();
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

        RefreshUpgradeArrows();
        UpdateUI();
    }

    public void ResetAllData()
    {
        PlayerPrefs.DeleteAll();

        // ✅ RAM'deki machine unlock durumunu da sıfırla
        unlockedMachines.Clear();

        PlayerPrefs.Save();

        foreach (var p in products)
        {
            p.level = p.config.baseLevel;
            p.incomeMultiplier = 1f;
        }

        totalMoney = 110f;

        ResetDecorationMultipliers();
        if (decorationIncome != null)
            decorationIncome.ResetDecorations();

        // ✅ Kilit UI'larını geri getir
        ActivateLocks();

        // ✅ Makine kilitlerini/tuşlarını Prefs+RAM durumuna göre yeniden ayarla
        LoadMachineStates();

        // ✅ İlk ürün kartlarını makine durumuna göre kapat/aç
        SyncMachineProductUIs();

        if (upgradeCardManager != null)
            upgradeCardManager.ResetUpgrades();

        UpdateUI();

        Debug.Log("🔁 ResetAllData tamamlandı.");
    }


    public double GetTotalIncome()
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
            RefreshUpgradeArrows();

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

    public static string FormatMoneyStatic(double value)
    {
        // Değer 1000'den küçükse virgülden sonra 1 basamak göster (Örn: 862.5)
        if (value < 1000)
        {
            // Not: Eğer telefonun dili Türkçeyse virgül (,), İngilizceyse nokta (.) görünür.
            // Her zaman nokta görünmesini istersen: value.ToString("F1", CultureInfo.InvariantCulture)
            return value.ToString("F1", CultureInfo.InvariantCulture);
        }

        // K, M, B, T (1000 ve üstü için mevcut mantık)
        string[] basicSuffixes = { "K", "M", "B", "T" };
        double[] basicValues = { 1e3, 1e6, 1e9, 1e12 };

        for (int i = basicValues.Length - 1; i >= 0; i--)
        {
            if (value >= basicValues[i])
            {
                double v = value / basicValues[i];
                if (v < 1000)
                    return v.ToString("F1", CultureInfo.InvariantCulture) + basicSuffixes[i];
            }
        }

        // T sonrası: a, b, c ... z
        double alphabetBase = 1e15;
        value /= alphabetBase;

        int suffixIndex = 0;
        while (value >= 1000 && suffixIndex < 25)
        {
            value /= 1000;
            suffixIndex++;
        }

        char suffixChar = (char)('a' + suffixIndex);
        return value.ToString("F1", CultureInfo.InvariantCulture) + suffixChar;
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

        totalMoney = Convert.ToDouble(PlayerPrefs.GetString("TotalMoney", "110"));
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

    // Bu metodun isminin ve parametresinin (int index) tam olarak böyle olduğundan emin ol
    public void BuyMachine(int index)
    {
        // Liste sınırlarını kontrol et
        if (index < 0 || index >= machineLocks.Count) return;

        var m = machineLocks[index];

        if (totalMoney >= m.price)
        {
            totalMoney -= m.price;
            UnlockMachine(m.type);

            // Görsel güncellemeler
            if (m.lockObject != null) m.lockObject.SetActive(false);
            if (m.buyButton != null) m.buyButton.SetActive(false);
            if (m.firstProductUI != null) m.firstProductUI.SetActive(true);

            if (successSound != null) successSound.Play();
            UpdateUI();
        }
        else
        {
            if (failSound != null) failSound.Play();
        }
    }


    // Bunlar artık BuyMachine metoduna listenin kaçıncı elemanı olduğunu gönderiyor
    public void BuyTea() => BuyMachine(0);      // Liste başında Tea varsa
    public void BuyCoffee() => BuyMachine(1);   // İkinci sırada Coffee varsa
    public void BuyDessert() => BuyMachine(2);  // Üçüncü sırada Dessert varsa

    private void LoadMachineStates()
    {
        foreach (var m in machineLocks)
        {
            if (m.priceText != null)
                m.priceText.text = FormatMoneyStatic(m.price);

            bool isUnlocked = IsMachineUnlocked(m.type);

            if (m.lockObject != null) m.lockObject.SetActive(!isUnlocked);
            if (m.buyButton != null) m.buyButton.SetActive(!isUnlocked);
        }
    }

    private void SyncMachineProductUIs()
    {
        foreach (var m in machineLocks)
        {
            if (m.firstProductUI != null)
            {
                m.firstProductUI.SetActive(IsMachineUnlocked(m.type));
            }
        }
    }

    // Reset kısmında kullanılacak
    private void ActivateLocks()
    {
        foreach (var m in machineLocks)
        {
            if (m.lockObject != null) m.lockObject.SetActive(true);
            if (m.buyButton != null) m.buyButton.SetActive(true);
        }
    }

    // IncomeManager.cs içinde
    public void RefreshUpgradeArrows()
    {
        // Panel objesini bul ve onun içindeki UpdateUpgradeArrow fonksiyonunu tetikle
        // (Panel kapalı olsa bile çalışması için)
        var panel = FindObjectOfType<ProductPurchasePanel>(true);
        if (panel != null)
        {
            panel.UpdateUpgradeArrow();
        }
    }


}




