using UnityEngine;

public class MenuToggleButton : MonoBehaviour
{
    public Animator animator;

    [HideInInspector] public bool isOpen = false;

    // MenuManager �zerinden �a�r�lacak
    public void OnToggleMenu()
    {
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.ToggleButton(this);
        }
    }

    // Slider a�mak i�in
    public void OpenSlider()
    {
        if (animator != null && !isOpen)
        {
            animator.SetTrigger("Open");
            isOpen = true;
        }
    }

    // Slider kapatmak i�in
    public void CloseSlider()
    {
        if (animator != null && isOpen)
        {
            animator.SetTrigger("Close");
            isOpen = false;
        }
    }
}
