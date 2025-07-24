using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonManager : MonoBehaviour
{
    [Header("Config Dosyaları")]
    public List<UpgradeConfig> upgradeConfig;

    [Header("UI Ayarları")]
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public int maxVisibleButtons = 6;

    private List<GameObject> allButtons = new List<GameObject>();
    private Queue<GameObject> hiddenButtons = new Queue<GameObject>();

    void Start()
    {
        Debug.Log("Upgrade Config Sayısı: " + upgradeConfig.Count); // 🕵️‍♂️  
        CreateButtonsFromConfigs();
        UpdateVisibleButtons();
    }

    void CreateButtonsFromConfigs()
    {
        foreach (var config in upgradeConfig)
        {
            string key = "Upgrade_Buyed_" + config.upgradeName;

            if (PlayerPrefs.GetInt(key, 0) == 1)
                continue;

            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
            Debug.Log($"🛠️ Buton prefab instantiate edildi: {config.upgradeName}");


            Text buttonText = newButton.GetComponentInChildren<Text>();
            if (buttonText != null)
                buttonText.text = config.upgradeName;

            Transform iconTransform = newButton.transform.Find("ItemIcon");
            if (iconTransform != null)
            {
                Image iconImage = iconTransform.GetComponent<Image>();
                if (iconImage != null)
                    iconImage.sprite = config.icon;
            }

            UpgradeConfig currentConfig = config;
            GameObject currentButton = newButton;

            Button btn = newButton.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                IncomeManager.Instance.ApplyUpgrade(currentConfig); // 🎯
                currentButton.SetActive(false);

                if (hiddenButtons.Count > 0)
                {
                    GameObject nextButton = hiddenButtons.Dequeue();
                    nextButton.SetActive(true);
                }
            });

            allButtons.Add(newButton);
        }
        UpdateVisibleButtons();
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
