using UnityEngine;

public class MenuToggleButton : MonoBehaviour
{
    // Inspector'dan bu butona ait Animator'ü sürükle
    public Animator animator;

    [HideInInspector] public bool isOpen = false;

    // Butonun OnClick eventi bu fonksiyonu çaðýracak
    public void OnToggleMenu()
    {
        // Orkestra þefine haber ver: "Bana týklandý!"
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.ToggleButton(this);
        }
        else
        {
            Debug.LogError("Sahnede MenuManager bulunamadý!");
        }
    }

    // Slider'ý AÇ (Sadece bu menüye ait)
    public void OpenSlider()
    {
        if (animator != null && !isOpen)
        {
            animator.SetTrigger("Open");
            isOpen = true;
        }
    }

    // Slider'ý KAPAT (Sadece bu menüye ait)
    public void CloseSlider()
    {
        if (animator != null && isOpen)
        {
            animator.SetTrigger("Close");
            isOpen = false;
        }
    }
}