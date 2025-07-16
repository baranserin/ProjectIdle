using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public GameObject menuToToggle;

    public void OnButtonClick()
    {
        MenuController.Instance.ToggleMenu(menuToToggle);
    }
}
