using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class UpgradeCardData
{
    public Sprite objectImage;
    public string objectName;
    public int price;
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
        priceTextUI.text = upgrades[index].price.ToString() + "💰";

        UpdatePageIndicators();
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

}
