using UnityEngine;

public class ProductCard : MonoBehaviour
{
    public int productIndex;
    public ProductPurchasePanel purchasePanel;
    public GameObject arrowSprite;

    public void OnClickCard()
    {
        purchasePanel.ShowForProduct(productIndex);
    }

    public void SetUpgradeArrowVisible(bool visible)
    {
        if(arrowSprite != null)
            arrowSprite.SetActive(visible);
    }
}
