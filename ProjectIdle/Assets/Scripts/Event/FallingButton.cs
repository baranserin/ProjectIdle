using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FallingButton : MonoBehaviour
{
    private EventConfig config;
    private RectTransform rectTransform;
    private Button button;
    private Image buttonImage;

    public void Init(EventConfig eventConfig)
    {
        config = eventConfig;
        rectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        if (button != null)
            button.onClick.AddListener(OnClick);

        // Eğer ikon varsa butonun görselini ayarla
        if (buttonImage != null && config.icon != null)
        {
            buttonImage.sprite = config.icon;
        }

        // Belirtilen süre sonra otomatik yok et
        Destroy(gameObject, config.lifeTime);
    }

    void Update()
    {
        rectTransform.localPosition += Vector3.down * config.fallSpeed * Time.deltaTime;
    }

    void OnClick()
    {
        switch (config.eventType)
        {
            case EventType.InstantReward:
                IncomeManager.Instance.AddMoney(config.rewardAmount);
                Debug.Log($"💰 Falling Event: +{config.rewardAmount} money!");
                break;

            case EventType.TimedMultiplier:
                IncomeManager.Instance.StartCoroutine(
                    ApplyTimedMultiplier(config.multiplier, config.duration)
                );
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
