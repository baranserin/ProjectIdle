using UnityEngine;
using UnityEngine.UI; // UI k�t�phanesini ekleyin

public class SettingController : MonoBehaviour
{
    // Animasyonu tetiklemek i�in panelin Animator bile�enini burada tutaca��z.
    public Animator menuPanelAnimator;

    // Men� panelinin a��k m� kapal� m� oldu�unu kontrol etmek i�in bir de�i�ken
    private bool isMenuAcik = false;

    // Bu fonksiyon butona t�kland���nda �a�r�lacak
    public void MenuyuAcKapa()
    {
        isMenuAcik = !isMenuAcik; // De�eri tersine �evir

        // E�er men� a��k olacaksa
        if (isMenuAcik)
        {
            // Panel objesini g�r�n�r yap�n
            menuPanelAnimator.gameObject.SetActive(true);
            // Animasyon tetikleyicisini �a��r�n
            menuPanelAnimator.SetTrigger("MenuAcil");
        }
        else // E�er men� kapanacaksa
        {
 
            menuPanelAnimator.gameObject.SetActive(false);
        }
    }
}
