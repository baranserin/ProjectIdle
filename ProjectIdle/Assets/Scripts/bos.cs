using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuFadeController : MonoBehaviour
{
    public CanvasGroup menuGroup; // Menü paneline ekle
    public Button fadeButton;     // Butonu baðla

    private void Start()
    {
        fadeButton.onClick.AddListener(OnFadeButtonClick);
    }

    private void OnFadeButtonClick()
    {
        StartCoroutine(FadeMenuCoroutine());
    }

    private IEnumerator FadeMenuCoroutine()
    {
        // Fade out (1'den 0'a yumuþak geçiþ)
        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            menuGroup.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        menuGroup.alpha = 0f;

        // 2 saniye bekle
        yield return new WaitForSeconds(1f);

        // Fade in (0'dan 1'e yumuþak geçiþ)
        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            menuGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
        menuGroup.alpha = 1f;
    }
}
