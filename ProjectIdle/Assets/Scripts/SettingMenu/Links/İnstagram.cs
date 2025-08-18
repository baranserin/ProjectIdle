using UnityEngine;

public class InstagramButton : MonoBehaviour
{
    public void OpenInstagramProfile()
    {
        // Instagram kullanıcı adınızı buraya yazın
        string username = "iku_roverium";

        // Açılacak URL'yi tutan değişken
        string urlToOpen;

        // Platforma göre doğru URL'yi belirle
#if UNITY_ANDROID
        urlToOpen = "instagram://user?username=" + username;
#elif UNITY_IOS
        urlToOpen = "instagram://user?username=" + username;
#else
        // Diğer platformlar için web URL'sini kullan
        urlToOpen = "https://www.instagram.com/" + username;
#endif

        // Uygulamayı açmayı dene
        // Bu try-catch bloğu sadece mobil platformlarda anlamlıdır.
        // Masaüstünde direkt web URL'si açılacağı için hata beklenmez.
        try
        {
            Application.OpenURL(urlToOpen);
        }
        catch (System.Exception)
        {
            // Eğer uygulama açılmazsa (Android/iOS), web sitesini aç
            Application.OpenURL("https://www.instagram.com/" + username);
        }
    }
}