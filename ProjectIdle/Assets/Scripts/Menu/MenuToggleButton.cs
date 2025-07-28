using UnityEngine;

public class MenuToggleButton : MonoBehaviour
{
    public GameObject menuGroupToToggle;
    public MenuGroupController groupController;

    public void OnToggleMenu()
    {
        MenuManager.Instance.ToggleMenuGroup(menuGroupToToggle, groupController);
    }
}
