using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class FallingButton : MonoBehaviour, IPointerClickHandler
{
    private EventConfig config;
    private RectTransform rectTransform;
    private Button button;
    private Image buttonImage;
    private RectTransform canvasRect;

    // --- Frame animasyon ---
    private Sprite[] frames;
    private int frameIndex = 0;
    private float frameTimer = 0f;
    [Header("Frame Animation")]
    public float framesPerSecond = 12f;      // 10–14 arası çok akıcı durur
    public bool loopAnimation = true;

    // --- Yumuşak düşüş & görsel efektler ---
    [Header("Motion")]
    public AnimationCurve fallEase = AnimationCurve.EaseInOut(0, 0, 1, 1); // 0→1 ease
    public float swayAmplitude = 2f;     // sağ-sol esneme
    public float swayFrequency = 0.8f;    // Hz
    public float pulseAmplitude = 0.06f;  // scale nefes (0.0–0.1)
    public float pulseFrequency = 2.0f;

    [Header("Fade")]
    public bool fadeNearExit = true;
    public float fadeOutPixels = 140f;    // alt tarafta şu kadar piksel kala şeffaflaş

    private float startY;
    private float endY;       // ekran altı + buffer
    private float startX;     // rastgele X sabit + sway eklenecek
    private float totalFallDistance;
    private float accumulatedFall; // Ease ile entegre etmek için

    private bool consumed = false;

    public void Init(EventConfig eventConfig, RectTransform canvas)
    {
        config = eventConfig;
        canvasRect = canvas;

        rectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }

        // Frame setini config’ten al
        if (config.animationFrames != null && config.animationFrames.Length > 0)
        {
            frames = config.animationFrames;
            buttonImage.sprite = frames[0];
        }
        else if (config.icon != null)
        {
            // tek kare fallback
            buttonImage.sprite = config.icon;
        }

        // Başlangıç konumu kayıt (EventManager spawn ederken X,Y veriyor)
        startX = rectTransform.localPosition.x;
        startY = rectTransform.localPosition.y;

        // Ekran altına düşeceği y
        endY = -canvasRect.rect.height / 2f - rectTransform.rect.height - 20f;
        totalFallDistance = Mathf.Abs(endY - startY);
        accumulatedFall = 0f;

        // Görsel kalite: yumuşak kenar
        if (buttonImage) buttonImage.preserveAspect = true;
    }

    void Update()
    {
        if (config == null || rectTransform == null) return;

        // --- Düşüşü ease ile hesapla ---
        // Her framede sabit piksel düşmek yerine; “ne kadar yol aldık?” üzerinden ease uyguluyoruz.
        float rawDelta = config.fallSpeed * Time.deltaTime; // piksel/frame
        accumulatedFall += rawDelta;

        float t = Mathf.Clamp01(accumulatedFall / Mathf.Max(1f, totalFallDistance));  // 0→1
        float easedT = fallEase.Evaluate(t);
        float currentY = Mathf.Lerp(startY, endY, easedT);

        // Sway (sağa-sola küçük salınım)
        float sway = Mathf.Sin(Time.time * Mathf.PI * 2f * swayFrequency) * swayAmplitude;

        // Nefes (scale pulse)
        float pulse = 1f + Mathf.Sin(Time.time * Mathf.PI * 2f * pulseFrequency) * pulseAmplitude;

        // Pozisyonu uygula (X + sway, easing Y)
        rectTransform.localPosition = new Vector3(startX + sway, currentY, 0f);
        rectTransform.localScale = new Vector3(pulse, pulse, 1f);

        // Fade-out (alt sınıra yaklaşırken)
        if (fadeNearExit && buttonImage != null)
        {
            float pixelsFromBottom = rectTransform.localPosition.y - endY; // 0’a yaklaştıkça alt sınıra iniyoruz
            float a = Mathf.InverseLerp(0, fadeOutPixels, pixelsFromBottom); // 0→1
            var c = buttonImage.color;
            c.a = a;
            buttonImage.color = c;
        }

        // Ekran dışına çıktıysa yok et
        if (rectTransform.localPosition.y <= endY)
        {
            Destroy(gameObject);
            return;
        }

        // --- Frame animasyonu (daha smooth) ---
        if (frames != null && frames.Length > 1 && framesPerSecond > 0f)
        {
            frameTimer += Time.deltaTime;
            float frameDur = 1f / framesPerSecond;

            while (frameTimer >= frameDur)
            {
                frameTimer -= frameDur;
                frameIndex = (frameIndex + 1) % frames.Length;
                if (!loopAnimation && frameIndex == frames.Length - 1)
                {
                    // loop kapalı ise son karede kal
                    frameIndex = frames.Length - 1;
                    break;
                }
                buttonImage.sprite = frames[frameIndex];
            }
        }
    }

    private void OnClick()
    {
        if (consumed) return;
        consumed = true;

        switch (config.eventType)
        {
            case EventType.InstantReward:
                {
                   
                    double reward = IncomeManager.Instance.income * 3;

                    IncomeManager.Instance.AddMoney(reward);

                    break;
                }

            case EventType.TimedMultiplier:
                IncomeManager.Instance.StartCoroutine(
                    ApplyTimedMultiplier(config.multiplier, config.duration)
                );
                break;
        }

        // Disable UI interaction
        if (button) button.interactable = false;
        if (buttonImage) buttonImage.raycastTarget = false;

        StartCoroutine(ClickPopAndDestroy());
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }

    private IEnumerator ClickPopAndDestroy()
    {
        // mini pop + hızlı fade
        float dur = 0.15f;
        float t = 0f;
        var startScale = rectTransform.localScale;
        var startColor = buttonImage ? buttonImage.color : Color.white;

        while (t < dur)
        {
            t += Time.deltaTime;
            float k = t / dur;
            float pop = 1f + 0.25f * Mathf.Sin(k * Mathf.PI); // hızlı açılıp kapanan pop
            rectTransform.localScale = startScale * pop;

            if (buttonImage)
            {
                var c = startColor;
                c.a = Mathf.Lerp(startColor.a, 0f, k);
                buttonImage.color = c;
            }
            yield return null;
        }

        Destroy(gameObject);
    }

    private IEnumerator ApplyTimedMultiplier(float multiplier, float duration)
    {
        IncomeManager.Instance.temporaryMultiplier *= multiplier;
        yield return new WaitForSeconds(duration);
        IncomeManager.Instance.temporaryMultiplier /= multiplier;
    }
}
