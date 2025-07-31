using System.Collections.Generic;
using UnityEngine;

public class DecorationIncome: MonoBehaviour
{
    [Header("Dekorasyon Ayarları")]
    public List<DecorationConfig> decorationConfigs;

    [Header("UI Prefabları")]
    public GameObject buttonPrefab;
    public Transform buttonParent;

    void Start()
    {
        CreateButtons();
        ApplySavedDecorations();
    }

    void CreateButtons()
    {
        foreach (var config in decorationConfigs)
        {
            if (PlayerPrefs.GetInt("Decoration_Buyed_" + config.itemName, 0) == 1)
                continue;

            GameObject button = Instantiate(buttonPrefab, buttonParent);
            button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"{config.itemName} (${config.upgradeCost})";
            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => BuyDecoration(config, button));
        }
    }

    void BuyDecoration(DecorationConfig config, GameObject buttonObj)
    {
        if (IncomeManager.Instance.totalMoney >= config.upgradeCost)
        {
            IncomeManager.Instance.totalMoney -= config.upgradeCost;

            ApplyDecoration(config);
            PlayerPrefs.SetInt("Decoration_Buyed_" + config.itemName, 1);
            PlayerPrefs.Save();

            Destroy(buttonObj); // Buton gizlenir
            IncomeManager.Instance.UpdateUI();

            Debug.Log($"🎉 {config.itemName} dekorasyonu satın alındı.");
        }
        else
        {
            Debug.Log("❌ Yeterli paran yok.");
        }
    }

    void ApplyDecoration(DecorationConfig config)
    {
        // Gelir etkisi uygula
        foreach (var product in IncomeManager.Instance.products)
        {
            product.incomeMultiplier *= config.itemMultiplier;
            product.UpdateUI();
        }

        // Dekorasyon sahneye yerleştir
        if (config.decorationPrefab != null && config.spawnPoint != null)
        {
            Instantiate(config.decorationPrefab, config.spawnPoint.position, config.spawnPoint.rotation);
        }
    }

    void ApplySavedDecorations()
    {
        foreach (var config in decorationConfigs)
        {
            if (PlayerPrefs.GetInt("Decoration_Buyed_" + config.itemName, 0) == 1)
            {
                ApplyDecoration(config); // hem gelir hem görünüm
            }
        }
    }
}
