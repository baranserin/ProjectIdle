using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FallingButton : MonoBehaviour, IPointerClickHandler
{
    private EventConfig config;
    private RectTransform rectTransform;
    private Button button;
    private Image buttonImage;
    private RectTransform canvasRect;
    private bool initialized = false;
    private bool consumed = false; // birden fazla tıkı engelle

    public void Init(EventConfig eventConfig, RectTransform canvas)
    {
        config = eventConfig;
        canvasRect = canvas;

        rectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        if (button != null)
        {
            // Inspector’daki tüm dinleyicileri temizle, sadece bizimkini ekle
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(HandleClick);
        }

        if (buttonImage != null && config.icon != null)
            buttonImage.sprite = config.icon;

        initialized = true;
    }

    void Update()
    {
        if (!initialized || config == null || rectTransform == null) return;

        rectTransform.localPosition += Vector3.down * config.fallSpeed * Time.deltaTime;

        // Canvas dışına çıktıysa yok et
        if (rectTransform.localPosition.y < -canvasRect.rect.height / 2f - rectTransform.rect.height)
        {
            Destroy(GetRootGO());
        }
    }

    // Button.onClick
    private void HandleClick()
    {
        if (consumed) return;
        consumed = true;

        ApplyEffectAndDestroy();
    }

    // Emniyet: Button çalışmasa bile child’a tıklama yakalansın
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!consumed) HandleClick();
    }

    private void ApplyEffectAndDestroy()
    {
        // Etki ver
        switch (config.eventType)
        {
            case EventType.InstantReward:
                IncomeManager.Instance.AddMoney(config.rewardAmount);
                break;
            case EventType.TimedMultiplier:
                IncomeManager.Instance.StartCoroutine(ApplyTimedMultiplier(config.multiplier, config.duration));
                break;
        }

        // Tıklamayı kapat ve yok et
        if (button) button.interactable = false;
        if (buttonImage) buttonImage.raycastTarget = false;

        // Görünen kökü yok ettiğimizden emin ol
        Destroy(GetRootGO());
    }

    private GameObject GetRootGO()
    {
        // Script child’taysa görsel parent’ı da yok etmek için
        return transform.root != null ? transform.root.gameObject : gameObject;
    }

    private System.Collections.IEnumerator ApplyTimedMultiplier(float multiplier, float duration)
    {
        IncomeManager.Instance.temporaryMultiplier *= multiplier;
        yield return new WaitForSeconds(duration);
        IncomeManager.Instance.temporaryMultiplier /= multiplier;
    }
}
