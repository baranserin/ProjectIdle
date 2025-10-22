    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using System.Collections.Generic;

    public class MenuFadeController : MonoBehaviour
    {
        public CanvasGroup menuGroup;          // Fade yapılacak panel
        public List<Button> fadeButtons;       // Birden fazla buton
        public float fadeDuration = 1f;
        public float waitTime = 1f;

        private void Start()
        {
            foreach (Button btn in fadeButtons)
            {
                if (btn != null)
                    btn.onClick.AddListener(() => StartCoroutine(FadeMenuCoroutine()));
            }
        }

        private IEnumerator FadeMenuCoroutine()
        {
            // Fade out
            for (float t = 0; t < 1f; t += Time.deltaTime / fadeDuration)
            {
                menuGroup.alpha = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }
            menuGroup.alpha = 0f;

            // Bekleme süresi
            yield return new WaitForSeconds(waitTime);

            // Fade in
            for (float t = 0; t < 1f; t += Time.deltaTime / fadeDuration)
            {
                menuGroup.alpha = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }
            menuGroup.alpha = 1f;
        }
    }
