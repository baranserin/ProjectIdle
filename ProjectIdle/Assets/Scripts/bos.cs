using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MenuFadeController : MonoBehaviour
{
    public CanvasGroup menuGroup;
    public List<Button> fadeButtons;

    public float fadeDuration = 1f;
    public float waitTime = 1f;

    private bool isFading = false; // ÇAKIŞMAYI ENGELLER

    private IEnumerator Start()
    {
        Debug.Log("MENUFADER START - Delayed");

        // 1 frame bekle ki diğer scriptler (DecorationIncome vs.) listenerları silsin
        yield return null;

        foreach (Button button in fadeButtons)
        {
            if (button != null)
            {
                Button captured = button;
                captured.onClick.AddListener(() => StartFade());
                Debug.Log("FADE LISTENER EKLENDİ → " + captured.name);
            }
        }
    }

    private void StartFade()
    {
        if (!isFading)
            StartCoroutine(FadeMenuCoroutine());
    }

    private IEnumerator FadeMenuCoroutine()
    {
        Debug.Log("FADE BAŞLADI");  // TEST

        for (float t = 0; t < 1f; t += Time.deltaTime / fadeDuration)
        {
            menuGroup.alpha = Mathf.Lerp(1f, 0f, t);
            Debug.Log("ALPHA OUT: " + menuGroup.alpha); // TEST
            yield return null;
        }

        menuGroup.alpha = 0f;
        Debug.Log("FADE OUT BİTTİ");

        yield return new WaitForSeconds(waitTime);

        for (float t = 0; t < 1f; t += Time.deltaTime / fadeDuration)
        {
            menuGroup.alpha = Mathf.Lerp(0f, 1f, t);
            Debug.Log("ALPHA IN: " + menuGroup.alpha); // TEST
            yield return null;
        }

        menuGroup.alpha = 1f;
        Debug.Log("FADE BİTTİ");
    }
    private void Awake()
    {
        Debug.Log("MENUFADER AWAKE");
    }
}
