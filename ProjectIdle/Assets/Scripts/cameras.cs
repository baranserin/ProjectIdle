using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class UIContentZoom : MonoBehaviour
{
    [Header("Target & Button")]
    public RectTransform target;
    public Button zoomButton;
    public TMP_Text buttonText;

    [Header("Zoom Levels & Labels")]
    public float[] zoomLevels = { 0.3f, 0.6f, 1f };      // Buton zoom değerleri
    public string[] zoomLabels = { "2x", "4x", "1x" };   // Buton üzerindeki yazılar

    [Header("Manual Zoom Settings")]
    public float minZoom = 0.3f;     // Artık daha uzaklaşabilirsin
    public float maxZoom = 2f;       // Yakınlaşma sınırı
    [Range(0.001f, 0.02f)]
    public float zoomSpeed = 0.001f; // Pinch hassasiyeti (düşük = daha yavaş)

    private int currentIndex = 0;
    private float currentZoom;
    private float lastPinchDist = -1f;

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Start()
    {
        // Eğer label sayısı zoomLevels ile eşit değilse otomatik oluştur
        if (zoomLabels.Length != zoomLevels.Length)
        {
            zoomLabels = new string[zoomLevels.Length];
            for (int i = 0; i < zoomLevels.Length; i++)
                zoomLabels[i] = zoomLevels[i].ToString("0.0") + "x";
        }

        // Başlangıçta currentZoom = buton değerinde
        currentZoom = zoomLevels[currentIndex];
        ApplyZoom(currentZoom);

        if (buttonText != null)
            buttonText.text = zoomLabels[currentIndex];

        if (zoomButton != null)
            zoomButton.onClick.AddListener(NextZoom);
    }

    void Update()
    {
        if (target == null) return;

#if UNITY_EDITOR || UNITY_STANDALONE
        // PC test için mouse scroll
        if (Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) > 0.03f)
            {
                currentZoom += scroll * zoomSpeed * 20f;
                currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
                ApplyZoom(currentZoom);
                if (buttonText != null)
                    buttonText.text = currentZoom.ToString("0.0") + "x";
            }
        }
#endif

#if UNITY_ANDROID || UNITY_IOS
        // Mobil pinch zoom (sadece iki parmak)
        if (Touch.activeTouches.Count == 2)
        {
            Vector2 t0 = Touch.activeTouches[0].screenPosition;
            Vector2 t1 = Touch.activeTouches[1].screenPosition;

            float dist = Vector2.Distance(t0, t1);

            if (lastPinchDist < 0f)
            {
                // İlk frame, sadece mesafeyi kaydet
                lastPinchDist = dist;
            }
            else
            {
                // Pinch delta hesapla
                float delta = dist - lastPinchDist;

                currentZoom += delta * zoomSpeed;
                currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
                ApplyZoom(currentZoom);

                if (buttonText != null)
                    buttonText.text = currentZoom.ToString("0.0") + "x";

                lastPinchDist = dist;
            }
        }
        else
        {
            lastPinchDist = -1f;
        }
#endif
    }

    public void NextZoom()
    {
        currentIndex = (currentIndex + 1) % zoomLevels.Length;
        currentZoom = zoomLevels[currentIndex];
        SetZoom(currentIndex);
    }

    private void SetZoom(int index)
    {
        currentZoom = zoomLevels[index];
        ApplyZoom(currentZoom);

        if (buttonText != null)
            buttonText.text = zoomLabels[index];
    }

    private void ApplyZoom(float zoom)
    {
        if (target != null)
            target.localScale = new Vector3(zoom, zoom, 1);
    }
}
