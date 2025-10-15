using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    // 1. ADIM: Inspector'dan atayaca��n kapatma panelini ekle
    public GameObject closePanel;

    private MenuToggleButton currentlyOpenButton;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
     
    }

   

    // Butonlar buray� �a��racak
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
            // Ayn� butona tekrar bas�l�rsa kapat (Yeni fonksiyonu �a��r�yoruz)k
            CloseCurrentlyOpenMenu();
        }
        else
        {
            // Yeni butonu a�
            clickedButton.OpenSlider();
            currentlyOpenButton = clickedButton;
         
        }
    }

    // 2. ADIM: Bo� alan paneli bu fonksiyonu �a��racak
    public void CloseCurrentlyOpenMenu()
    {
        if (currentlyOpenButton != null)
        {
            currentlyOpenButton.CloseSlider();
            currentlyOpenButton = null;
          
        }
    }
}