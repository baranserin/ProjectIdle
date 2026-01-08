using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class FadeEnterLoading : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI infoText; // Fade olacak "ENTER" yazýsý

    [Header("Settings")]
    public string sceneToLoad = "SampleScene"; // Yüklenecek sahne adý
    public string finalMessage = "ENTER"; // Gözükecek mesaj
    [Tooltip("Yazýnýn ne kadar hýzlý belirip kaybolacaðýný belirler. Yüksek deðer = hýzlý fade.")]
    public float fadeSpeed = 3.0f; // Fade hýzý

    void Start()
    {
        // Baþlangýçta metni ayarla ama tamamen þeffaf yap (görünmez)
        if (infoText != null)
        {
            infoText.text = finalMessage;
            SetTextAlpha(0f);
        }

        StartCoroutine(LoadGameScene());
    }

    IEnumerator LoadGameScene()
    {
        // Sahneyi arka planda yükle
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // Yükleme bittiðinde (Unity'de %90'da durur)
            if (operation.progress >= 0.9f)
            {
                // --- YÜKLEME BÝTTÝ: FADE EFEKTÝ ---

                // Mathf.Sin -1 ile 1 arasýnda deðer üretir.
                // Bunu 0 ile 1 arasýna (Alpha deðerine) dönüþtürmek için formül:
                float fadeValue = (Mathf.Sin(Time.time * fadeSpeed) + 1f) / 2f;

                SetTextAlpha(fadeValue);

                // Týklanýnca sahneye geç
                if (Input.GetMouseButtonDown(0))
                {
                    // Geçiþ öncesi son kez tam görünür yapalým ki kesilmesin
                    SetTextAlpha(1f);
                    operation.allowSceneActivation = true;
                }
            }
            else
            {
                // --- HALA YÜKLENÝYOR ---
                // Yükleme sürerken tamamen görünmez olduðundan emin ol
                SetTextAlpha(0f);
            }

            yield return null;
        }
    }

    // TextMeshPro'nun alphasýný deðiþtirmek için yardýmcý fonksiyon
    void SetTextAlpha(float alpha)
    {
        if (infoText != null)
        {
            Color currentColor = infoText.color;
            currentColor.a = alpha;
            infoText.color = currentColor;
        }
    }
}