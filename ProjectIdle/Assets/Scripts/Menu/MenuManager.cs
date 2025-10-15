using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    private MenuToggleButton currentlyOpenButton;

    private void Awake()
    {
        // Singleton deseninin DOÐRU ve GÜVENLÝ hali
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            // Eðer sahnede zaten baþka bir yönetici varsa, bu yeni olan kendini yok etsin.
            Destroy(gameObject);
            return;
        }
    }

    // Butonlar bu fonksiyonu çaðýracak
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