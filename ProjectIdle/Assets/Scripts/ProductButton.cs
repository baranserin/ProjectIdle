using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProductButtonManager : MonoBehaviour
{
    [Header("Config Dosyalar�")]
    public List<ProductConfig> productConfigs;      // ProductConfig listesi

    [Header("UI Ayarlar�")]
    public GameObject buttonPrefab;                 // UI prefab (buton)
    public Transform buttonContainer;               // HorizontalLayoutGroup i�eren obje
    public int maxVisibleButtons = 5;               // Ayn� anda g�sterilecek maksimum buton say�s�

    private List<GameObject> allButtons = new List<GameObject>();
    private Queue<GameObject> hiddenButtons = new Queue<GameObject>();

    void Start()
    {
        CreateButtonsFromConfigs();
        UpdateVisibleButtons();
    }

    void CreateButtonsFromConfigs()
    {
        foreach (var config in productConfigs)
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);

            // 1. Butonun metnini ayarla
            Text buttonText = newButton.GetComponentInChildren<Text>();
            if (buttonText != null)
                buttonText.text = config.productName;

            // 2. Sprite (ikon) ayarla - prefab i�inde "Icon" ad�nda bir Image objesi olmal�
            Transform iconTransform = newButton.transform.Find("ItemIcon");
            if (iconTransform != null)
            {
                Image iconImage = iconTransform.GetComponent<Image>();
                iconImage.sprite = config.icon;
            }

            // 3. T�klama fonksiyonu ba�la
            Button btn = newButton.GetComponent<Button>();
            btn.onClick.AddListener(() => OnProductButtonClicked(newButton, config));

            // 4. Listeye ekle
            allButtons.Add(newButton);
        }
    }


    void OnProductButtonClicked(GameObject clickedButton, ProductConfig config)
    {
        clickedButton.SetActive(false);

        if (hiddenButtons.Count > 0)
        {
            GameObject nextButton = hiddenButtons.Dequeue();
            nextButton.SetActive(true);
        }
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

    void OnProductButtonClicked(GameObject clickedButton)
    {
        clickedButton.SetActive(false);

        // S�radaki butonu g�ster
        if (hiddenButtons.Count > 0)
        {
            GameObject nextButton = hiddenButtons.Dequeue();
            nextButton.SetActive(true);
        }
    }
}
