using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [HideInInspector] public MenuToggleButton currentlyOpenButton;

    private void Awake()
    {
        // Singleton güvenli
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this); // duplicate varsa sadece bu script yok olur, objeler silinmez
            return;
        }
    }

    // Menü açma butonlarý bu fonksiyonu çaðýracak
    public void ToggleButton(MenuToggleButton clickedButton)
    {
        // Eðer baþka bir menü açýksa kapat
        if (currentlyOpenButton != null && currentlyOpenButton != clickedButton)
        {
            currentlyOpenButton.CloseSlider();
        }

        if (currentlyOpenButton == clickedButton)
        {
            // Ayný menüye tekrar basýlýrsa kapat
            CloseCurrentlyOpenMenu();
        }
        else
        {
            clickedButton.OpenSlider();
            currentlyOpenButton = clickedButton;
        }
    }

    // Açýk menüyü kapat
    public void CloseCurrentlyOpenMenu()
    {
        if (currentlyOpenButton != null)
        {
            currentlyOpenButton.CloseSlider();
            currentlyOpenButton = null;
        }
    }
}
