using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    private MenuToggleButton currentlyOpenButton;

    private void Awake()
    {
        // Singleton deseni: Sahnede sadece bir tane MenuManager olmasýný saðlar.
        if (Instance == null)
        {
            Instance = this;
        }
      
    }

    // Butonlar bu fonksiyonu çaðýracak
    public void ToggleButton(MenuToggleButton clickedButton)
    {
        // Eðer baþka bir buton açýksa, önce onu kapat
        if (currentlyOpenButton != null && currentlyOpenButton != clickedButton)
        {
            currentlyOpenButton.CloseSlider();
        }

        // Týklanan butonun durumunu deðiþtir
        if (currentlyOpenButton == clickedButton)
        {
            // Ayný butona tekrar basýlýrsa kapat
            CloseCurrentlyOpenMenu();
        }
        else
        {
            // Yeni butonu aç
            clickedButton.OpenSlider();
            currentlyOpenButton = clickedButton;
        }
    }

    // O an açýk olan menüyü kapatmak için kullanýlýr
    public void CloseCurrentlyOpenMenu()
    {
        if (currentlyOpenButton != null)
        {
            currentlyOpenButton.CloseSlider();
            currentlyOpenButton = null;
        }
    }
}