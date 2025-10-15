using UnityEngine;

public class MenuToggleButton : MonoBehaviour
{
    public Animator animator;
    [HideInInspector] public bool isOpen = false;

    private MenuManager menuManager;

    private void Awake()
    {
        // Unity 2023+ için güncel referans alma
        menuManager = GameObject.FindFirstObjectByType<MenuManager>();

        if (menuManager == null)
            Debug.LogError("Sahnede MenuManager bulunamadý!");
    }

    public void OnToggleMenu()
    {
        if (menuManager != null)
            menuManager.ToggleButton(this);
    }

    public void OpenSlider()
    {
        if (animator != null && !isOpen)
        {
            animator.SetTrigger("Open");
            isOpen = true;
            animator.ResetTrigger("Close");
        }
    }

    public void CloseSlider()
    {
        if (animator != null && isOpen)
        {
            animator.SetTrigger("Close");
            isOpen = false;
            animator.ResetTrigger("Open");
        }
    }
}
