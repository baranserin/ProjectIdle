using UnityEngine;

public class DecorationsMenuController : MonoBehaviour
{
    public GameObject menuPanel; // Paneli direkt referans alýyoruz
    private bool isMenuAcik = false;

    // Butona týklanýnca çaðrýlýr
    public void MenuyuAcKapa()
    {
        isMenuAcik = !isMenuAcik; // Açýk/kapalý durumunu deðiþtir
        menuPanel.SetActive(isMenuAcik);
    }

    // Paneli kapatma fonksiyonu
    public void MenuyuKapat()
    {
        isMenuAcik = false;
        menuPanel.SetActive(false);
    }
}
