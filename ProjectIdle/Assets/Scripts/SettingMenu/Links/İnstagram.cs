using UnityEngine;

public class instagram : MonoBehaviour
{
    // Butona basıldığında çağrılacak olan fonksiyon
    public void InstagramSayfasiniAc()
    {
        // Instagram sayfanızın URL'sini buraya yapıştırın.
        // Web tarayıcısını açar ve belirtilen URL'ye gider.
        Application.OpenURL("https://www.instagram.com/iku_roverium/");
    }
}