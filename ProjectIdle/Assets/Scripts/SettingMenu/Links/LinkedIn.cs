using UnityEngine;

public class LinkedIn : MonoBehaviour
{
    // Butona bas�ld���nda �a�r�lacak olan fonksiyon
    public void LinkedInSayfasiniAc()
    {
        // Sayfan�z�n URL'sini buraya yap��t�r�n.
        // Web taray�c�s�n� a�ar ve belirtilen URL'ye gider.
        Application.OpenURL("https://www.linkedin.com/company/iku-roverium-club/posts/?feedView=all");
    }
}