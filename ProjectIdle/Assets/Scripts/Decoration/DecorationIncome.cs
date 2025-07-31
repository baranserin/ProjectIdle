using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DecorationIncome : MonoBehaviour
{
    [System.Serializable]
    public class DecorationEntry
    {
        public DecorationConfig config;
        public Button buyButton;
        private bool isPurchased = false;

        public void Initialize(System.Action<DecorationConfig> onBuy)
        {
            if (buyButton != null && config != null)
            {
                buyButton.onClick.AddListener(() =>
                {
                    if (!isPurchased)
                    {
                        isPurchased = true;
                        onBuy?.Invoke(config);
                        buyButton.interactable = false;
                    }
                });
            }
        }
    }

    [Header("Tüm Dekorasyonlar")]
    public List<DecorationEntry> decorations;

    [Header("Gelir Yöneticisi")]
    public IncomeManager incomeManager; // Varsa, yoksa çıkarılabilir

    private void Start()
    {
        foreach (var deco in decorations)
        {
            deco.Initialize(ApplyDecoration);
        }
    }

    private void ApplyDecoration(DecorationConfig config)
    {
        // 1. Görsel olarak dekorasyonu sahneye (UI) yerleştir
        if (config.decorationPrefab != null && config.spawnPoint != null)
        {
            GameObject instance = Instantiate(config.decorationPrefab, config.spawnPoint.position, Quaternion.identity, config.spawnPoint);
            instance.transform.localScale = Vector3.one; // UI ölçek düzeltmesi
        }

        // 2. Geliri artır (IncomeManager varsa)
        /*if (incomeManager != null)
        {
            incomeManager.AddDecorationMultiplier(config.itemMultiplier);
        }
        */
    }
}
