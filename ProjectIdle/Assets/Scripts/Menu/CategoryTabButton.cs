using UnityEngine;

public class CategoryTabButton : MonoBehaviour
{
    [Tooltip("The MenuGroupController that manages this group (e.g., Product Menu)")]
    public MenuGroupController menuGroup;

    [Tooltip("Which category this button should show (0 = Tea, 1 = Coffee, 2 = Dessert)")]
    public int categoryIndex;

    public void OnTabClick()
    {
        if (menuGroup != null)
        {
            menuGroup.ShowCategory(categoryIndex);
        }
    }
}
