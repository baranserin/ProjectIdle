using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static NUnit.Framework.Internal.OSPlatform;

[System.Serializable]
public class UpgradeCardData
{
    public Sprite objectImage;
    public string objectName;
    public int price;

    public ProductType targetType;
    public float multiplier;

    [HideInInspector] public bool isBought = false;
}
public class UpgradeCardManager : MonoBehaviour
{
    [Header("Card Data")]
    public UpgradeCardData[] upgrades;

    [Header("UI References")]
    public Image objectImageUI;
    public TMP_Text objectNameUI;
    public TMP_Text priceTextUI;
    public Image[] pageIndicators;

    [Header("Page Indicator Sprites")]
    public Sprite dotOn;
    public Sprite dotOff;


    private int currentIndex = 0;

    void Start()
    {
        ShowCard(0);
    }

    public void ShowCard(int index)
    {
        if (index < 0 || index >= upgrades.Length) return;

        currentIndex = index;

        objectImageUI.sprite = upgrades[index].objectImage;
        objectNameUI.text = upgrades[index].objectName;
        priceTextUI.text = upgrades[index].price.ToString();

        UpdatePageIndicators();
        UpdateCardVisual(upgrades[index]);
    }

    public void NextCard()
    {
        int nextIndex = (currentIndex + 1) % upgrades.Length;
        ShowCard(nextIndex);
    }

    public void PreviousCard()
    {
        int prevIndex = (currentIndex - 1 + upgrades.Length) % upgrades.Length;
        ShowCard(prevIndex);
    }

    void UpdatePageIndicators()
    {
        for (int i = 0; i < pageIndicators.Length; i++)
        {
            pageIndicators[i].sprite = (i == currentIndex) ? dotOn : dotOff;
        }
    }
    public void BuyUpgrade()
    {
        var upgrade = upgrades[currentIndex];

        if (upgrade.isBought)
        {
            Debug.Log("Upgrade already bought!");
            return;
        }

        if (IncomeManager.Instance.totalMoney >= upgrade.price)
        {
            IncomeManager.Instance.totalMoney -= upgrade.price;
            IncomeManager.Instance.ApplyCategoryUpgrade(upgrade.targetType, upgrade.multiplier);

            upgrade.isBought = true;

            Debug.Log($"✅ Bought upgrade: {upgrade.objectName}, applied x{upgrade.multiplier} to {upgrade.targetType}");
            IncomeManager.Instance.UpdateUI();

            UpdateCardVisual(upgrade);
        }
        else
        {
            Debug.Log("❌ Not enough money for upgrade!");
        }
    }

    private void UpdateCardVisual(UpgradeCardData upgrade)
    {
        if (upgrade.isBought)
        {
            priceTextUI.text = "BOUGHT"; // change text
            priceTextUI.color = Color.green;

            // Optionally fade out the image
            //objectImageUI.color = new Color(1f, 1f, 1f, 0.5f);
        }
        else
        {
            priceTextUI.text = upgrade.price.ToString();
            priceTextUI.color = Color.white;
            objectImageUI.color = Color.white;
        }
    }


}
