using UnityEngine;

public class MenuToggleButton : MonoBehaviour
{
    public int menuID;   // 0, 1, 2, 3...
    public Animator animator;

    // Awake yerine Start kullanıyoruz ki Manager kesinlikle yüklenmiş olsun.
    private void Start()
    {
        if (MenuManager.Instance == null)
        {
            Debug.LogError("HATA: MenuManager sahnede bulunamadı veya henüz yüklenmedi!");
            return;
        }

        // ✅ BUTON MANAGERA GÜVENLE KAYDEDİLİYOR
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