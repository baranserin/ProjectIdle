using UnityEngine;

public class MenuToggleButton : MonoBehaviour
{
    public int menuID;   // 0, 1, 2
    public Animator animator;

    private void Awake()
    {
        if (MenuManager.Instance == null)
        {
            Debug.LogError("SAHNEDE MenuManager YOK!");
            return;
        }

        // ✅ BUTON MANAGERA KAYDEDİLİYOR
        MenuManager.Instance.RegisterButton(this);
    }

    public void OnToggleMenu()
    {
        if (MenuManager.Instance != null)
            MenuManager.Instance.ToggleButton(this);
    }

    public void OpenSlider()
    {
        if (animator != null)
        {
            animator.ResetTrigger("Close");
            animator.SetTrigger("Open");
        }
    }

    public void CloseSlider()
    {
        if (animator != null)
        {
            animator.ResetTrigger("Open");
            animator.SetTrigger("Close");
        }
    }
}
