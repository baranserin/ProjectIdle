using UnityEngine;

public class MenuGroupController : MonoBehaviour
{
    public GameObject[] categoryMenus;

    public void ShowCategory(int index)
    {
        if (index < 0 || index >= categoryMenus.Length) return;

        for (int i = 0; i < categoryMenus.Length; i++)
        {
            categoryMenus[i].SetActive(i == index);
        }
    }

    public void ShowDefaultCategory()
    {
        ShowCategory(0); // Always show first tab by default
    }
}
