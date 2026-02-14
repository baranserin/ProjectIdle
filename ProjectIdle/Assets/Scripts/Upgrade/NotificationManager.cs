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

    // Bildirimleri sadece açıp kapatan temel fonksiyon
    public void SetNotificationUI(ProductType type, bool state)
    {
        switch (type)
        {
            case ProductType.Tea:
                teaNotifications.SetActive(state);
                break;
            case ProductType.Coffee:
                coffeeNotifications.SetActive(state);
                break;
            case ProductType.Dessert:
                dessertNotifications.SetActive(state);
                break;
        }
    }

    // Ürünleri "görüldü" olarak işaretleyen ve UI'ı kapatan fonksiyon (Eskiden IncomeManager'daydı)
    public void ClearNotification(int productTypeInt, List<ProductData> products)
    {
        ProductType type = (ProductType)productTypeInt;

        // O kategoriye ait ürünlerin durumunu güncelle
        foreach (var p in products)
        {
            if (p.config.productType == type)
            {
                p.isNewlyUnlocked = false;
                p.hasBeenSeen = true;
            }
        }

        // UI ikonlarını söndür
        SetNotificationUI(type, false);
    }
}