using UnityEngine;

public class LinkedInButton : MonoBehaviour
{
    public void OpenLinkedInProfile()
    {
        // LinkedIn sayfa linkin
        string username = "company/iku-roverium-club/posts/?feedView=all";

        // Açýlacak URL'yi tutan deðiþken
        string urlToOpen;

        // Platforma göre doðru URL'yi belirle
#if UNITY_ANDROID
        urlToOpen = "linkedin://" + username;
#elif UNITY_IOS
        urlToOpen = "linkedin://" + username;
#else
        // Diðer platformlar için web URL'sini kullan
        urlToOpen = "https://www.linkedin.com/" + username;
#endif

        // Uygulamayý açmayý dene
        try
        {
            Application.OpenURL(urlToOpen);
        }
        catch (System.Exception)
        {
            // Eðer uygulama açýlmazsa (Android/iOS), web sitesini aç
            Application.OpenURL("https://www.linkedin.com/" + username);
        }
    }
}
