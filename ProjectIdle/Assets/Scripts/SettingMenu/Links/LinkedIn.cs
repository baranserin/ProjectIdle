using UnityEngine;

public class LinkedInButton : MonoBehaviour
{
    public void OpenLinkedInCompanyPage()
    {
        // LinkedIn þirket sayfasýnýn URL þemasýný oluþtur.
        // Genellikle kullanýcý adýný kullanmak daha kolaydýr.
        string appUrl = "linkedin://company/iku-roverium-club";

        // Uygulama yüklü deðilse veya bir hata oluþursa yedek olarak açýlacak web URL'si.
        string webUrl = "https://www.linkedin.com/company/iku-roverium-club/posts/?feedView=all";

        try
        {
            // Önce uygulamayý açmayý dene.
            Application.OpenURL(appUrl);
        }
        catch (System.Exception)
        {
            // Uygulama açýlmazsa web sitesini aç.
            Application.OpenURL(webUrl);
        }
    }
}