using UnityEngine;

public class MenuToggleButton : MonoBehaviour
{
    public Animator animator;

    [HideInInspector] public bool isOpen = false;

    // MenuManager üzerinden çaðrýlacak
    public void OnToggleMenu()
    {
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.ToggleButton(this);
        }
    }

    // Slider açmak için
    public void OpenSlider()
    {
        if (animator != null && !isOpen)
        {
            animator.SetTrigger("Open");
            isOpen = true;
        }
    }

    // Slider kapatmak için
    public void CloseSlider()
    {
        if (animator != null && isOpen)
        {
            animator.SetTrigger("Close");
            isOpen = false;
        }
    }
}
