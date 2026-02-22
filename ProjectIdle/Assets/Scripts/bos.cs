using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MenuFadeController : MonoBehaviour
{
    [Header("Menü")]
    public CanvasGroup menuGroup;
    public List<Button> fadeButtons;
    public float fadeDuration = 1f;
    public float waitTime = 1f;

    [Header("Satın Alma Animasyonu")]
    public Animator purchaseAnimator;
    public string animationStateName = "PurchaseAnim";
    public float autoHideDelay = 1.5f;

    [System.Serializable]
    public class ItemEntry
    {
        public string itemName;
        public Button buyButton;
        public RectTransform itemRect;
    }
    public List<ItemEntry> items = new List<ItemEntry>();

    private bool isFading = false;
    private Coroutine _hideRoutine;
    private RectTransform _animRect;
    private Vector2 _targetAnchoredPos;
    private bool _lockPosition = false;

    private void Awake()
    {
        if (purchaseAnimator != null)
        {
            _animRect = purchaseAnimator.GetComponent<RectTransform>();
            purchaseAnimator.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        // Animator pozisyonu ezerse her frame zorla geri yaz
        if (_lockPosition && _animRect != null)
            _animRect.anchoredPosition = _targetAnchoredPos;
    }

    private IEnumerator Start()
    {
        yield return null;

        foreach (Button button in fadeButtons)
        {
            if (button != null)
            {
                Button captured = button;
                captured.onClick.AddListener(() => StartFade(null));
            }
        }

        foreach (ItemEntry item in items)
        {
            if (item.buyButton != null)
            {
                ItemEntry captured = item;
                captured.buyButton.onClick.AddListener(() => StartFade(captured.itemRect));
            }
        }
    }

    private void StartFade(RectTransform originRect)
    {
        if (!isFading)
        {
            if (originRect != null)
                PlayPurchaseAnimation(originRect);

            StartCoroutine(FadeMenuCoroutine());
        }
    }

    // ─── SATIN ALMA ANİMASYONU ───────────────────────────────────────────────

    private void PlayPurchaseAnimation(RectTransform originRect)
    {
        if (purchaseAnimator == null) return;

        if (_hideRoutine != null)
            StopCoroutine(_hideRoutine);

        _hideRoutine = StartCoroutine(PlayAnimAtPosition(originRect));
    }

    private IEnumerator PlayAnimAtPosition(RectTransform originRect)
    {
        _lockPosition = false;

        purchaseAnimator.gameObject.SetActive(true);
        purchaseAnimator.Rebind();
        purchaseAnimator.Play(animationStateName, 0, 0f);

        yield return null; // 1 frame bekle

        // originRect'in dünya pozisyonunu Canvas (animRect'in parent'ı) local koordinatına çevir
        RectTransform canvasRect = _animRect.parent as RectTransform;
        Canvas rootCanvas = GetComponentInParent<Canvas>();
        if (rootCanvas != null && !rootCanvas.isRootCanvas)
            rootCanvas = rootCanvas.rootCanvas;

        Camera cam = (rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            ? rootCanvas.worldCamera
            : null;

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, originRect.position);

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, cam, out localPoint);

        _targetAnchoredPos = localPoint;
        _lockPosition = true; // LateUpdate her frame pozisyonu kilitler

        // Clip süresi kadar bekle
        yield return null;
        float clipLength = GetClipLength();
        float waitDuration = clipLength > 0f ? clipLength : autoHideDelay;
        yield return new WaitForSecondsRealtime(waitDuration);

        _lockPosition = false;
        purchaseAnimator.gameObject.SetActive(false);
    }

    private float GetClipLength()
    {
        if (purchaseAnimator == null) return 0f;
        AnimatorClipInfo[] clips = purchaseAnimator.GetCurrentAnimatorClipInfo(0);
        if (clips != null && clips.Length > 0)
            return clips[0].clip.length;
        return 0f;
    }

    // ─── MENÜ FADE ───────────────────────────────────────────────────────────

    private IEnumerator FadeMenuCoroutine()
    {
        isFading = true;

        for (float t = 0; t < 1f; t += Time.deltaTime / fadeDuration)
        {
            menuGroup.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        menuGroup.alpha = 0f;

        yield return new WaitForSeconds(waitTime);

        for (float t = 0; t < 1f; t += Time.deltaTime / fadeDuration)
        {
            menuGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
        menuGroup.alpha = 1f;

        isFading = false;
    }
}