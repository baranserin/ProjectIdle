using UnityEngine;
using UnityEngine.UI;

public class LockVisual : MonoBehaviour
{
    [Header("Saydamlaþacak Mevcut Resim")]
    public Image hedefResim; // Butonun kendi Image bileþeni veya içindeki ikon

    // Oyun baþýnda LockManager bu fonksiyonu çaðýrýp resmi saydamlaþtýracak
    public void KilitliYap()
    {
        if (hedefResim != null)
        {
            Color renk = hedefResim.color;
            renk.a = 0.5f; // %50 Saydamlýk (Alpha)
            hedefResim.color = renk;
        }
    }

    // Seviye yetince LockManager bu fonksiyonu çaðýrýp saydamlýðý kaldýracak
    public void KilidiAc()
    {
        if (hedefResim != null)
        {
            Color renk = hedefResim.color;
            renk.a = 1f; // %100 Görünürlük (Alpha tam dolu)
            hedefResim.color = renk;
        }
    }
}
