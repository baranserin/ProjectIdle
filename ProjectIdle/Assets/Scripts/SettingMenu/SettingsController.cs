using UnityEngine;
using UnityEngine.UI; // UI kütüphanesini ekleyin

public class SettingController : MonoBehaviour
{
    // Animasyonu tetiklemek için panelin Animator bileşenini burada tutacağız.
    public Animator menuPanelAnimator;

    // Menü panelinin açık mı kapalı mı olduğunu kontrol etmek için bir değişken
    private bool isMenuAcik = false;

    // Bu fonksiyon butona tıklandığında çağrılacak
    public void MenuyuAcKapa()
    {
        isMenuAcik = !isMenuAcik; // Değeri tersine çevir

        // Eğer menü açık olacaksa
        if (isMenuAcik)
        {
            // Panel objesini görünür yapın
            menuPanelAnimator.gameObject.SetActive(true);
            // Animasyon tetikleyicisini çağırın
            menuPanelAnimator.SetTrigger("MenuAcil");
        }
        else // Eğer menü kapanacaksa
        {
 
            menuPanelAnimator.gameObject.SetActive(false);
        }
    }
    public void MenuyuKapat()
    {
        isMenuAcik = false;
        menuPanelAnimator.gameObject.SetActive(false);
    }
}
