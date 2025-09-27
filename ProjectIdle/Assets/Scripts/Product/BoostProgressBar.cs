using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoostProgressBar : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private ProductConfig config; // Inspector�da se�ilecek

    [Header("UI Elements")]
    [SerializeField] private Image fillBar;                // Elle ba�lanacak
    [SerializeField] private TextMeshProUGUI progressText; // Opsiyonel, elle ba�lanacak

    private ProductData product;

    void Awake()
    {
        // IncomeManager �zerinden ger�ek ProductData�y� bul
        product = IncomeManager.Instance.products.Find(p => p.config == config);
        if (product == null)
            Debug.LogError($"BoostProgressBar: {config.productName} i�in ProductData bulunamad�!");
    }

    void Update()
    {
        if (product != null)
            UpdateBoostBar(product.level);
    }

    private void UpdateBoostBar(int currentLevel)
    {
        if (fillBar == null) return;

        int prevBoost = 1;
        int nextBoost = -1;

        foreach (var boost in config.levelBoosts)
        {
            if (boost == null || boost.requiredLevel <= 0) continue;

            if (boost.requiredLevel <= currentLevel)
                prevBoost = boost.requiredLevel;
            else
            {
                nextBoost = boost.requiredLevel;
                break;
            }
        }

        if (nextBoost == -1)
        {
            fillBar.fillAmount = 1f;
            if (progressText != null)
                progressText.text = "Max";
        }
        else
        {
            float progress = (float)(currentLevel - prevBoost) / (nextBoost - prevBoost);
            fillBar.fillAmount = Mathf.Clamp01(progress);

            if (progressText != null)
                progressText.text = $"{currentLevel - prevBoost}/{nextBoost - prevBoost}";
        }
    }
}
