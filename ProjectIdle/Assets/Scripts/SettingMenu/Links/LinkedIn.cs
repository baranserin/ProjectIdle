using UnityEngine;

public class LinkedInButton : MonoBehaviour
{
    public void OpenLinkedInProfile()
    {
        // LinkedIn sayfa linkin
        string username = "company/iku-roverium-club/posts/?feedView=all";

        // A��lacak URL'yi tutan de�i�ken
        string urlToOpen;

        // Platforma g�re do�ru URL'yi belirle
#if UNITY_ANDROID
        urlToOpen = "linkedin://" + username;
#elif UNITY_IOS
        urlToOpen = "linkedin://" + username;
#else
        // Di�er platformlar i�in web URL'sini kullan
        urlToOpen = "https://www.linkedin.com/" + username;
#endif

        // Uygulamay� a�may� dene
        try
        {
            Application.OpenURL(urlToOpen);
        }
        catch (System.Exception)
        {
            // E�er uygulama a��lmazsa (Android/iOS), web sitesini a�
            Application.OpenURL("https://www.linkedin.com/" + username);
        }
    }
}
