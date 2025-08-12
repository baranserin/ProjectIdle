using UnityEngine;
using UnityEngine.UI; // UI kütüphanesini ekleyin

public class SettingController : MonoBehaviour
{
    // Animasyonu tetiklemek için panelin Animator bileþenini burada tutacaðýz.
    public Animator menuPanelAnimator;

    // Menü panelinin açýk mý kapalý mý olduðunu kontrol etmek için bir deðiþken
    private bool isMenuAcik = false;

    // Bu fonksiyon butona týklandýðýnda çaðrýlacak
    public void MenuyuAcKapa()
    {
        isMenuAcik = !isMenuAcik; // Deðeri tersine çevir

        // Eðer menü açýk olacaksa
        if (isMenuAcik)
        {
            // Panel objesini görünür yapýn
            menuPanelAnimator.gameObject.SetActive(true);
            // Animasyon tetikleyicisini çaðýrýn
            menuPanelAnimator.SetTrigger("MenuAcil");
        }
        else // Eðer menü kapanacaksa
        {
 
            menuPanelAnimator.gameObject.SetActive(false);
        }
    }
}
