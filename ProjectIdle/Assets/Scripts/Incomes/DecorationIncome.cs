using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecorationIncome : MonoBehaviour
{
    [System.Serializable]
    public class DecorationEntry
    {
        public string itemName;
        public GameObject targetObject;   // Sahnedeki obje
        public Button buyButton;          // Satın alma / Seç butonu
        public int itemCost;
        public float itemMultiplier = 1f;

        public int groupId;
        public TextMeshProUGUI costText;
        public DecorationEntry prerequisite;

        [Header("Satın Alındı Prefab")]
        public GameObject purchasedPrefab;      // Inspector’dan atanacak
        [HideInInspector] public GameObject spawnedPurchased; // Oluşturulan prefab referansı

        [HideInInspector] public bool isPurchased = false; // Satın alındı mı?
        [HideInInspector] public bool isSelected = false;  // Seçili mi?

        public void Initialize(System.Action<DecorationEntry> onBuy)
        {
            if (targetObject != null)
                targetObject.SetActive(false);

            if (costText != null)
                costText.text = itemCost.ToString() + " $";

            if (buyButton != null)
            {
                buyButton.onClick.AddListener(() =>
                {
                    onBuy?.Invoke(this);
                });
            }
        }
    }

    [Header("Dekorasyonlar")]
    public List<DecorationEntry> decorations;

    public IncomeManager incomeManager;

    public TextMeshProUGUI popupText; // Inspector'dan bağla
    public float displayDuration = 3f; // Kaç saniye gözükecek

    private void Start()
    {
        foreach (var deco in decorations)
        {
            deco.Initialize(ApplyDecoration);
        }

        LoadDecorations();
    }

    private void ApplyDecoration(DecorationEntry entry)
    {
        // Eğer satın alınmamışsa para kontrolü
        if (!entry.isPurchased)
        {
            if (entry.itemCost > incomeManager.totalMoney)
                return;

            incomeManager.totalMoney -= entry.itemCost;
            entry.isPurchased = true;
            incomeManager.AddDecorationMultiplier(entry.itemMultiplier);
            PlayerPrefs.SetInt("Decoration_" + entry.itemName, 1);
            PlayerPrefs.Save();

            // Prefab oluştur
            if (entry.purchasedPrefab != null && entry.spawnedPurchased == null && entry.buyButton != null)
            {
                Transform parent = entry.buyButton.transform.parent;
                entry.spawnedPurchased = Instantiate(entry.purchasedPrefab, parent, false);
                entry.spawnedPurchased.transform.SetSiblingIndex(entry.buyButton.transform.GetSiblingIndex());
                entry.spawnedPurchased.transform.localScale = Vector3.one;
            }
        }

        // Aynı gruptaki diğer dekorasyonları deselect et
        foreach (var deco in decorations)
        {
            if (deco != entry && deco.groupId == entry.groupId)
            {
                deco.isSelected = false;
                if (deco.targetObject != null)
                    deco.targetObject.SetActive(false);
            }
        }

        // Bu dekorasyonu seçili yap
        entry.isSelected = true;
        if (entry.targetObject != null)
            entry.targetObject.SetActive(true);

        // Buton metinlerini ve interactivity'yi güncelle
        UpdateDecorationButtons();
    }

    private void UpdateDecorationButtons()
    {
        foreach (var deco in decorations)
        {
            if (!deco.isPurchased)
            {
                if (deco.buyButton != null)
                {
                    deco.buyButton.gameObject.SetActive(true);
                    deco.buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "BUY";
                    deco.buyButton.interactable = deco.prerequisite == null || deco.prerequisite.isPurchased;
                }
            }
            else
            {
                if (deco.buyButton != null)
                {
                    deco.buyButton.gameObject.SetActive(true);

                    if (deco.isSelected)
                    {
                        deco.buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "SELECTED";
                        deco.buyButton.interactable = false;
                    }
                    else
                    {
                        deco.buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "SELECT";
                        deco.buyButton.interactable = true;
                    }
                }
            }

            // Fiyat text
            if (deco.costText != null)
                deco.costText.text = deco.itemCost.ToString() + " $";
        }
    }

    public void LoadDecorations()
    {
        foreach (var deco in decorations)
        {
            int saved = PlayerPrefs.GetInt("Decoration_" + deco.itemName, 0);

            if (saved == 1)
            {
                deco.isPurchased = true;
            }

            deco.isSelected = false;
        }

        UpdateDecorationButtons();
    }

    public void ResetDecorations()
    {
        foreach (var deco in decorations)
        {
            PlayerPrefs.DeleteKey("Decoration_" + deco.itemName);
            deco.isPurchased = false;
            deco.isSelected = false;

            if (deco.targetObject != null)
                deco.targetObject.SetActive(false);
            if (deco.buyButton != null)
                deco.buyButton.gameObject.SetActive(true);

            if (deco.spawnedPurchased != null)
            {
                Destroy(deco.spawnedPurchased);
                deco.spawnedPurchased = null;
            }

            if (deco.costText != null)
                deco.costText.text = deco.itemCost.ToString() + " $";
        }
        PlayerPrefs.Save();
    }
}
