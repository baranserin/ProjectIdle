using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ProductPurchasePanel : MonoBehaviour
{
    [Header("Which product does this popup control?")]
    public int productIndex;

    [Header("UI Refs in this popup")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI currentIncomeText;
    public TextMeshProUGUI incomeIncreaseText;
    public TextMeshProUGUI nextIncomeText;
    public TextMeshProUGUI costText;
    public Button buyButton;
    public TextMeshProUGUI buyButtonLabel;

    [Header("Buy Mode Buttons")]
    public Button btnX1;
    public Button btnX10;
    public Button btnX50;
    public Button btnMax;

    [Header("Colors")]
    public Color affordableTextColor = Color.white;
    public Color unaffordableTextColor = Color.red;
    public Color affordableButtonColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    public Color unaffordableButtonColor = Color.gray;

    // Cache to prevent unnecessary UI refreshes
    private double _lastMoney = -1;
    private int _lastLevel = -1;
    private IncomeManager.BuyMode _lastMode;

    private ProductData P => IncomeManager.Instance.products[productIndex];

    private void Start()
    {
        UpdateUpgradeArrow();
    }

    private void OnEnable()
    {
        // --- BİLDİRİM SİLME KISMI BURAYA GERİ DÖNDÜ ---
        // Panel her görünür olduğunda (butonla açıldığında) otomatik çalışır
        if (IncomeManager.Instance != null && NotificationManager.Instance != null)
        {
            if (productIndex >= 0 && productIndex < IncomeManager.Instance.products.Count)
            {
                var product = IncomeManager.Instance.products[productIndex];

                // Ürün yeni açıldıysa veya henüz popup'ı görülmediyse
                if (product.isNewlyUnlocked || !product.hasBeenSeen)
                {
                    // Artık görüldü olarak işaretle
                    product.isNewlyUnlocked = false;
                    product.hasBeenSeen = true;

                    // Sadece bu ürün görüldüğü için ana bildirim ikonlarını tekrar hesapla
                    NotificationManager.Instance.CheckAndUpdateAllNotifications(IncomeManager.Instance.products);
                }
            }
        }
        // ----------------------------------------------

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
        gameObject.SetActive(true); // Bu satır çalıştığı an otomatik olarak yukarıdaki OnEnable() tetiklenir!
        Refresh();
    }

    public void OnClickBuy()
    {
        IncomeManager.Instance.UpgradeProduct(productIndex);
        Refresh();
        UpdateUpgradeArrow();
    }

    #region Buy Mode Logic
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
        if (btnX1 != null && mode == IncomeManager.BuyMode.x1) btnX1.Select();
        if (btnX10 != null && mode == IncomeManager.BuyMode.x10) btnX10.Select();
        if (btnX50 != null && mode == IncomeManager.BuyMode.x50) btnX50.Select();
        if (btnMax != null && mode == IncomeManager.BuyMode.Max) btnMax.Select();
    }
    #endregion

    public void UpdateUpgradeArrow()
    {
        var im = IncomeManager.Instance;
        if (im == null) return;

        for (int i = 0; i < im.products.Count; i++)
        {
            var product = im.products[i];

            if (product.upgradeArrow != null)
            {
                int levelsToBuy = im.currentBuyMode switch
                {
                    IncomeManager.BuyMode.x1 => 1,
                    IncomeManager.BuyMode.x10 => 10,
                    IncomeManager.BuyMode.x50 => 50,
                    IncomeManager.BuyMode.Max => Mathf.Max(1, im.CalculateMaxBuyableLevels(product)),
                    _ => 1
                };

                double cost = im.CalculateTotalCost(product, levelsToBuy);
                bool canAfford = im.totalMoney >= cost;

                product.upgradeArrow.SetActive(canAfford);
            }
        }
    }

    public void Refresh()
    {
        var im = IncomeManager.Instance;
        if (im == null || productIndex < 0 || productIndex >= im.products.Count) return;

        if (titleText != null) titleText.text = P.config.productName;

        if (currentIncomeText != null)
            currentIncomeText.text = IncomeManager.FormatMoneyStatic(P.GetIncome()) + "/s";

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

        int originalLevel = P.level;
        P.level = originalLevel + levelsToBuy;
        double newIncome = P.GetIncome();
        P.level = originalLevel;

        double incomeDiff = newIncome - P.GetIncome();

        if (incomeIncreaseText != null)
        {
            if (incomeDiff > 0)
            {
                double visibleDiff = System.Math.Max(incomeDiff, 0.1);
                incomeIncreaseText.text = "+" + IncomeManager.FormatMoneyStatic(visibleDiff) + "/s";

                if (ColorUtility.TryParseHtmlString("#1CC717", out Color customGreen))
                    incomeIncreaseText.color = customGreen;
            }
            else
            {
                incomeIncreaseText.text = string.Empty;
            }
        }

        if (nextIncomeText != null)
            nextIncomeText.text = IncomeManager.FormatMoneyStatic(newIncome) + "/s";

        _lastMoney = im.totalMoney;
        _lastLevel = P.level;
        _lastMode = im.currentBuyMode;

        HighlightBuyModeButtons();
        UpdateUpgradeArrow();
    }
}