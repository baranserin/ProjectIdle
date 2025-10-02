using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
    public IncomeManager IncomeManager;

    [Header("Event Ayarları")]
    public EventConfig[] possibleEvents;
    public float eventInterval = 5f;
    private bool isEventActive = false;

    [Header("UI (Opsiyonel)")]
    public GameObject eventPanel;
    public TextMeshProUGUI eventTitle;
    public TextMeshProUGUI eventDescription;
    public Image eventIcon;

    [Header("Bağlantılar")]
    public Canvas targetCanvas;   // Inspector’dan ata (önerilir)

    private void Awake() => Instance = this;

    private void Start()
    {
        if (eventPanel != null) eventPanel.SetActive(false);

        if (targetCanvas == null)
            targetCanvas = FindFirstObjectByType<Canvas>();

        if (targetCanvas == null)
        {
            Debug.LogError("❌ Canvas bulunamadı! Lütfen EventManager > targetCanvas bağla.");
            enabled = false;
            return;
        }

        if (possibleEvents == null || possibleEvents.Length == 0)
        {
            Debug.LogError("❌ possibleEvents boş!");
            enabled = false;
            return;
        }

        InvokeRepeating(nameof(TryTriggerEvent), eventInterval, eventInterval);
    }

    private void TryTriggerEvent()
    {
        if (isEventActive || possibleEvents.Length == 0) return;

        EventConfig config = possibleEvents[Random.Range(0, possibleEvents.Length)];
        TriggerEvent(config);
    }

    private void TriggerEvent(EventConfig config)
    {
        isEventActive = true;

        if (eventPanel != null)
        {
            eventPanel.SetActive(true);
            if (eventTitle != null) eventTitle.text = config.eventName;
            if (eventDescription != null) eventDescription.text = config.description;
            if (eventIcon != null) eventIcon.sprite = config.icon;
        }

        ApplyEvent(config);

        if (eventPanel != null)
            StartCoroutine(HideEventPanelAfterDelay(2f));
    }

    private void ApplyEvent(EventConfig config)
    {
        SpawnFallingButton(config);
        // Event, buton ömrü bitince yeniden aktifleşsin
        StartCoroutine(ResetEventActive(eventInterval));

    }

    private IEnumerator ResetEventActive(float delay)
    {
        yield return new WaitForSeconds(delay);
        isEventActive = false;
    }

    private void SpawnFallingButton(EventConfig config)
    {
        RectTransform canvasRT = targetCanvas.GetComponent<RectTransform>();

        GameObject btnObj = Instantiate(config.fallingButtonPrefab, canvasRT);
        RectTransform rt = btnObj.GetComponent<RectTransform>();

        float randomX = Random.Range(-canvasRT.rect.width / 2f, canvasRT.rect.width / 2f);
        float spawnY = canvasRT.rect.height / 2f + 100f; // ekran üstünden spawn

        rt.localPosition = new Vector3(randomX, spawnY, 0);

        FallingButton fb = btnObj.GetComponent<FallingButton>();
        if (fb != null)
        {
            fb.Init(config, canvasRT);
        }
    }


    private IEnumerator HideEventPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (eventPanel != null) eventPanel.SetActive(false);
    }
}
