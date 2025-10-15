using UnityEngine;

public class MenuToggleButton : MonoBehaviour
{
    // Inspector'dan bu butona ait Animator'� s�r�kle
    public Animator animator;

    [HideInInspector] public bool isOpen = false;

    // Butonun OnClick eventi bu fonksiyonu �a��racak
    public void OnToggleMenu()
    {
        // Orkestra �efine haber ver: "Bana t�kland�!"
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.ToggleButton(this);
        }
        else
        {
            Debug.LogError("Sahnede MenuManager bulunamad�!");
        }
    }

    // Slider'� A� (Sadece bu men�ye ait)
    public void OpenSlider()
    {
        if (animator != null && !isOpen)
        {
            animator.SetTrigger("Open");
            isOpen = true;
        }
    }

    // Slider'� KAPAT (Sadece bu men�ye ait)
    public void CloseSlider()
    {
        if (animator != null && isOpen)
        {
            animator.SetTrigger("Close");
            isOpen = false;
        }
    }
}