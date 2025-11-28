using UnityEngine;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    // Her menünün açık/kapalı durumunu tutar
    private Dictionary<int, bool> menuOpenState = new Dictionary<int, bool>();

    // Her menünün slider animator referansını tutar
    private Dictionary<int, List<MenuToggleButton>> buttonsByMenu
        = new Dictionary<int, List<MenuToggleButton>>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Butonlar sahnede register edilir
    public void RegisterButton(MenuToggleButton button)
    {
        int id = button.menuID;

        if (!buttonsByMenu.ContainsKey(id))
            buttonsByMenu[id] = new List<MenuToggleButton>();

        if (!buttonsByMenu[id].Contains(button))
            buttonsByMenu[id].Add(button);

        if (!menuOpenState.ContainsKey(id))
            menuOpenState[id] = false;
    }

    public void ToggleButton(MenuToggleButton clickedButton)
    {
        int id = clickedButton.menuID;

        bool isOpen = menuOpenState[id];

        if (isOpen)
        {
            // ✅ Aynı menü → KAPAT
            CloseMenu(id);
        }
        else
        {
            // ✅ ÖNCE TÜM DİĞER MENÜLERİ KAPAT
            CloseAllExcept(id);

            // ✅ SONRA SEÇİLEN MENÜYÜ AÇ
            OpenMenu(id);
        }
    }

    private void OpenMenu(int id)
    {
        if (buttonsByMenu.ContainsKey(id))
        {
            foreach (var button in buttonsByMenu[id])
                button.OpenSlider();
        }

        menuOpenState[id] = true;
    }

    private void CloseMenu(int id)
    {
        if (buttonsByMenu.ContainsKey(id))
        {
            foreach (var button in buttonsByMenu[id])
                button.CloseSlider();
        }

        menuOpenState[id] = false;
    }

    private void CloseAllExcept(int exceptID)
    {
        foreach (var kvp in buttonsByMenu)
        {
            int id = kvp.Key;

            if (id == exceptID) continue;

            if (menuOpenState.ContainsKey(id) && menuOpenState[id])
            {
                CloseMenu(id);
            }
        }
    }
}
