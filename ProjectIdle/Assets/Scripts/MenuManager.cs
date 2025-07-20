using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    private GameObject currentlyOpenGroup;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Called by buttons to open/close any menu group
    public void ToggleMenuGroup(GameObject group, MenuGroupController controller)
    {
        bool isAlreadyOpen = currentlyOpenGroup == group;

        CloseCurrentGroup();

        if (!isAlreadyOpen)
        {
            group.SetActive(true);
            controller.ShowDefaultCategory();
            currentlyOpenGroup = group;
        }
    }

    public void CloseCurrentGroup()
    {
        if (currentlyOpenGroup != null)
        {
            currentlyOpenGroup.SetActive(false);
            currentlyOpenGroup = null;
        }
    }
}
