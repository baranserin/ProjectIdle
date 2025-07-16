using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ProductHoverInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject infoPanel; // A UI panel that displays the info
    public TextMeshProUGUI infoText; // Text to show product income
    public ProductData productData; // Reference to the product

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (infoPanel != null && productData != null)
        {
            infoPanel.SetActive(true);
            infoText.text = $"Income: {productData.GetIncome():F1}/s";
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }
}
