using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ProductPurchasePanel : MonoBehaviour
{
    [Header("Which product does this popup control?")]
    public int productIndex; // set this when opening the popup

    [Header("UI Refs in this popup")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI currentIncomeText;
    public TextMeshProUGUI incomeIncreaseText;
    public TextMeshProUGUI nextIncomeText;
    public TextMeshProUGUI costText;
    public Button buyButton;
    public TextMeshProUGUI buyButtonLabel;

    [Header("Buy Mode Buttons (Sprite Swap üzerinden görünecek)")]
    public Button btnX1;
    public Button btnX10;
    public Button btnX50;
    public Button btnMax;

    [Header("Colors")]
    public Color affordableTextColor = Color.white;
    public Color unaffordableTextColor = Color.red;
    public Color affordableButtonColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    public Color unaffordableButtonColor = Color.gray;

    // cache
    private double _lastMoney = -1;
    private int _lastLevel = -1;
    private IncomeManager.BuyMode _lastMode;

    private ProductData P => IncomeManager.Instance.products[productIndex];

    private void OnEnable()
    {
        Refresh();
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        var im = IncomeManager.Instance;
        if (im == null || productIndex < 0 || productIndex >= im.products.Count) return;

        if (_lastMoney != im.totalMoney || _lastLevel != P.level || _lastMode != im.currentBuyMode)
        {
            Refresh();
        }
    }

    public void ShowForProduct(int index)
    {
        productIndex = index;
        gameObject.SetActive(true);
        Refresh();
    }

    public void OnClickBuy()
    {
        IncomeManager.Instance.UpgradeProduct(productIndex);
        Refresh();
    }

    // Hook these to the buy-mode buttons
    public void OnClickModeX1() { SetMode(IncomeManager.BuyMode.x1); }
    public void OnClickModeX10() { SetMode(IncomeManager.BuyMode.x10); }
    public void OnClickModeX50() { SetMode(IncomeManager.BuyMode.x50); }
    public void OnClickModeMax() { SetMode(IncomeManager.BuyMode.Max); }

    private void SetMode(IncomeManager.BuyMode mode)
    {
        IncomeManager.Instance.currentBuyMode = mode;
        HighlightBuyModeButtons();
        Refresh();
    }

    private void HighlightBuyModeButtons()
    {
        var mode = IncomeManager.Instance.currentBuyMode;

        // Hangi mod seçildiyse onu "Selected" state'e geçiriyoruz
        if (btnX1 != null && mode == IncomeManager.BuyMode.x1) btnX1.Select();
        if (btnX10 != null && mode == IncomeManager.BuyMode.x10) btnX10.Select();
        if (btnX50 != null && mode == IncomeManager.BuyMode.x50) btnX50.Select();
        if (btnMax != null && mode == IncomeManager.BuyMode.Max) btnMax.Select();
    }

    public void Refresh()
    {
        var im = IncomeManager.Instance;
        if (im == null || productIndex < 0 || productIndex >= im.products.Count) return;

        if (titleText != null) titleText.text = P.config.productName;
        if (currentIncomeText != null) currentIncomeText.text = P.GetIncome().ToString("F1") + "/s";

        int levelsToBuy = im.currentBuyMode switch
        {
            IncomeManager.BuyMode.x1 => 1,
            IncomeManager.BuyMode.x10 => 10,
            IncomeManager.BuyMode.x50 => 50,
            IncomeManager.BuyMode.Max => Mathf.Max(1, im.CalculateMaxBuyableLevels(P)),
            _ => 1
        };

        double totalCost = im.CalculateTotalCost(P, levelsToBuy);
        bool canAfford = im.totalMoney >= totalCost;

        if (costText != null)
        {
            costText.text = IncomeManager.FormatMoneyStatic(totalCost);
            costText.color = canAfford ? affordableTextColor : unaffordableTextColor;
        }

        if (buyButton != null)
        {
            buyButton.interactable = canAfford;
            var img = buyButton.GetComponent<Image>();
            if (img != null)
                img.color = canAfford ? affordableButtonColor : unaffordableButtonColor;
        }

        if (buyButtonLabel != null)
        {
            buyButtonLabel.text = $"Buy x{levelsToBuy}";
            buyButtonLabel.color = canAfford ? affordableTextColor : unaffordableTextColor;
        }

        // Calculate income difference
        int originalLevel = P.level;
        P.level = originalLevel + levelsToBuy;
        double newIncome = P.GetIncome();
        P.level = originalLevel;

        double incomeDiff = newIncome - P.GetIncome();

        // Show current income
        if (currentIncomeText != null)
            currentIncomeText.text = $"{P.GetIncome():F1}/s";

        // Show next income difference (like +5/s)
        if (incomeIncreaseText != null)
        {
            if (incomeDiff > 0)
            {
                incomeIncreaseText.text = $"+{incomeDiff:F1}/s";
            Color customGreen;
            if (ColorUtility.TryParseHtmlString("#1CC717", out customGreen))
            {
                incomeIncreaseText.color = customGreen;
            }
            }
            else
            {
                incomeIncreaseText.text = string.Empty;
            }
        }

        // Optionally, show next total income if you like:
        if (nextIncomeText != null)
            nextIncomeText.text = $"{newIncome:F1}/s";


        _lastMoney = im.totalMoney;
        _lastLevel = P.level;
        _lastMode = im.currentBuyMode;

        HighlightBuyModeButtons();
    }
}
