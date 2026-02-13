using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.InputSystem;

public class ManageMenu : MonoBehaviour
{
    [System.Serializable]
    public class MenuItem
    {
        public RectTransform menu;
        public float closedY;
        public float openY;

        [Header("Aktif Olacak Paneller")]
        public GameObject[] panelsToActivate; // 👈 Buraya panelleri ekle

        [HideInInspector] public Coroutine routine;
    }

    public MenuItem[] menus;

    [Header("Animasyon")]
    public float openDuration1 = 0.25f;
    public float openDuration2 = 0.15f;
    public float openDuration3 = 0.15f;
    public float closeDuration = 0.3f;
    public float bounceAmount = 30f;

    int currentOpenIndex = -1;

    void Start()
    {
        for (int i = 0; i < menus.Length; i++)
        {
            SetMenuY(menus[i].menu, menus[i].closedY);
            SetPanelsActive(i, false); // Başlangıçta tüm paneller kapalı
        }
    }

    public void ToggleMenu(int index)
    {
        if (index < 0 || index >= menus.Length) return;

        if (currentOpenIndex == index)
        {
            CloseMenu(index);
            return;
        }

        if (currentOpenIndex != -1)
        {
            CloseMenu(currentOpenIndex);
        }

        OpenMenu(index);
        currentOpenIndex = index;
    }

    public void CloseMenu(int index)
    {
        if (index < 0 || index >= menus.Length) return;

        StopIfRunning(index);
        menus[index].routine = StartCoroutine(
            Slide(menus[index].menu, menus[index].closedY, closeDuration)
        );

        SetPanelsActive(index, false); // 👈 Panelleri kapat

        if (currentOpenIndex == index)
        {
            currentOpenIndex = -1;
        }
    }

    void OpenMenu(int index)
    {
        StopIfRunning(index);
        menus[index].routine = StartCoroutine(OpenWithBounce(menus[index]));

        SetPanelsActive(index, true); // 👈 Panelleri aç
    }

    // 👇 Yeni metod
    void SetPanelsActive(int index, bool active)
    {
        if (menus[index].panelsToActivate == null) return;

        foreach (GameObject panel in menus[index].panelsToActivate)
        {
            if (panel != null)
            {
                panel.SetActive(active);
            }
        }
    }

    IEnumerator OpenWithBounce(MenuItem item)
    {
        float baseY = item.openY;
        yield return Slide(item.menu, baseY + bounceAmount, openDuration1);
        yield return Slide(item.menu, baseY - bounceAmount, openDuration2);
        yield return Slide(item.menu, baseY, openDuration3);
    }

    IEnumerator Slide(RectTransform menu, float targetY, float duration)
    {
        float startY = menu.anchoredPosition.y;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float x = t / duration;
            x = x * x * (3f - 2f * x);

            menu.anchoredPosition = new Vector2(
                menu.anchoredPosition.x,
                Mathf.Lerp(startY, targetY, x)
            );
            yield return null;
        }

        SetMenuY(menu, targetY);
    }

    void StopIfRunning(int index)
    {
        if (menus[index].routine != null)
            StopCoroutine(menus[index].routine);
    }

    void SetMenuY(RectTransform menu, float y)
    {
        menu.anchoredPosition = new Vector2(menu.anchoredPosition.x, y);
    }
}

