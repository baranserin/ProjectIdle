using UnityEngine;

public class ProductCard : MonoBehaviour
{
    public int productIndex;
    public ProductPurchasePanel purchasePanel;
    public GameObject arrowSprite;

    [Header("Upgrade Hint Icon")]
    public GameObject upgradeHintIcon; // 👈 Üstte çıkacak ok / ışık / item görseli

    public void OnClickCard()
    {
        purchasePanel.ShowForProduct(productIndex);
    }

    public void SetUpgradeArrowVisible(bool visible)
    {
        if(arrowSprite != null)
            arrowSprite.SetActive(visible);
    }

    public void UpdateUpgradeArrow()
    {
        if (IncomeManager.Instance == null || upgradeHintIcon == null)
            return;

        var manager = IncomeManager.Instance;

        // Güvenli olsun diye index kontrolü
        if (productIndex < 0 || productIndex >= manager.products.Count)
        {
            upgradeHintIcon.SetActive(false);
            return;
        }

        var p = manager.products[productIndex];

        // Ürün UI'ı kapalıysa (makine kilidi veya unlock şartı yüzünden) ikon da görünmesin
        if (p.uiObject == null || !p.uiObject.activeInHierarchy)
        {
            upgradeHintIcon.SetActive(false);
            return;
        }

        // Geçerli BuyMode'a göre kaç level alınacak?
        int levelsToBuy = manager.currentBuyMode switch
        {
            IncomeManager.BuyMode.x1 => 1,
            IncomeManager.BuyMode.x10 => 10,
            IncomeManager.BuyMode.x50 => 50,
            IncomeManager.BuyMode.Max => Mathf.Max(1, manager.CalculateMaxBuyableLevels(p)),
            _ => 1
        };

        if (levelsToBuy <= 0)
        {
            upgradeHintIcon.SetActive(false);
            return;
        }

        // Bu kadar level almak kaç para?
        double totalCost = manager.CalculateTotalCost(p, levelsToBuy);

        // Şu anki parayla alınabiliyor mu?
        bool canUpgrade = manager.totalMoney >= totalCost;

        // İkonu aç/kapa
        upgradeHintIcon.SetActive(canUpgrade);
    }

    private void Update()
    {
        UpdateUpgradeArrow();
    }
}
