using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using System.Collections.Generic;

public class UIZoomCustomStartPos : MonoBehaviour
{
    [Header("Gerekli Objeler")]
    public RectTransform target;     // Content objesi (artık üzerinde resim var)
    public RectTransform viewArea;   // Viewport (görünen alan)
    public Canvas parentCanvas;      // Canvas

    [Header("Buton ve Zoom Ayarları")]
    public Button zoomButton;
    public TMP_Text buttonText;

    // Zoom seviyeleri
    public float[] zoomSteps = { 1f, 1.5f, 2f, 3f };

    [Header("Hassasiyet")]
    public float dragSpeed = 1f;

    private int currentStepIndex = 0;
    private float currentZoom = 1f;

    // Başlangıç pozisyonu
    private Vector2 startPos;

    void OnEnable() { EnhancedTouchSupport.Enable(); }
    void OnDisable() { EnhancedTouchSupport.Disable(); }

    void Start()
    {
        if (target != null)
        {
            target.pivot = new Vector2(0.5f, 0.5f);
            startPos = target.anchoredPosition;
        }

        if (viewArea == null)
        {
            Debug.LogError("View Area (Viewport) atanmamış! Inspector'dan ata.");
        }

        if (parentCanvas == null)
            parentCanvas = GetComponentInParent<Canvas>();

        if (zoomButton != null)
        {
            zoomButton.onClick.RemoveAllListeners();
            zoomButton.onClick.AddListener(OnZoomClicked);
        }

        currentStepIndex = 0;
        SetZoom(zoomSteps[0]);
    }

    void Update()
    {
        if (target == null || viewArea == null) return;

        // 1x zoom seviyesinde kilitle
        if (currentStepIndex == 0)
        {
            if (target.anchoredPosition != startPos)
            {
                target.anchoredPosition = startPos;
            }
            return;
        }

        // Sürükleme
        float scaleFactor = parentCanvas != null ? parentCanvas.scaleFactor : 1f;

        // Mouse (Editör)
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Mouse.current.leftButton.isPressed && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            target.anchoredPosition += (delta / scaleFactor) * dragSpeed;
            ClampTargetPosition();
        }
#endif

        // Mobil (Touch)
#if UNITY_ANDROID || UNITY_IOS
        if (Touch.activeTouches.Count == 1)
        {
            Touch t = Touch.activeTouches[0];
            if (!IsPointerOverUI(t))
            {
                Vector2 delta = t.delta;
                target.anchoredPosition += (delta / scaleFactor) * dragSpeed;
                ClampTargetPosition();
            }
        }
#endif
    }

    public void OnZoomClicked()
    {
        currentStepIndex = (currentStepIndex + 1) % zoomSteps.Length;
        SetZoom(zoomSteps[currentStepIndex]);
    }

    private void SetZoom(float zoomValue)
    {
        currentZoom = zoomValue;
        target.localScale = new Vector3(currentZoom, currentZoom, 1f);

        if (buttonText != null)
            buttonText.text = $"{currentZoom:0.0}x";

        // 1x'e dönüşte başlangıç pozisyonuna git
        if (currentStepIndex == 0)
        {
            target.anchoredPosition = startPos;
        }
        else
        {
            ClampTargetPosition();
        }
    }

    private void ClampTargetPosition()
    {
        if (currentStepIndex == 0) return;

        // Content'in zoom yapılmış boyutu
        float contentWidth = target.rect.width * currentZoom;
        float contentHeight = target.rect.height * currentZoom;

        // Viewport'un boyutu
        float viewWidth = viewArea.rect.width;
        float viewHeight = viewArea.rect.height;

        // DEBUG - Değerleri görelim
        Debug.Log($"Content Size: {target.rect.width}x{target.rect.height}, Zoom: {currentZoom}");
        Debug.Log($"Zoomed Size: {contentWidth}x{contentHeight}");
        Debug.Log($"Viewport Size: {viewWidth}x{viewHeight}");

        Vector2 pos = target.anchoredPosition;

        // X ekseni sınırlaması
        if (contentWidth > viewWidth)
        {
            float maxOffset = (contentWidth - viewWidth) / 2f;
            Debug.Log($"X MaxOffset: {maxOffset}");
            pos.x = Mathf.Clamp(pos.x, -maxOffset, maxOffset);
        }
        else
        {
            pos.x = 0f;
        }

        // Y ekseni sınırlaması
        if (contentHeight > viewHeight)
        {
            float maxOffset = (contentHeight - viewHeight) / 2f;
            Debug.Log($"Y MaxOffset: {maxOffset}");
            pos.y = Mathf.Clamp(pos.y, -maxOffset, maxOffset);
        }
        else
        {
            pos.y = 0f;
        }

        target.anchoredPosition = pos;
    }

    private bool IsPointerOverUI(Touch touch)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = touch.screenPosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}