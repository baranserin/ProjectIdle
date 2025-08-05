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

        public List<GameObject> affectedProducts; // Etkilenecek ürün objeleri

        public DecorationEntry prerequisite; // Ön koşul dekorasyon (bu alınmadan açılamaz)

        [HideInInspector] public bool isPurchased = false;

        public void Initialize(System.Action<DecorationEntry> onBuy)
        {
            if (targetObject != null)
                targetObject.SetActive(false); // Başta gizli

            if (buyButton != null)
            {
                // Ön koşul varsa ve alınmamışsa buton başta pasif olsun
                buyButton.interactable = prerequisite == null || prerequisite.isPurchased;

                buyButton.onClick.AddListener(() =>
                {
                    if (!isPurchased)
                    {
                        isPurchased = true;
                        onBuy?.Invoke(this);
                        buyButton.interactable = false;
                    }
                });
            }
        }
    }

    [Header("Dekorasyonlar")]
    public List<DecorationEntry> decorations;

    [Header("Toplam Para")]
    public float totalMoney = 1000f;

    private void Start()
    {
        foreach (var deco in decorations)
        {
            deco.Initialize(ApplyDecoration);
        }
    }

    private void ApplyDecoration(DecorationEntry entry)
    {
        if (entry.itemCost > totalMoney)
            return;

        totalMoney -= entry.itemCost;

        // Tüm dekorasyon objelerini kapat (öncekileri)
        foreach (var deco in decorations)
        {
            if (deco != entry && deco.targetObject != null)
                deco.targetObject.SetActive(false);
        }

        // Bu dekorasyon objesini aç
        if (entry.targetObject != null)
            entry.targetObject.SetActive(true);

        // Etkilediği objelere çarpanı uygula
        foreach (var obj in entry.affectedProducts)
        {
            var product = obj.GetComponent<ProductData>();
            if (product != null)
            {
                product.incomeMultiplier *= entry.itemMultiplier;
                product.UpdateUI(); // Eğer varsa UI güncelle
            }
        }

        // Bu dekorasyonu şart koşan diğerlerinin butonunu aç
        foreach (var deco in decorations)
        {
            if (deco.prerequisite == entry && deco.buyButton != null)
            {
                deco.buyButton.interactable = true;
            }
        }

        // Kaydet (isteğe bağlı)
        PlayerPrefs.SetInt("Decoration_" + entry.itemName, 1);
        PlayerPrefs.Save();
    }
}
