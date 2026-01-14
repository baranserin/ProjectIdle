using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class FadeEnterLoading : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI infoText;
    public Animator transitionAnimator; // YENÝ: Animasyon kontrolcüsü

    [Header("Settings")]
    public string sceneToLoad = "SampleScene";
    public string finalMessage = "ENTER";

    [Header("Animation Settings")]
    [Tooltip("Animator içindeki Trigger parametresinin adý.")]
    public string animTriggerName = "End"; // YENÝ: Animasyonu baþlatan trigger adý
    [Tooltip("Animasyonun ne kadar süreceði (Sahne geçiþi bu süre kadar bekler).")]
    public float transitionDuration = 1.0f; // YENÝ: Bekleme süresi

    [Header("Text Effects")]
    public float flashSpeed = 3.0f;

    private bool isExiting = false;

    void Start()
    {
        if (infoText != null)
        {
            infoText.text = finalMessage;
            SetTextAlpha(0f);
        }

        StartCoroutine(LoadGameScene());
    }

    IEnumerator LoadGameScene()
    {
        // Sahneyi arka planda yüklemeye baþla
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // Yükleme hazýr olduðunda (%90)
            if (operation.progress >= 0.9f)
            {
                // --- SENARYO 1: HENÜZ TIKLANMADI (BEKLEME MODU) ---
                if (!isExiting)
                {
                    // Yanýp sönme efekti
                    float flashValue = (Mathf.Sin(Time.time * flashSpeed) + 1f) / 2f;
                    SetTextAlpha(flashValue);

                    // TIKLAMA ALGILANDIÐINDA
                    if (Input.GetMouseButtonDown(0))
                    {
                        StartCoroutine(PlayAnimAndSwitch(operation));
                    }
                }
            }
            else
            {
                // Yüklenirken yazý görünmez olsun
                SetTextAlpha(0f);
            }

            yield return null;
        }
    }

    // YENÝ: Animasyonu oynatýp sahneyi deðiþtiren Coroutine
    IEnumerator PlayAnimAndSwitch(AsyncOperation operation)
    {
        isExiting = true;

        // 1. Týklanýr týklanmaz yazýyý yok et (Ýsteðe baðlý, temiz görüntü için)
        SetTextAlpha(0f);

        // 2. Animasyonu tetikle
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger(animTriggerName);
        }

        // 3. Animasyonun bitmesi için belirlediðin süre kadar bekle
        yield return new WaitForSeconds(transitionDuration);

        // 4. Sahne geçiþine izin ver
        operation.allowSceneActivation = true;
    }

    void SetTextAlpha(float alpha)
    {
        if (infoText != null)
        {
            Color currentColor = infoText.color;
            currentColor.a = Mathf.Clamp01(alpha);
            infoText.color = currentColor;
        }
    }
}