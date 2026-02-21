using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.Collections.Generic; // Liste yapýsýný kullanabilmek için gerekli

// Unity Inspector'da gruplar halinde görünmesini saðlayacak özel veri yapýmýz
[System.Serializable]
public class LockData
{
    public TextMeshProUGUI seviyeTexti;
    public int hedefSeviye;
    public Button acilacakButon;
}

public class LockManager : MonoBehaviour
{
    [Header("Kilit Açma Kurallarý (Artý butonuna basarak ekle)")]
    // LockData taslaðýmýzdan oluþan sonsuz uzunlukta bir liste
    public List<LockData> kilitKurallari;

    void Start()
    {
        // Oyun baþladýðýnda listedeki tüm butonlarý otomatik olarak kilitler
        foreach (var kural in kilitKurallari)
        {
            if (kural.acilacakButon != null)
            {
                kural.acilacakButon.interactable = false;
            }
        }
    }

    // Bu fonksiyonu ürünlerin "Upgrade" butonlarýna baðlayacaksýn
    public void SeviyeleriKontrolEt()
    {
        // Týklama yapýldýðýnda listedeki bütün kurallarý tek tek kontrol et
        foreach (var kural in kilitKurallari)
        {
            if (kural.seviyeTexti != null)
            {
                string ekrandakiYazi = kural.seviyeTexti.text;
                string sadeceSayi = Regex.Match(ekrandakiYazi, @"\d+").Value;

                if (!string.IsNullOrEmpty(sadeceSayi))
                {
                    int gercekSeviye = int.Parse(sadeceSayi);

                    // Hedef seviyeye ulaþýldýysa veya geçildiyse kilidi aç
                    if (gercekSeviye >= kural.hedefSeviye)
                    {
                        if (kural.acilacakButon != null)
                        {
                            kural.acilacakButon.interactable = true;
                        }
                    }
                }
            }
        }
    }
}
