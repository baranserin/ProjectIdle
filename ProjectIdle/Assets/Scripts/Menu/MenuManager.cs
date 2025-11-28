using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [HideInInspector] public MenuToggleButton currentlyOpenButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
            return;
        }
    }

    // Menü aç/kapat kontrolü
    public void ToggleButton(MenuToggleButton clickedButton)
    {
        // Eðer baþka bir menü açýksa kapat
        if (currentlyOpenButton != null && currentlyOpenButton != clickedButton)
        {
            currentlyOpenButton.CloseSlider();
        }

        // Ayný butona tekrar basýldýysa kapat
        if (currentlyOpenButton == clickedButton)
        {
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
