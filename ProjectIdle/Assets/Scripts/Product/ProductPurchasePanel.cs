using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProductPurchasePanel : MonoBehaviour
{
    [Header("Which product does this popup control?")]
    public int productIndex; // set this when opening the popup

    [Header("UI Refs in this popup")]
    public TextMeshProUGUI titleText;        // optional: product name
    public TextMeshProUGUI currentIncomeText; // optional
    public TextMeshProUGUI nextIncomeText;    // optional (preview)
    public TextMeshProUGUI costText;          // the cost label in the popup
    public Button buyButton;                  // the main purchase button
    public TextMeshProUGUI buyButtonLabel;    // the button text (e.g., "Buy x12 - 34K")

    [Header("Buy Mode Buttons (optional highlight)")]
    public Button btnX1;
    public Button btnX10;
    public Button btnX50;
    public Button btnMax;

    [Header("Colors")]
    public Color affordableTextColor = Color.white;
    public Color unaffordableTextColor = Color.red;
    public Color affordableButtonColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    public Color unaffordableButtonColor = Color.gray;
    public Color selectedModeColor = new Color(1f, 0.85f, 0.2f, 1f);
    public Color unselectedModeColor = Color.white;

    // cache to avoid unnecessary Refresh() calls every frame
    private double _lastMoney = -1;
    private int _lastLevel = -1;
    private IncomeManager.BuyMode _lastMode;

    private ProductData P => IncomeManager.Instance.products[productIndex];

    private void OnEnable()
    {
        // When the popup opens, refresh immediately.
        Refresh();
    }

    private void Update()
    {
        // Auto-refresh when anything important changes
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

    // Hook these to the buy-mode buttons in the popup
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

        if (btnX1 != null) Tint(btnX1, mode == IncomeManager.BuyMode.x1 ? selectedModeColor : unselectedModeColor);
        if (btnX10 != null) Tint(btnX10, mode == IncomeManager.BuyMode.x10 ? selectedModeColor : unselectedModeColor);
        if (btnX50 != null) Tint(btnX50, mode == IncomeManager.BuyMode.x50 ? selectedModeColor : unselectedModeColor);
        if (btnMax != null) Tint(btnMax, mode == IncomeManager.BuyMode.Max ? selectedModeColor : unselectedModeColor);
    }

    private void Tint(Button b, Color c)
    {
        var img = b.GetComponent<Image>();
        if (img != null) img.color = c;
    }

    public void Refresh()
    {
        var im = IncomeManager.Instance;
        if (im == null || productIndex < 0 || productIndex >= im.products.Count) return;

        // Optional top texts
        if (titleText != null) titleText.text = P.config.productName;
        if (currentIncomeText != null) currentIncomeText.text = P.GetIncome().ToString("F1") + "/s";

        // Determine how many levels to buy for current mode
        int levelsToBuy = im.currentBuyMode switch
        {
            IncomeManager.BuyMode.x1 => 1,
            IncomeManager.BuyMode.x10 => 10,
            IncomeManager.BuyMode.x50 => 50,
            IncomeManager.BuyMode.Max => Mathf.Max(1, im.CalculateMaxBuyableLevels(P)), // fallback to 1 if unaffordable
            _ => 1
        };

        // Calculate total cost + affordability
        double totalCost = im.CalculateTotalCost(P, levelsToBuy);
        bool canAfford = im.totalMoney >= totalCost;

        // Cost label
        if (costText != null)
        {
            costText.text = IncomeManager.FormatMoneyStatic(totalCost);
            costText.color = canAfford ? affordableTextColor : unaffordableTextColor;
        }

        // Buy button visuals
        if (buyButton != null)
        {
            buyButton.interactable = canAfford;

            var img = buyButton.GetComponent<Image>();
            if (img != null)
                img.color = canAfford ? affordableButtonColor : unaffordableButtonColor;
        }

        // Buy button label: "Buy xN - COST"
        if (buyButtonLabel != null)
        {
            buyButtonLabel.text = $"Buy x{levelsToBuy}";
            buyButtonLabel.color = canAfford ? affordableTextColor : unaffordableTextColor;
        }

        // Optional: preview next income (after purchase)
        if (nextIncomeText != null)
        {
            // naive preview: pretend level increases by levelsToBuy, compute income, then revert
            int originalLevel = P.level;
            P.level = originalLevel + levelsToBuy;
            double newIncome = P.GetIncome();
            P.level = originalLevel;

            nextIncomeText.text = $"+{(newIncome - P.GetIncome()):F1}/s";
        }

        // cache
        _lastMoney = im.totalMoney;
        _lastLevel = P.level;
        _lastMode = im.currentBuyMode;

        // also ensure mode buttons are highlighted correctly
        HighlightBuyModeButtons();
    }
}
