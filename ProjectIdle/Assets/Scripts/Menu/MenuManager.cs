using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    private MenuToggleButton currentlyOpenButton;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Butonlar buray� �a��racak
    public void ToggleButton(MenuToggleButton clickedButton)
    {
        if (currentlyOpenButton != null && currentlyOpenButton != clickedButton)
        {
            currentlyOpenButton.CloseSlider();
        }

        if (currentlyOpenButton == clickedButton)
        {
            // Ayn� butona tekrar bas�l�rsa kapat
            clickedButton.CloseSlider();
            currentlyOpenButton = null;
        }
        else
        {
            clickedButton.OpenSlider();
            currentlyOpenButton = clickedButton;
        }
    }
}
