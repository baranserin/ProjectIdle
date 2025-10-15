using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    private MenuToggleButton currentlyOpenButton;

    private void Awake()
    {
        // Singleton deseni: Sahnede sadece bir tane MenuManager olmas�n� sa�lar.
        if (Instance == null)
        {
            Instance = this;
        }
      
    }

    // Butonlar bu fonksiyonu �a��racak
    public void ToggleButton(MenuToggleButton clickedButton)
    {
        // E�er ba�ka bir buton a��ksa, �nce onu kapat
        if (currentlyOpenButton != null && currentlyOpenButton != clickedButton)
        {
            currentlyOpenButton.CloseSlider();
        }

        // T�klanan butonun durumunu de�i�tir
        if (currentlyOpenButton == clickedButton)
        {
            // Ayn� butona tekrar bas�l�rsa kapat
            CloseCurrentlyOpenMenu();
        }
        else
        {
            // Yeni butonu a�
            clickedButton.OpenSlider();
            currentlyOpenButton = clickedButton;
        }
    }

    // O an a��k olan men�y� kapatmak i�in kullan�l�r
    public void CloseCurrentlyOpenMenu()
    {
        if (currentlyOpenButton != null)
        {
            currentlyOpenButton.CloseSlider();
            currentlyOpenButton = null;
        }
    }
}