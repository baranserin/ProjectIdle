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
        public GameObject targetObject;   // Sahnedeki obje
        public Button buyButton;          // Satın alma butonu
        public int itemCost;
        public float itemMultiplier = 1f;

        public int groupId;               // ✅ Grup kimliği (aynı grup = aynı yerdeki alternatifler)
        public TextMeshProUGUI costText;  // ✅ Fiyat yazısı

        public DecorationEntry prerequisite; // Ön koşul dekorasyon
        [HideInInspector] public bool isPurchased = false;

        [Header("Satın Alındı Prefab")]
        public GameObject purchasedPrefab;      // ✅ Inspector’dan atanacak
        [HideInInspector] public GameObject spawnedPurchased; // Oluşturulan prefab referansı

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

        // ✅ Aynı gruptaki diğer dekorasyonları kapat
        foreach (var deco in decorations)
        {
            if (deco != entry && deco.groupId == entry.groupId)
            {
                if (deco.targetObject != null)
                    deco.targetObject.SetActive(false);

                deco.isPurchased = false;

                if (deco.buyButton != null)
                    deco.buyButton.gameObject.SetActive(true);
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

        // Buton gizle + Prefab oluştur
        if (entry.buyButton != null)
        {
            RectTransform btnRect = entry.buyButton.GetComponent<RectTransform>();
            Transform parent = entry.buyButton.transform.parent;

            entry.buyButton.gameObject.SetActive(false);

            if (entry.purchasedPrefab != null)
            {
                entry.spawnedPurchased = Instantiate(entry.purchasedPrefab, parent, false);

                RectTransform prefabRect = entry.spawnedPurchased.GetComponent<RectTransform>();
                if (prefabRect != null && btnRect != null)
                {
                    prefabRect.anchoredPosition = btnRect.anchoredPosition;
                    prefabRect.sizeDelta = btnRect.sizeDelta;
                    prefabRect.localScale = btnRect.localScale;
                }
            }
            else
            {
                Debug.LogWarning($"Purchased Prefab atanmadı: {entry.itemName}");
            }
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

                // Prefab yükle
                if (deco.purchasedPrefab != null && deco.spawnedPurchased == null)
                {
                    RectTransform btnRect = deco.buyButton != null ? deco.buyButton.GetComponent<RectTransform>() : null;
                    Transform parent = deco.buyButton != null ? deco.buyButton.transform.parent : null;

                    deco.spawnedPurchased = Instantiate(deco.purchasedPrefab, parent, false);

                    if (btnRect != null)
                    {
                        RectTransform prefabRect = deco.spawnedPurchased.GetComponent<RectTransform>();
                        prefabRect.anchoredPosition = btnRect.anchoredPosition;
                        prefabRect.sizeDelta = btnRect.sizeDelta;
                        prefabRect.localScale = btnRect.localScale;
                    }
                }

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

            if (deco.spawnedPurchased != null)
                Destroy(deco.spawnedPurchased);

            if (deco.costText != null)
                deco.costText.text = deco.itemCost.ToString() + " $"; // reset sonrası fiyat geri yaz
        }
        PlayerPrefs.Save();
    }
}
