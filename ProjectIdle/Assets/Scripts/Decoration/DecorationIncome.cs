using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DecorationIncome : MonoBehaviour
{
    
    [System.Serializable]
    public class DecorationEntry
    {
        public string itemName;
        public GameObject targetObject;   // Sahnedeki dekorasyon objesi (başta gizli)
        public Button buyButton;          // O objeye ait buton
        public int itemCost;
        public float itemMultiplier = 1f; // Gelir çarpanı

        [HideInInspector] public bool isPurchased = false;

        public void Initialize(System.Action<DecorationEntry> onBuy)
        {
           
            if (targetObject != null)
            {
                targetObject.SetActive(false); // Başta gizli
            }

            if (buyButton != null)
            {
                buyButton.onClick.AddListener(() =>
                {
                    if (!isPurchased)
                    {
                        isPurchased = true;
                        onBuy?.Invoke(this);
                        buyButton.interactable = false;
                    }
                });
            }
        }
    }

    [Header("Dekorasyonlar")]
    public List<DecorationEntry> decorations;

    [Header("Gelir Yöneticisi")]
    public IncomeManager incomeManager;

    private void Start()
    {
        foreach (var deco in decorations)
        {
            deco.Initialize(ApplyDecoration);
        }
    }

    private void ApplyDecoration(DecorationEntry entry)
    {
        if (incomeManager.totalMoney >= entry.itemCost)
        {
            incomeManager.totalMoney -= entry.itemCost;

            if (entry.targetObject != null)
            {
                entry.targetObject.SetActive(true);
            }

            incomeManager.AddDecorationMultiplier(entry.itemMultiplier);

            PlayerPrefs.SetInt("Decoration_" + entry.itemName, 1);
            PlayerPrefs.Save();
        }
    }

}
