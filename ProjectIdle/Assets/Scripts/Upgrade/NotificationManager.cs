using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class NotificationGroup
{
    public GameObject panel1Icon;
    public GameObject panel2Icon;

    public void SetActive(bool state)
    {
        if (panel1Icon != null) panel1Icon.SetActive(state);
        if (panel2Icon != null) panel2Icon.SetActive(state);
    }
}

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    [Header("Notification Groups")]
    public NotificationGroup teaNotifications;
    public NotificationGroup coffeeNotifications;
    public NotificationGroup dessertNotifications;

    private void Awake()
    {
        Instance = this;
    }

    // Tüm listeyi tarar ve gerçekten görülmemiş ürün varsa sadece o menünün bildirimini yakar
    public void CheckAndUpdateAllNotifications(List<ProductData> allProducts)
    {
        bool hasNewTea = false;
        bool hasNewCoffee = false;
        bool hasNewDessert = false;

        foreach (var p in allProducts)
        {
            // Ürün açık ama henüz görülmediyse (UI'da yeni ise)
            if (p.isNewlyUnlocked && !p.hasBeenSeen)
            {
                if (p.config.productType == ProductType.Tea) hasNewTea = true;
                else if (p.config.productType == ProductType.Coffee) hasNewCoffee = true;
                else if (p.config.productType == ProductType.Dessert) hasNewDessert = true;
            }
        }

        teaNotifications.SetActive(hasNewTea);
        coffeeNotifications.SetActive(hasNewCoffee);
        dessertNotifications.SetActive(hasNewDessert);
    }

    // Menüye tıklandığında o kategoriyi komple "Görüldü" olarak işaretler
    public void MarkCategoryAsSeen(ProductType type, List<ProductData> allProducts)
    {
        foreach (var p in allProducts)
        {
            if (p.config.productType == type && p.isNewlyUnlocked)
            {
                p.isNewlyUnlocked = false;
                p.hasBeenSeen = true;
            }
        }

        // Durumlar güncellendikten sonra UI'ı tekrar check et ve kapatılması gerekenleri kapat
        CheckAndUpdateAllNotifications(allProducts);
    }
}