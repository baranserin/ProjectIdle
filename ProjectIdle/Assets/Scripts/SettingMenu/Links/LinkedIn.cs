using UnityEngine;

public class LinkedIn : MonoBehaviour
{
    // Butona basýldýðýnda çaðrýlacak olan fonksiyon
    public void LinkedInSayfasiniAc()
    {
        // Sayfanýzýn URL'sini buraya yapýþtýrýn.
        // Web tarayýcýsýný açar ve belirtilen URL'ye gider.
        Application.OpenURL("https://www.linkedin.com/company/iku-roverium-club/posts/?feedView=all");
    }
}