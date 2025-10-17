using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FallingButton : MonoBehaviour, IPointerClickHandler
{
    private EventConfig config;
    private RectTransform rectTransform;
    private Button button;
    private Image buttonImage;
    private RectTransform canvasRect;

    // 🔥 Config’ten gelen animasyon kareleri
    private Sprite[] frames;
    private int frameIndex = 0;
    private float frameTimer = 0f;
    public float framesPerSecond = 10f;
    private bool hasAnimation = false;

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

        // Animasyon kareleri config’ten al
        if (config.animationFrames != null && config.animationFrames.Length > 0)
        {
            frames = config.animationFrames;
            hasAnimation = true;
            buttonImage.sprite = frames[0];
        }
        else if (config != null && config.fallingButtonPrefab != null)
        {
            // fallback tek ikon
            buttonImage.sprite = config.icon;
        }

        // Spawn pozisyon
        frameIndex = 0;
    }

    void Update()
    {
        if (config == null || rectTransform == null) return;

        // Düşme
        rectTransform.localPosition += Vector3.down * config.fallSpeed * Time.deltaTime;

        // Canvas dışına çıktıysa yok et
        if (rectTransform.localPosition.y < -canvasRect.rect.height / 2f - rectTransform.rect.height)
        {
            Destroy(gameObject);
            return;
        }

        // Kare animasyonu
        if (hasAnimation && frames != null && frames.Length > 1)
        {
            frameTimer += Time.deltaTime;
            if (frameTimer >= 1f / framesPerSecond)
            {
                frameTimer = 0f;
                frameIndex = (frameIndex + 1) % frames.Length;
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
                IncomeManager.Instance.AddMoney(config.rewardAmount);
                break;

            case EventType.TimedMultiplier:
                IncomeManager.Instance.StartCoroutine(ApplyTimedMultiplier(config.multiplier, config.duration));
                break;
        }

        Destroy(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }

    private IEnumerator ApplyTimedMultiplier(float multiplier, float duration)
    {
        IncomeManager.Instance.temporaryMultiplier *= multiplier;
        yield return new WaitForSeconds(duration);
        IncomeManager.Instance.temporaryMultiplier /= multiplier;
    }
}
