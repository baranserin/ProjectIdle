

using UnityEngine;

public class MenuToggleButton : MonoBehaviour
{
    public Animator animator;

    [HideInInspector] public bool isOpen = false;

    
    public void OnToggleMenu()
    {
      
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.ToggleButton(this);
        }
    }

  
    public void OpenSlider()
    {
        if (animator != null && !isOpen)
        {
            animator.SetTrigger("Open");
            isOpen = true;
        }
    }

    public void CloseSlider()
    {
       

        if (animator != null && isOpen)
        {
         
            animator.SetTrigger("Close");
            isOpen = false;
        }
       
     
    }
}