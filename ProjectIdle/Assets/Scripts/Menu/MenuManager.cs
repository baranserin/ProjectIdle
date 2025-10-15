using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [HideInInspector] public MenuToggleButton currentlyOpenButton;

    private void Awake()
    {
        // Singleton g�venli
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

    // Men� a�ma butonlar� bu fonksiyonu �a��racak
    public void ToggleButton(MenuToggleButton clickedButton)
    {
        // E�er ba�ka bir men� a��ksa kapat
        if (currentlyOpenButton != null && currentlyOpenButton != clickedButton)
        {
            currentlyOpenButton.CloseSlider();
        }

        if (currentlyOpenButton == clickedButton)
        {
            // Ayn� men�ye tekrar bas�l�rsa kapat
            CloseCurrentlyOpenMenu();
        }
        else
        {
            clickedButton.OpenSlider();
            currentlyOpenButton = clickedButton;
        }
    }

    // A��k men�y� kapat
    public void CloseCurrentlyOpenMenu()
    {
        if (currentlyOpenButton != null)
        {
            currentlyOpenButton.CloseSlider();
            currentlyOpenButton = null;
        }
    }
}
