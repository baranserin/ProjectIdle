using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    private MenuToggleButton currentlyOpenButton;

    private void Awake()
    {
        // Singleton deseninin DO�RU ve G�VENL� hali
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            // E�er sahnede zaten ba�ka bir y�netici varsa, bu yeni olan kendini yok etsin.
            Destroy(gameObject);
            return;
        }
    }

    // Butonlar bu fonksiyonu �a��racak
    public void ToggleButton(MenuToggleButton clickedButton)
    {
        if (currentlyOpenButton != null && currentlyOpenButton != clickedButton)
        {
            currentlyOpenButton.CloseSlider();
        }

        if (currentlyOpenButton == clickedButton)
        {
            currentlyOpenButton.CloseSlider();
            currentlyOpenButton = null;
        }
        else
        {
            clickedButton.OpenSlider();
            currentlyOpenButton = clickedButton;
        }
    }
}