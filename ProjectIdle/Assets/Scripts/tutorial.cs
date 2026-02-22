using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TutorialSystem : MonoBehaviour
{
    [Header("Karakter & Panel")]
    public CanvasGroup darkPanel;
    public RectTransform characterGroup;
    public CanvasGroup characterGroupCanvas;
    public RectTransform handPointer;

    [Header("Animasyon Ayarlari")]
    public float fadeDuration = 0.5f;
    public float slideDistance = 600f;
    public float slideDuration = 0.6f;
    public float handMoveDistance = 40f;
    public float handSpeed = 2f;

    [Header("Yazi UI")]
    public TextMeshProUGUI tutorialText;
    public float typingSpeed = 0.03f;

    [Header("Skip Butonu (tutorial icinde)")]
    public Button skipButton;

    [Header("Restart Butonu (tutorial disinda)")]
    public Button restartButton;

    [Header("Tutorial Adimlar")]
    public List<TutorialStep> tutorialSteps = new List<TutorialStep>();

    // ── State ──────────────────────────────────
    private int currentStepIndex = -1;
    private int currentTextIndex = -1;
    private bool handAnimating = false;
    private bool isTyping = false;
    private bool characterVisible = false;
    private float stepStartTime = 0f;
    private Coroutine typingCoroutine;
    private Vector2 characterHiddenPos;
    private bool listenersRegistered = false;

    // ─────────────────────────────────────────
    void Start()
    {
        characterHiddenPos = characterGroup.anchoredPosition;
        RegisterListeners();

        // İlk açılışta tutorial tamamlandıysa kapat
        if (PlayerPrefs.GetInt("TutorialCompleted", 0) == 1)
        {
            CleanupAllUI();
            gameObject.SetActive(false);
            return;
        }

        StartTutorialFromScratch();
    }

    void RegisterListeners()
    {
        if (listenersRegistered) return;
        listenersRegistered = true;

        if (skipButton != null)
            skipButton.onClick.AddListener(SkipTutorial);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartTutorial);

        for (int i = 0; i < tutorialSteps.Count; i++)
        {
            int idx = i;
            if (tutorialSteps[i].targetButton != null)
            {
                Button btn = tutorialSteps[i].targetButton.GetComponent<Button>();
                if (btn != null)
                    btn.onClick.AddListener(() => OnTargetButtonClicked(idx));
            }
        }
    }

    void StartTutorialFromScratch()
    {
        // State sıfırla
        currentStepIndex = -1;
        currentTextIndex = -1;
        handAnimating = false;
        characterVisible = false;
        isTyping = false;

        // UI başlangıç durumu
        characterGroup.anchoredPosition = characterHiddenPos;
        characterGroupCanvas.alpha = 0;
        darkPanel.alpha = 0;

        handPointer.gameObject.SetActive(false);
        if (tutorialText != null) tutorialText.gameObject.SetActive(false);
        if (skipButton != null) skipButton.gameObject.SetActive(true);

        darkPanel.gameObject.SetActive(true);
        characterGroup.gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(BeginStep(0));
    }

    // ─────────────────────────────────────────
    void Update()
    {
        if (handAnimating && currentStepIndex >= 0 && currentStepIndex < tutorialSteps.Count)
            AnimateHand(tutorialSteps[currentStepIndex]);

        if (characterVisible && IsScreenPressed())
            HandleScreenTap();
    }

    // ─────────────────────────────────────────
    void HandleScreenTap()
    {
        if (isTyping)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            tutorialText.ForceMeshUpdate();
            tutorialText.maxVisibleCharacters = tutorialText.textInfo.characterCount;
            isTyping = false;
        }
        else
        {
            currentTextIndex++;
            TutorialStep step = tutorialSteps[currentStepIndex];

            if (currentTextIndex < step.texts.Count)
            {
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                typingCoroutine = StartCoroutine(TypeText(step.texts[currentTextIndex]));
            }
            else
            {
                StartCoroutine(HideCharacterAndShowHand());
            }
        }
    }

    public void OnScreenTapped() => HandleScreenTap();

    // ─────────────────────────────────────────
    void OnTargetButtonClicked(int stepIndex)
    {
        if (stepIndex != currentStepIndex) return;
        if (characterVisible) return;

        handAnimating = false;
        handPointer.gameObject.SetActive(false);

        int nextStep = currentStepIndex + 1;
        if (nextStep < tutorialSteps.Count)
            StartCoroutine(BeginStep(nextStep));
        else
            EndTutorial();
    }

    // ─────────────────────────────────────────
    IEnumerator BeginStep(int stepIndex)
    {
        currentStepIndex = stepIndex;
        currentTextIndex = 0;
        TutorialStep step = tutorialSteps[stepIndex];

        yield return StartCoroutine(ShowCharacter());

        if (step.texts != null && step.texts.Count > 0)
        {
            tutorialText.gameObject.SetActive(true);
            typingCoroutine = StartCoroutine(TypeText(step.texts[0]));
        }
        else
        {
            StartCoroutine(HideCharacterAndShowHand());
        }
    }

    // ─────────────────────────────────────────
    IEnumerator ShowCharacter()
    {
        characterVisible = true;
        characterGroup.gameObject.SetActive(true);
        darkPanel.gameObject.SetActive(true);

        yield return FadeCanvasGroup(darkPanel, 0, 0.6f, fadeDuration);

        Vector2 hiddenPos = characterHiddenPos;
        Vector2 shownPos = hiddenPos + Vector2.right * slideDistance;
        characterGroupCanvas.alpha = 0;
        characterGroup.anchoredPosition = hiddenPos;

        float t = 0;
        while (t < slideDuration)
        {
            t += Time.deltaTime;
            float s = Mathf.SmoothStep(0, 1, t / slideDuration);
            characterGroup.anchoredPosition = Vector2.Lerp(hiddenPos, shownPos, s);
            characterGroupCanvas.alpha = s;
            yield return null;
        }
        characterGroupCanvas.alpha = 1;
        characterGroup.anchoredPosition = shownPos;
    }

    IEnumerator HideCharacterAndShowHand()
    {
        characterVisible = false;

        if (tutorialText != null)
            tutorialText.gameObject.SetActive(false);

        // Karakter fade out
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            characterGroupCanvas.alpha = 1 - (t / fadeDuration);
            yield return null;
        }
        characterGroupCanvas.alpha = 0;

        // Dark panel fade out
        yield return FadeCanvasGroup(darkPanel, 0.6f, 0, fadeDuration);

        darkPanel.gameObject.SetActive(false);
        characterGroup.gameObject.SetActive(false);
        characterGroup.anchoredPosition = characterHiddenPos;

        // El pointer
        TutorialStep step = tutorialSteps[currentStepIndex];
        if (step.targetButton != null)
        {
            handPointer.gameObject.SetActive(true);
            handPointer.position = step.targetButton.position + new Vector3(step.handPosition.x, step.handPosition.y, 0);
            stepStartTime = Time.time;
            handAnimating = true;
        }
    }

    // ─────────────────────────────────────────
    void CleanupAllUI()
    {
        StopAllCoroutines();
        handAnimating = false;
        characterVisible = false;
        isTyping = false;

        if (darkPanel != null)
        {
            darkPanel.alpha = 0;
            darkPanel.gameObject.SetActive(false);
        }
        if (characterGroup != null)
        {
            characterGroup.gameObject.SetActive(false);
            characterGroup.anchoredPosition = characterHiddenPos;
        }
        if (characterGroupCanvas != null) characterGroupCanvas.alpha = 0;
        if (handPointer != null) handPointer.gameObject.SetActive(false);
        if (tutorialText != null) { tutorialText.text = ""; tutorialText.gameObject.SetActive(false); }
        if (skipButton != null) skipButton.gameObject.SetActive(false);
    }

    public void RestartTutorial()
    {
        PlayerPrefs.DeleteKey("TutorialCompleted");
        PlayerPrefs.Save();

        // Tutorial GameObject'i aktifleştir (kapalıysa)
        gameObject.SetActive(true);

        // Listener'lar zaten kayıtlı, tekrar ekleme
        StartTutorialFromScratch();
    }

    public void SkipTutorial()
    {
        CleanupAllUI();
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();
        gameObject.SetActive(false);
    }

    void EndTutorial()
    {
        CleanupAllUI();
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();
        Debug.Log("[TutorialSystem] Tutorial tamamlandi.");
        gameObject.SetActive(false);
    }

    // ─────────────────────────────────────────
    void AnimateHand(TutorialStep step)
    {
        float elapsed = Time.time - stepStartTime;
        float offset = Mathf.Sin(elapsed * handSpeed) * handMoveDistance;
        float x = step.handPosition.x + (step.animateHorizontal ? offset : 0);
        float y = step.handPosition.y + (step.animateVertical ? -offset : 0);
        handPointer.position = step.targetButton.position + new Vector3(x, y, 0);
    }

    IEnumerator TypeText(string fullText)
    {
        isTyping = true;
        tutorialText.text = fullText;
        tutorialText.maxVisibleCharacters = 0;
        yield return null;
        tutorialText.ForceMeshUpdate();
        int total = tutorialText.textInfo.characterCount;

        while (tutorialText.maxVisibleCharacters < total)
        {
            tutorialText.maxVisibleCharacters++;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    bool IsScreenPressed()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return true;
        return false;
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        float t = 0;
        cg.alpha = from;
        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        cg.alpha = to;
    }
}

[System.Serializable]
public class TutorialStep
{
    public RectTransform targetButton;

    [Header("Bu Stepin Yazilari (sirayla gosterilir)")]
    [TextArea(2, 6)]
    public List<string> texts = new List<string>();

    [Header("El Konumu (butona gore offset)")]
    public Vector2 handPosition = new Vector2(0f, 0f);

    [Header("El Animasyon Yonu")]
    public bool animateHorizontal = true;
    public bool animateVertical = true;
}