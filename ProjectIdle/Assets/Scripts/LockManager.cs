using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.Collections.Generic; // Liste yap�s�n� kullanabilmek i�in gerekli

// Unity Inspector'da gruplar halinde g�r�nmesini sa�layacak �zel veri yap�m�z
[System.Serializable]
public class LockData
{
    public TextMeshProUGUI seviyeTexti;
    public int hedefSeviye;
    public Button acilacakButon;

    [Header("G�rsel Saydaml�k Kontrol�")]
    public LockVisual gorselKontrol; // YEN� EKLENEN BO�LUK
}

public class LockManager : MonoBehaviour
{
    [Header("Kilit A�ma Kurallar� (Art� butonuna basarak ekle)")]
    // LockData tasla��m�zdan olu�an sonsuz uzunlukta bir liste
    public List<LockData> kilitKurallari;

    void Start()
    {
        // Oyun ba�lad���nda listedeki t�m butonlar� otomatik olarak kilitler
        foreach (var kural in kilitKurallari)
        {
            if (kural.acilacakButon != null)
            {
                kural.acilacakButon.interactable = false;
            }
            // Ba�lang��ta hedef resmi %50 saydam yap
            if (kural.gorselKontrol != null)
            {
                kural.gorselKontrol.KilitliYap();
            }
        }
    }

    // Bu fonksiyonu �r�nlerin "Upgrade" butonlar�na ba�layacaks�n
    public void SeviyeleriKontrolEt()
    {
        // T�klama yap�ld���nda listedeki b�t�n kurallar� tek tek kontrol et
        foreach (var kural in kilitKurallari)
        {
            if (kural.seviyeTexti != null)
            {
                string ekrandakiYazi = kural.seviyeTexti.text;
                string sadeceSayi = Regex.Match(ekrandakiYazi, @"\d+").Value;

                if (!string.IsNullOrEmpty(sadeceSayi))
                {
                    int gercekSeviye = int.Parse(sadeceSayi);

                    // Hedef seviyeye ula��ld�ysa veya ge�ildiyse kilidi a�
                    if (gercekSeviye >= kural.hedefSeviye)
                    {
                        if (kural.acilacakButon != null)
                        {
                            kural.acilacakButon.interactable = true;

                            // 2. Resmin saydaml���n� kald�r�p tam g�r�n�r yap
                            if (kural.gorselKontrol != null)
                            {
                                kural.gorselKontrol.KilidiAc();
                            }
                        }
                    }
                }
            }
        }
    }
}
