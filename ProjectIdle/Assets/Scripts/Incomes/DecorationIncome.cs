using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // 💠 TMP desteği

public class DecorationIncome : MonoBehaviour
{
    [System.Serializable]
    public class DecorationEntry
    {
        public string itemName;
        public GameObject targetObject; // Sahnedeki obje
        public Button buyButton;        // Satın alma butonu
        public int itemCost;
        public float itemMultiplier = 1f;

        public TextMeshProUGUI costText; // ✅ Fiyat yazısı

        public DecorationEntry prerequisite; // Ön koşul dekorasyon

        [HideInInspector] public bool isPurchased = false;

        public void Initialize(System.Action<DecorationEntry> onBuy)
        {
            if (targetObject != null)
                targetObject.SetActive(false); // Başta gizli

            if (costText != null)
                costText.text = itemCost.ToString() + " $"; // ✅ fiyat yazdır

            if (buyButton != null)
            {
                buyButton.interactable = prerequisite == null || prerequisite.isPurchased;

                buyButton.onClick.AddListener(() =>
                {
                    if (!isPurchased)
                    {
                        isPurchased = true;
                        onBuy?.Invoke(this);
                    }
                });
            }
        }
    }

    [Header("Dekorasyonlar")]
    public List<DecorationEntry> decorations;

    public IncomeManager incomeManager;

    private void Start()
    {
        foreach (var deco in decorations)
        {
            deco.Initialize(ApplyDecoration);
        }

        LoadDecorations(); // ✅ oyun açıldığında yükle
    }

    private void ApplyDecoration(DecorationEntry entry)
    {
        if (entry.itemCost > incomeManager.totalMoney)
            return;

        incomeManager.totalMoney -= entry.itemCost;

        // Aynı yerdeki diğer dekorasyonları kapat
        foreach (var deco in decorations)
        {
            if (deco != entry && deco.targetObject != null && deco.targetObject.transform.position == entry.targetObject.transform.position)
            {
                deco.targetObject.SetActive(false);
                deco.isPurchased = false;
            }
        }

        // Bu dekorasyonu aç
        if (entry.targetObject != null)
            entry.targetObject.SetActive(true);

        // Total income’a çarpanı uygula
        incomeManager.AddDecorationMultiplier(entry.itemMultiplier);

        // Ön koşulu geçenlerin butonunu aç
        foreach (var deco in decorations)
        {
            if (deco.prerequisite == entry && deco.buyButton != null)
                deco.buyButton.interactable = true;
        }

        // Kaydet
        PlayerPrefs.SetInt("Decoration_" + entry.itemName, 1);
        PlayerPrefs.Save();
    }

    public void LoadDecorations()
    {
        foreach (var deco in decorations)
        {
            int saved = PlayerPrefs.GetInt("Decoration_" + deco.itemName, 0);

            if (saved == 1)
            {
                deco.isPurchased = true;
                if (deco.targetObject != null)
                    deco.targetObject.SetActive(true);
                if (deco.buyButton != null)
                    deco.buyButton.gameObject.SetActive(false);

                incomeManager.AddDecorationMultiplier(deco.itemMultiplier);
            }
            else
            {
                deco.isPurchased = false;
                if (deco.targetObject != null)
                    deco.targetObject.SetActive(false);
                if (deco.buyButton != null)
                    deco.buyButton.gameObject.SetActive(true);
            }

            // Cost text güncelle
            if (deco.costText != null)
                deco.costText.text = deco.itemCost.ToString() + " $";
        }
    }

    public void ResetDecorations()
    {
        foreach (var deco in decorations)
        {
            PlayerPrefs.DeleteKey("Decoration_" + deco.itemName);
            deco.isPurchased = false;

            if (deco.targetObject != null)
                deco.targetObject.SetActive(false);
            if (deco.buyButton != null)
                deco.buyButton.gameObject.SetActive(true);

            if (deco.costText != null)
                deco.costText.text = deco.itemCost.ToString() + " $"; // reset sonrası fiyat geri yaz
        }
        PlayerPrefs.Save();
    }
}
