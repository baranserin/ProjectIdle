using UnityEngine;

public class MenuController : MonoBehaviour
{
    public static MenuController Instance;

    [Header("All Menus")]
    public GameObject[] menus;

    private GameObject currentOpenMenu;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ToggleMenu(GameObject menuToToggle)
    {
        // If this is already open, close it
        if (currentOpenMenu == menuToToggle)
        {
            CloseCurrentMenu();
            return;
        }

        // Close previous menu
        CloseCurrentMenu();

        // Open new one
        currentOpenMenu = menuToToggle;
        currentOpenMenu.SetActive(true);
    }

    private void CloseCurrentMenu()
    {
        if (currentOpenMenu != null)
        {
            currentOpenMenu.SetActive(false);
            currentOpenMenu = null;
        }
    }
}
