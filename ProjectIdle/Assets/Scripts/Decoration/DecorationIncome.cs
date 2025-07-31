using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DecorationIncome    : MonoBehaviour
{
    [System.Serializable]
    public class DecorationEntry
    {
        public DecorationConfig config;
        public Button buyButton;
        public TextMeshProUGUI priceText;
    }

    [Header("Dekorasyonlar")]
    public List<DecorationEntry> decorations;

    void Start()
    {
        foreach (var deco in decorations)
        {
            string key = "Decoration_Buyed_" + deco.config.itemName;

            if (PlayerPrefs.GetInt(key, 0) == 1)
            {
                // Satın alınmışsa butonu gizle ve dekorasyonu sahneye koy
                if (deco.buyButton != null)
                    deco.buyButton.gameObject.SetActive(false);

                SpawnDecoration(deco.config);
                ApplyMultiplier(deco.config);
            }
            else
            {
                // Butonu aktif bırak, işlevini ata
                if (deco.priceText != null)
                    deco.priceText.text = $"{deco.config.itemName} (${deco.config.upgradeCost})";

                if (deco.buyButton != null)
                {
                    deco.buyButton.onClick.RemoveAllListeners(); // Çakışma önlemi
                    deco.buyButton.onClick.AddListener(() => BuyDecoration(deco));
                }
            }
        }
    }

    void BuyDecoration(DecorationEntry entry)
    {
        var config = entry.config;

        if (IncomeManager.Instance.totalMoney >= config.upgradeCost)
        {
            IncomeManager.Instance.totalMoney -= config.upgradeCost;
            IncomeManager.Instance.UpdateUI();

            PlayerPrefs.SetInt("Decoration_Buyed_" + config.itemName, 1);
            PlayerPrefs.Save();

            SpawnDecoration(config);
            ApplyMultiplier(config);

            if (entry.buyButton != null)
                entry.buyButton.gameObject.SetActive(false);

            Debug.Log($"✅ {config.itemName} satın alındı!");
        }
        else
        {
            Debug.Log("❌ Yeterli paran yok.");
        }
    }

    void ApplyMultiplier(DecorationConfig config)
    {
        foreach (var product in IncomeManager.Instance.products)
        {
            product.incomeMultiplier *= config.itemMultiplier;
            product.UpdateUI();
        }
    }

    void SpawnDecoration(DecorationConfig config)
    {
        if (config.decorationPrefab != null && config.spawnPoint != null)
        {
            Instantiate(config.decorationPrefab, config.spawnPoint.position, config.spawnPoint.rotation);
        }
    }
}
