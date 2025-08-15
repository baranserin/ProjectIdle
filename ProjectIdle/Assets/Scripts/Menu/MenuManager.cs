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

    // Butonlar burayý çaðýracak
    public void ToggleButton(MenuToggleButton clickedButton)
    {
        if (currentlyOpenButton != null && currentlyOpenButton != clickedButton)
        {
            currentlyOpenButton.CloseSlider();
        }

        if (currentlyOpenButton == clickedButton)
        {
            // Ayný butona tekrar basýlýrsa kapat
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
