using UnityEngine;

public class LinkedInButton : MonoBehaviour
{
    public void OpenLinkedInCompanyPage()
    {
        // LinkedIn �irket sayfas�n�n URL �emas�n� olu�tur.
        // Genellikle kullan�c� ad�n� kullanmak daha kolayd�r.
        string appUrl = "linkedin://company/iku-roverium-club";

        // Uygulama y�kl� de�ilse veya bir hata olu�ursa yedek olarak a��lacak web URL'si.
        string webUrl = "https://www.linkedin.com/company/iku-roverium-club/posts/?feedView=all";

        try
        {
            // �nce uygulamay� a�may� dene.
            Application.OpenURL(appUrl);
        }
        catch (System.Exception)
        {
            // Uygulama a��lmazsa web sitesini a�.
            Application.OpenURL(webUrl);
        }
    }
}