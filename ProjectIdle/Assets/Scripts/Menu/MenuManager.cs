using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    // 1. ADIM: Inspector'dan atayacaðýn kapatma panelini ekle
    public GameObject closePanel;

    private MenuToggleButton currentlyOpenButton;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
     
    }

   

    // Butonlar burayý çaðýracak
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
            // Ayný butona tekrar basýlýrsa kapat (Yeni fonksiyonu çaðýrýyoruz)k
            CloseCurrentlyOpenMenu();
        }
        else
        {
            // Yeni butonu aç
            clickedButton.OpenSlider();
            currentlyOpenButton = clickedButton;
         
        }
    }

    // 2. ADIM: Boþ alan paneli bu fonksiyonu çaðýracak
    public void CloseCurrentlyOpenMenu()
    {
        if (currentlyOpenButton != null)
        {
            currentlyOpenButton.CloseSlider();
            currentlyOpenButton = null;
          
        }
    }
}