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
    public RectTransform target;     // Hareket edecek Büyük Resim
    public RectTransform viewArea;   // Sınırları belirleyen Panel
    public Canvas parentCanvas;      // Hassas sürükleme için Canvas

    [Header("Buton ve Zoom Ayarları")]
    public Button zoomButton;
    public TMP_Text buttonText;

    // Zoom seviyeleri
    public float[] zoomSteps = { 1f, 1.5f, 2f, 3f };

    [Header("Hassasiyet")]
    public float dragSpeed = 1f;

    private int currentStepIndex = 0;
    private float currentZoom = 1f;

    // Senin ayarladığın başlangıç pozisyonunu hafızada tutmak için:
    private Vector2 startPos;

    void OnEnable() { EnhancedTouchSupport.Enable(); }
    void OnDisable() { EnhancedTouchSupport.Disable(); }

    void Start()
    {
        // 1. Pivot Ayarları (Zoom'un düzgün çalışması için pivot 0.5 olmalı)
        // Ama Anchor pozisyonunu bozmuyoruz.
        if (target != null)
        {
            target.pivot = new Vector2(0.5f, 0.5f);

            // ÖNEMLİ: Senin ayarladığın pozisyonu hafızaya alıyoruz.
            startPos = target.anchoredPosition;
        }

        if (viewArea == null && target != null)
            viewArea = target.parent as RectTransform;

        if (parentCanvas == null)
            parentCanvas = GetComponentInParent<Canvas>();

        if (zoomButton != null)
        {
            zoomButton.onClick.RemoveAllListeners();
            zoomButton.onClick.AddListener(OnZoomClicked);
        }

        // Başlangıç Ayarları
        currentStepIndex = 0;
        SetZoom(zoomSteps[0]);
    }

    void Update()
    {
        if (target == null || viewArea == null) return;

        // --- 1. SERT KİLİT (HARD LOCK) ---
        // Eğer 1. seviyedeysek (1x), resim senin ayarladığın başlangıç noktasında (startPos) kalmalı.
        if (currentStepIndex == 0)
        {
            // Eğer milim kaymışsa senin ayarladığın konuma geri getir
            if (target.anchoredPosition != startPos)
            {
                target.anchoredPosition = startPos;
            }
            // Input okuma, burada bitir.
            return;
        }

        // --- 2. SÜRÜKLEME KODLARI (Zoom > 1x ise çalışır) ---
        float scaleFactor = parentCanvas != null ? parentCanvas.scaleFactor : 1f;

        // Mouse (Editör)
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Mouse.current.leftButton.isPressed && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            target.anchoredPosition += (delta / scaleFactor) * dragSpeed;
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
            }
        }
#endif

        // Sınırları Koru
        ClampTargetPosition();
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

        // Zoom 1x'e döndüyse, senin orijinal pozisyonuna geri dön
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
        // 1x ise hesaplama yapma (startPos'ta kalacak)
        if (currentStepIndex == 0) return;

        float contentWidth = target.rect.width * target.localScale.x;
        float contentHeight = target.rect.height * target.localScale.y;

        float viewWidth = viewArea.rect.width;
        float viewHeight = viewArea.rect.height;

        Vector2 pos = target.anchoredPosition;

        // X Sınırı
        if (contentWidth > viewWidth)
        {
            float limitX = (contentWidth - viewWidth) / 2f;
            // Orijinal pozisyonun ofsetini de hesaba katmak gerekebilir ama 
            // genellikle Clamp yaparken merkeze göre yapmak en sağlıklısıdır.
            // Eğer çok kenarda başlıyorsa burası hafif zıplama yapabilir, 
            // ama zoom yapınca genelde kullanıcı bunu fark etmez.
            pos.x = Mathf.Clamp(pos.x, -limitX, limitX);
        }
        // else: Eğer resim ekrandan küçükse serbest bırak veya startPos.x'e çek (Şimdilik serbest)

        // Y Sınırı
        if (contentHeight > viewHeight)
        {
            float limitY = (contentHeight - viewHeight) / 2f;
            pos.y = Mathf.Clamp(pos.y, -limitY, limitY);
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