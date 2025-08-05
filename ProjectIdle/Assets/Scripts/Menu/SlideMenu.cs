using UnityEngine;

public class SlideMenu : MonoBehaviour
{
    public RectTransform panel; // Assign the menu panel
    public float slideSpeed = 500f;
    public Vector2 hiddenPosition;
    public Vector2 shownPosition;
    private bool isVisible = false;

    void Start()
    {
        // Set panel to hidden position at start
        panel.anchoredPosition = hiddenPosition;
    }

    public void TogglePanel()
    {
        StopAllCoroutines(); // Stop ongoing animations
        StartCoroutine(Slide(isVisible ? hiddenPosition : shownPosition));
        isVisible = !isVisible;
    }

    System.Collections.IEnumerator Slide(Vector2 target)
    {
        while (Vector2.Distance(panel.anchoredPosition, target) > 0.1f)
        {
            panel.anchoredPosition = Vector2.Lerp(panel.anchoredPosition, target, Time.deltaTime * 10);
            yield return null;
        }
        panel.anchoredPosition = target;
    }
}
