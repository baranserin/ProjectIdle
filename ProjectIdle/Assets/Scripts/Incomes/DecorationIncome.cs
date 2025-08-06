using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
        public int groupId;             // Aynı yere konulabilecek objeler için grup ID
        public DecorationEntry prerequisite; // Ön koşul dekorasyon

        [HideInInspector] public bool isPurchased = false;

        public void Initialize(System.Action<DecorationEntry> onBuy)
        {
            if (targetObject != null)
                targetObject.SetActive(false); // Başta gizli

            if (buyButton != null)
            {
                // Ön koşul varsa, sadece o alındıysa açılır
                buyButton.interactable = prerequisite == null || prerequisite.isPurchased;

                buyButton.onClick.AddListener(() =>
                {
                    if (!isPurchased)
                    {
                        isPurchased = true;
                        onBuy?.Invoke(this);
                        buyButton.gameObject.SetActive(false); // Butonu gizle
                    }
                });
            }
        }

        public override string ToString()
        {
            return itemName;
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
    }

    private void ApplyDecoration(DecorationEntry selected)
    {
        if (selected.itemCost > incomeManager.totalMoney)
            return;

        incomeManager.totalMoney -= selected.itemCost;

        // Aynı gruptaki diğer dekorasyonları kapat
        foreach (var deco in decorations)
        {
            if (deco != selected && deco.groupId == selected.groupId)
            {
                if (deco.targetObject != null)
                    deco.targetObject.SetActive(false);
            }
        }

        // Seçilen dekorasyon objesini aç
        if (selected.targetObject != null)
            selected.targetObject.SetActive(true);

        // Gelir çarpanını sadece totalMoney’e uygula
        incomeManager.AddDecorationMultiplier(selected.itemMultiplier);

        // Sonraki dekorasyonların kilidini aç
        foreach (var deco in decorations)
        {
            if (deco.prerequisite == selected && deco.buyButton != null)
            {
                deco.buyButton.interactable = true;
            }
        }

        // Kaydetme işlemi (isteğe bağlı)
        PlayerPrefs.SetInt("Decoration_" + selected.itemName, 1);
        PlayerPrefs.Save();
    }

    public void ResetDecorations()
    {
        foreach (var deco in decorations)
        {
            // PlayerPrefs temizle
            PlayerPrefs.DeleteKey("Decoration_" + deco.itemName);

            // Satın alma durumu sıfırla
            deco.isPurchased = false;

            // Objeyi kapat
            if (deco.targetObject != null)
                deco.targetObject.SetActive(false);

            // Butonu aktif et
            if (deco.buyButton != null)
            {
                // Ön koşula göre etkileştir
                bool canBeActive = deco.prerequisite == null || deco.prerequisite.isPurchased;
                deco.buyButton.gameObject.SetActive(true);
                deco.buyButton.interactable = canBeActive;
            }
        }

        PlayerPrefs.Save();
    }


}
