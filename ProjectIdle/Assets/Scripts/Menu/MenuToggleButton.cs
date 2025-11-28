using UnityEngine;
using UnityEngine.UI;

public class MenuToggleButton : MonoBehaviour
{
    public Animator animator;

    public Image buttonImage;
    public Color normalColor = Color.white;
    public Color selectedColor = new Color(0.8f, 0.8f, 0.8f); // hafif kararma

    [HideInInspector] public bool isOpen = false;

    private MenuManager menuManager;

    private void Awake()
    {
        menuManager = GameObject.FindFirstObjectByType<MenuManager>();

        if (menuManager == null)
            Debug.LogError("Sahnede MenuManager bulunamadı!");
    }

    private void Start()
    {
        if (buttonImage != null)
            buttonImage.color = normalColor;
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
            animator.ResetTrigger("Close");
            isOpen = true;

            SetSelected(true);
        }
    }

    public void CloseSlider()
    {
        if (animator != null && isOpen)
        {
            animator.SetTrigger("Close");
            animator.ResetTrigger("Open");
            isOpen = false;

            SetSelected(false);
        }
    }

    // ✅ BUTON RENGİNİ DEĞİŞTİREN FONKSİYON
    public void SetSelected(bool selected)
    {
        if (buttonImage != null)
            buttonImage.color = selected ? selectedColor : normalColor;
    }
}
