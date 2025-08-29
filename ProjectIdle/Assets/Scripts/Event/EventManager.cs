using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
    public IncomeManager IncomeManager;

    [Header("Event Ayarları")]
    public EventConfig[] possibleEvents; // Inspector’dan ekle
    public float eventInterval = 5f;    // Kaç saniyede bir event çıksın
    private bool isEventActive = false;

    [Header("UI")]
    public GameObject eventPanel;
    public TextMeshProUGUI eventTitle;
    public TextMeshProUGUI eventDescription;
    public Image eventIcon;
    public Button acceptButton;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (eventPanel != null)
            eventPanel.SetActive(false);

        InvokeRepeating(nameof(TryTriggerEvent), eventInterval, eventInterval);
    }

    private void TryTriggerEvent()
    {
        if (isEventActive || possibleEvents.Length == 0) return;

        // Rastgele bir event seç
        EventConfig config = possibleEvents[Random.Range(0, possibleEvents.Length)];
        ShowEvent(config);
    }

    private void ShowEvent(EventConfig config)
    {
        if (eventPanel == null) return;

        isEventActive = true;
        eventPanel.SetActive(true);

        if (eventTitle != null) eventTitle.text = config.eventName;
        if (eventDescription != null) eventDescription.text = config.description;
        if (eventIcon != null) eventIcon.sprite = config.icon;

        acceptButton.onClick.RemoveAllListeners();
        acceptButton.onClick.AddListener(() => AcceptEvent(config));
    }

    private void AcceptEvent(EventConfig config)
    {
        eventPanel.SetActive(false);
        isEventActive = false;

        if (config.isInstantReward)
        {
            IncomeManager.Instance.AddMoney(config.rewardAmount);
            Debug.Log($"💰 Event ödülü: +{config.rewardAmount}");
        }

        if (config.isTimedMultiplier)
        {
            StartCoroutine(ApplyTimedMultiplier(config.multiplier, config.duration));
        }
    }


    private IEnumerator ApplyTimedMultiplier(float multiplier, float duration)
    {
        Debug.Log($"⚡ {multiplier}x multiplier başladı ({duration} sn)");

        IncomeManager.Instance.temporaryMultiplier *= multiplier;
        yield return new WaitForSeconds(duration);
        IncomeManager.Instance.temporaryMultiplier /= multiplier;

        Debug.Log("⏳ Multiplier süresi bitti.");
    }
}
