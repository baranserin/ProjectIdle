using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq; // Listeyi sıralamak için gerekli

public class BoostProgressBar : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private ProductConfig config;

    [Header("UI Elements")]
    [SerializeField] private Image fillBar;
    [SerializeField] private TextMeshProUGUI progressText;

    private ProductData product;
    private int[] sortedBoostLevels; // Seviyeleri küçükten büyüğe tutacak dizi

    void Start()
    {
        // Merkezi tablodaki seviyeleri küçükten büyüğe sırala ve diziye al
        sortedBoostLevels = GlobalLevelBoosts.BoostTable.Keys.OrderBy(x => x).ToArray();

        // IncomeManager üzerinden gerçek ProductData’yı bul
        if (IncomeManager.Instance != null)
        {
            product = IncomeManager.Instance.products.Find(p => p.config == config);
        }

        if (product == null)
            Debug.LogError($"BoostProgressBar: {config.productName} için ProductData bulunamadı!");
    }

    void Update()
    {
        if (product != null)
            UpdateBoostBar(product.level);
    }

    private void UpdateBoostBar(int currentLevel)
    {
        if (fillBar == null || sortedBoostLevels == null || sortedBoostLevels.Length == 0) return;

        int prevBoost = 0;
        int nextBoost = -1;

        // Mevcut seviyeye göre hangi aralıkta olduğumuzu bul
        for (int i = 0; i < sortedBoostLevels.Length; i++)
        {
            if (sortedBoostLevels[i] > currentLevel)
            {
                nextBoost = sortedBoostLevels[i];
                if (i > 0) prevBoost = sortedBoostLevels[i - 1];
                break;
            }
        }

        // Eğer son boostu da geçtiysek barı fulle
        if (nextBoost == -1)
        {
            fillBar.fillAmount = 1f;
            if (progressText != null)
                progressText.text = "MAX BOOST";
        }
        else
        {
            // Mevcut aralıktaki ilerlemeyi hesapla
            float progress = (float)(currentLevel - prevBoost) / (nextBoost - prevBoost);
            fillBar.fillAmount = Mathf.Clamp01(progress);

            if (progressText != null)
                progressText.text = $"{currentLevel}/{nextBoost}";
        }
    }
}