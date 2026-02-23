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

    // Çoklu buton takibi
    private HashSet<int> clickedButtonsInStep = new HashSet<int>();
    private int currentHandTargetIndex = 0;

    // ─────────────────────────────────────────
    void Start()
    {
        characterHiddenPos = characterGroup.anchoredPosition;
        RegisterListeners();

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

        // Her step'teki her butona listener ekle
        for (int i = 0; i < tutorialSteps.Count; i++)
        {
            int stepIdx = i;
            List<RectTransform> buttons = tutorialSteps[i].targetButtons;
            for (int j = 0; j < buttons.Count; j++)
            {
                int btnIdx = j;
                if (buttons[j] == null) continue;
                Button btn = buttons[j].GetComponent<Button>();
                if (btn != null)
                    btn.onClick.AddListener(() => OnTargetButtonClicked(stepIdx, btnIdx));
            }
        }
    }

    void StartTutorialFromScratch()
    {
        currentStepIndex = -1;
        currentTextIndex = -1;
        handAnimating = false;
        characterVisible = false;
        isTyping = false;
        clickedButtonsInStep.Clear();
        currentHandTargetIndex = 0;

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
    void OnTargetButtonClicked(int stepIndex, int buttonIndex)
    {
        if (stepIndex != currentStepIndex) return;
        if (characterVisible) return;

        // Aynı butona tekrar tıklamayı engelle
        if (clickedButtonsInStep.Contains(buttonIndex)) return;

        clickedButtonsInStep.Add(buttonIndex);

        TutorialStep step = tutorialSteps[currentStepIndex];

        // Tıklanmamış ilk butonu bul
        int nextUnclicked = -1;
        for (int i = 0; i < step.targetButtons.Count; i++)
        {
            if (!clickedButtonsInStep.Contains(i) && step.targetButtons[i] != null)
            {
                nextUnclicked = i;
                break;
            }
        }

        if (nextUnclicked == -1)
        {
            // Tüm butonlar tıklandı → sonraki adıma geç
            handAnimating = false;
            handPointer.gameObject.SetActive(false);

            int nextStep = currentStepIndex + 1;
            if (nextStep < tutorialSteps.Count)
                StartCoroutine(BeginStep(nextStep));
            else
                EndTutorial();
        }
        else
        {
            // El imlecini tıklanmamış sonraki butona taşı
            currentHandTargetIndex = nextUnclicked;
            stepStartTime = Time.time;

            bool show = ShouldShowHand(step, nextUnclicked);
            handPointer.gameObject.SetActive(show);
            if (show)
            {
                Vector2 offset = GetHandOffset(step, nextUnclicked);
                handPointer.position = step.targetButtons[nextUnclicked].position
                                       + new Vector3(offset.x, offset.y, 0);
            }
        }
    }

    // ─────────────────────────────────────────
    IEnumerator BeginStep(int stepIndex)
    {
        currentStepIndex = stepIndex;
        currentTextIndex = 0;
        clickedButtonsInStep.Clear();
        currentHandTargetIndex = 0;

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

        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            characterGroupCanvas.alpha = 1 - (t / fadeDuration);
            yield return null;
        }
        characterGroupCanvas.alpha = 0;

        yield return FadeCanvasGroup(darkPanel, 0.6f, 0, fadeDuration);

        darkPanel.gameObject.SetActive(false);
        characterGroup.gameObject.SetActive(false);
        characterGroup.anchoredPosition = characterHiddenPos;

        // İlk tıklanabilir butona el imlecini yerleştir
        TutorialStep step = tutorialSteps[currentStepIndex];
        currentHandTargetIndex = 0;

        if (step.targetButtons != null && step.targetButtons.Count > 0 && step.targetButtons[0] != null)
        {
            bool show = ShouldShowHand(step, 0);
            handPointer.gameObject.SetActive(show);
            if (show)
            {
                Vector2 offset = GetHandOffset(step, 0);
                handPointer.position = step.targetButtons[0].position + new Vector3(offset.x, offset.y, 0);
            }
            stepStartTime = Time.time;
            handAnimating = true;
        }
    }

    // ─────────────────────────────────────────
    /// <summary>
    /// Belirtilen buton için hand offset'ini döner.
    /// handPositions listesinde bu indeks varsa onu, yoksa genel handPosition'ı kullanır.
    /// </summary>
    Vector2 GetHandOffset(TutorialStep step, int btnIndex)
    {
        if (step.handPositions != null && btnIndex < step.handPositions.Count)
            return step.handPositions[btnIndex];
        return step.handPosition;
    }

    /// <summary>
    /// showHand listesinde bu indeks tanımlıysa ona bakar, tanımlı değilse varsayılan olarak true döner.
    /// </summary>
    bool ShouldShowHand(TutorialStep step, int btnIndex)
    {
        if (step.showHand != null && btnIndex < step.showHand.Count)
            return step.showHand[btnIndex];
        return true; // liste eksikse varsayılan: göster
    }

    void AnimateHand(TutorialStep step)
    {
        if (currentHandTargetIndex >= step.targetButtons.Count) return;
        RectTransform target = step.targetButtons[currentHandTargetIndex];
        if (target == null) return;

        // Bu buton için hand gizliyse animasyon yapma
        if (!ShouldShowHand(step, currentHandTargetIndex)) return;

        float elapsed = Time.time - stepStartTime;
        Vector2 offset = GetHandOffset(step, currentHandTargetIndex);
        float xExtra = step.animateHorizontal ? Mathf.Sin(elapsed * handSpeed) * handMoveDistance : 0f;
        float yExtra = step.animateVertical ? -Mathf.Sin(elapsed * handSpeed) * handMoveDistance : 0f;

        handPointer.position = target.position + new Vector3(offset.x + xExtra, offset.y + yExtra, 0);
    }

    // ─────────────────────────────────────────
    void CleanupAllUI()
    {
        StopAllCoroutines();
        handAnimating = false;
        characterVisible = false;
        isTyping = false;
        clickedButtonsInStep.Clear();

        if (darkPanel != null) { darkPanel.alpha = 0; darkPanel.gameObject.SetActive(false); }
        if (characterGroup != null) { characterGroup.gameObject.SetActive(false); characterGroup.anchoredPosition = characterHiddenPos; }
        if (characterGroupCanvas != null) characterGroupCanvas.alpha = 0;
        if (handPointer != null) handPointer.gameObject.SetActive(false);
        if (tutorialText != null) { tutorialText.text = ""; tutorialText.gameObject.SetActive(false); }
        if (skipButton != null) skipButton.gameObject.SetActive(false);
    }

    public void RestartTutorial()
    {
        PlayerPrefs.DeleteKey("TutorialCompleted");
        PlayerPrefs.Save();
        gameObject.SetActive(true);
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
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) return true;
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) return true;
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
    [Header("Hedef Butonlar (hepsi tıklanmalı)")]
    public List<RectTransform> targetButtons = new List<RectTransform>();

    [Header("Bu Stepin Yazilari (sirayla gosterilir)")]
    [TextArea(2, 6)]
    public List<string> texts = new List<string>();

    [Header("Genel El Konumu (butona gore offset) — tek buton varsa bu yeterli")]
    public Vector2 handPosition = new Vector2(0f, 0f);

    [Header("Her Butona Ozel El Konumu (opsiyonel, targetButtons sirasiyla eslesmeli)")]
    public List<Vector2> handPositions = new List<Vector2>();

    [Header("Her Buton Icin Hand Goster mi? (targetButtons sirasiyla eslesmeli)")]
    public List<bool> showHand = new List<bool>();

    [Header("El Animasyon Yonu")]
    public bool animateHorizontal = true;
    public bool animateVertical = true;
}