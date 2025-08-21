using UnityEngine;

public class DecorationsMenuController : MonoBehaviour
{
    public GameObject menuPanel; // Paneli direkt referans al�yoruz
    private bool isMenuAcik = false;

    // Butona t�klan�nca �a�r�l�r
    public void MenuyuAcKapa()
    {
        isMenuAcik = !isMenuAcik; // A��k/kapal� durumunu de�i�tir
        menuPanel.SetActive(isMenuAcik);
    }

    // Paneli kapatma fonksiyonu
    public void MenuyuKapat()
    {
        isMenuAcik = false;
        menuPanel.SetActive(false);
    }
}
