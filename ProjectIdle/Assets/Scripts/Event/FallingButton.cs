using UnityEngine;
using UnityEngine.UI;

public class FallingButton : MonoBehaviour
{
    private EventConfig config;
    private RectTransform rectTransform;
    private Button button;
    private Image buttonImage;
    private RectTransform canvasRect;

    private bool initialized = false;

    public void Init(EventConfig eventConfig, RectTransform canvas)
    {
        config = eventConfig;
        rectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        canvasRect = canvas;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }

        if (buttonImage != null && config.icon != null)
            buttonImage.sprite = config.icon;

        initialized = true;
    }

    void Update()
    {
        if (!initialized || config == null || rectTransform == null) return;

        // düşür
        rectTransform.localPosition += Vector3.down * config.fallSpeed * Time.deltaTime;

        // canvas dışına çıktı mı?
        if (rectTransform.localPosition.y < -canvasRect.rect.height / 2f - rectTransform.rect.height)
        {
            Destroy(gameObject);
        }
    }

    void OnClick()
    {
        if (config == null) return;

        switch (config.eventType)
        {
            case EventType.InstantReward:
                IncomeManager.Instance.AddMoney(config.rewardAmount);
                Debug.Log($"💰 Falling Event: +{config.rewardAmount} money!");
                break;

            case EventType.TimedMultiplier:
                IncomeManager.Instance.StartCoroutine(ApplyTimedMultiplier(config.multiplier, config.duration));
                Debug.Log($"⚡ Falling Event: {config.multiplier}x gelir başladı!");
                break;
        }

        Destroy(gameObject);
    }

    private System.Collections.IEnumerator ApplyTimedMultiplier(float multiplier, float duration)
    {
        IncomeManager.Instance.temporaryMultiplier *= multiplier;
        yield return new WaitForSeconds(duration);
        IncomeManager.Instance.temporaryMultiplier /= multiplier;
        Debug.Log("⏳ Multiplier süresi bitti.");
    }
}
