using UnityEngine;

public class ProductCard : MonoBehaviour
{
    public int productIndex;
    public ProductPurchasePanel purchasePanel; // assign in Inspector

    public void OnClickCard()
    {
        purchasePanel.ShowForProduct(productIndex);
    }
}
