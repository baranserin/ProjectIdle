using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonManager : MonoBehaviour
{
    [Header("Config Dosyalar�")]
    public List<UpgradeConfig> upgradeConfig;

    [Header("UI Ayarlar�")]
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public int maxVisibleButtons = 5;

    private List<GameObject> allButtons = new List<GameObject>();
    private Queue<GameObject> hiddenButtons = new Queue<GameObject>();

    void Start()
    {
        CreateButtonsFromConfigs();
        UpdateVisibleButtons();
    }

    void CreateButtonsFromConfigs()
    {
        foreach (var config in upgradeConfig)
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);

            // Butonun metnini ayarla
            Text buttonText = newButton.GetComponentInChildren<Text>();
            if (buttonText != null)
                buttonText.text = config.upgradeName;

            // �konu ayarla
            Transform iconTransform = newButton.transform.Find("ItemIcon");
            if (iconTransform != null)
            {
                Image iconImage = iconTransform.GetComponent<Image>();
                if (iconImage != null)
                    iconImage.sprite = config.icon;
            }

            // Buton t�klama fonksiyonu
            Button btn = newButton.GetComponent<Button>();
            btn.onClick.AddListener(() => OnUpgradeButtonClicked(newButton, config));

            allButtons.Add(newButton);
        }
    }

    void OnUpgradeButtonClicked(GameObject clickedButton, UpgradeConfig config)
    {
        clickedButton.SetActive(false);

        // Yeni butonu s�radan ��kar ve g�ster
        if (hiddenButtons.Count > 0)
        {
            GameObject nextButton = hiddenButtons.Dequeue();
            nextButton.SetActive(true);
        }

        // Burada upgrade efekti uygulanabilir
        Debug.Log($"Upgrade uyguland�: {config.upgradeName}");
    }

    void UpdateVisibleButtons()
    {
        int visibleCount = 0;
        hiddenButtons.Clear();

        foreach (var button in allButtons)
        {
            if (visibleCount < maxVisibleButtons)
            {
                button.SetActive(true);
                visibleCount++;
            }
            else
            {
                button.SetActive(false);
                hiddenButtons.Enqueue(button);
            }
        }
    }
}
