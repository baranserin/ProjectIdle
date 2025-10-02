using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
    public IncomeManager IncomeManager;

    [Header("Event Ayarları")]
    public EventConfig[] possibleEvents;
    public float eventInterval = 5f;    // Kaç saniyede bir event çıksın
    private bool isEventActive = false;

    [Header("UI (Opsiyonel)")]
    public GameObject eventPanel;
    public TextMeshProUGUI eventTitle;
    public TextMeshProUGUI eventDescription;
    public Image eventIcon;

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
        TriggerEvent(config);
    }

    private void TriggerEvent(EventConfig config)
    {
        isEventActive = true;

        // Opsiyonel UI gösterme
        if (eventPanel != null)
        {
            eventPanel.SetActive(true);
            if (eventTitle != null) eventTitle.text = config.eventName;
            if (eventDescription != null) eventDescription.text = config.description;
            
        }

        // Direkt uygula
        ApplyEvent(config);

        // UI’yı kısa süre sonra kapatmak istersen:
        if (eventPanel != null)
            StartCoroutine(HideEventPanelAfterDelay(2f)); // 2 saniye sonra gizle
    }

    private void ApplyEvent(EventConfig config)
    {
        // Bütün eventler düşen buton üzerinden çalışacak
        SpawnFallingButton(config);

        isEventActive = false;
    }

    private void SpawnFallingButton(EventConfig config)
    {
        RectTransform canvas = FindFirstObjectByType<Canvas>().GetComponent<RectTransform>();

        GameObject btnObj = Instantiate(config.fallingButtonPrefab, canvas);
        RectTransform rt = btnObj.GetComponent<RectTransform>();

        // Rastgele X pozisyonu (canvas genişliği içinde)
        float randomX = Random.Range(-canvas.rect.width / 2f, canvas.rect.width / 2f);

        // Canvas’ın üst kenarının biraz üstünden spawn et (sanki dışarıdan geliyor gibi)
        float spawnY = canvas.rect.height / 2f + 100f;

        rt.localPosition = new Vector3(randomX, spawnY, 0);

        // FallingButton scriptini başlat
        FallingButton fb = btnObj.GetComponent<FallingButton>();
        if (fb != null)
        {
            fb.Init(config);
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

    private IEnumerator HideEventPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (eventPanel != null)
            eventPanel.SetActive(false);
    }
}
