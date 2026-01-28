using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecorationIncome : MonoBehaviour
{
    [System.Serializable]
    public class DecorationEntry
    {
        [Header("Tanım")]
        public string itemName;
        public int itemCost;
        public float itemMultiplier = 1f;

        [Header("Tür Ayarı")]
        // 🔹 YENİ: Bu eşya bir duvar boyası mı yoksa normal eşya mı?
        public bool isWallSkin = false;

        [Header("Bağlantılar (Normal Eşya İçin)")]
        public GameObject targetObject;         // Masa, koltuk vs. (SetActive kullanılır)

        [Header("Bağlantılar (Duvar Boyası İçin)")]
        // 🔹 YENİ: Eğer isWallSkin işaretliyse burası çalışır
        public Image targetWallImage;           // Duvardaki Image bileşeni
        public Sprite skinSprite;               // Atanacak yeni renk/resim

        [Header("UI Kontrolleri")]
        public Button buyButton;
        public TextMeshProUGUI costText;
        public RectTransform itemNameRect;
        [HideInInspector] public Vector2 originalItemNamePos;

        [Header("Grup ve Önkoşul")]
        public int groupId;                     // Aynı gruptakilerden sadece biri seçili
        public DecorationEntry prerequisite;

        [Header("Satın Alındı Prefab (Opsiyonel)")]
        public GameObject purchasedPrefab;
        [HideInInspector] public GameObject spawnedPurchased;

        [Header("Ek Butonlar")]
        public Button deselectButton;

        [Header("Hedef Türler (ProductType)")]
        public bool applyToAllTypes = false;
        public List<ProductType> affectedTypes = new();

        [HideInInspector] public bool isPurchased = false;
        [HideInInspector] public bool isSelected = false;

        public void Initialize(System.Action<DecorationEntry> onBuy, System.Action<DecorationEntry> onDeselect)
        {
            // Başlangıçta objeyi kapat (Eğer normal eşyaysa)
            if (!isWallSkin && targetObject != null)
                targetObject.SetActive(false);

            if (costText != null)
                costText.text = itemCost.ToString() + " $";

            if (buyButton != null)
            {
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(() => { onBuy?.Invoke(this); });
            }

            if (deselectButton != null)
            {
                deselectButton.onClick.RemoveAllListeners();
                deselectButton.onClick.AddListener(() => { onDeselect?.Invoke(this); });
            }

            if (itemNameRect != null)
                originalItemNamePos = itemNameRect.anchoredPosition;
        }
    }

    [Header("Dekorasyonlar")]
    public List<DecorationEntry> decorations = new List<DecorationEntry>();

    [Header("Bağımlılıklar")]
    public IncomeManager incomeManager;

    [Header("UI - Popup")]
    public TextMeshProUGUI popupText;
    public float displayDuration = 2.0f;

    [Header("Otomatik UI Yenileme")]
    public bool autoRefresh = true;
    public float refreshInterval = 0.25f;

    private Coroutine popupRoutine;
    private float _nextRefresh;

    private void Start()
    {
        foreach (var deco in decorations)
            deco.Initialize(ApplyDecoration, DeselectDecoration);

        if (incomeManager != null)
            incomeManager.ResetDecorationMultipliers();

        LoadDecorations();
        UpdateDecorationButtons();
    }

    private void Update()
    {
        if (!autoRefresh) return;

        if (Time.unscaledTime >= _nextRefresh)
        {
            _nextRefresh = Time.unscaledTime + refreshInterval;
            UpdateDecorationButtons();
        }
    }

    private bool CanAfford(DecorationEntry d)
    {
        return incomeManager != null && incomeManager.totalMoney >= d.itemCost;
    }

    private bool PrereqOk(DecorationEntry d)
    {
        return d.prerequisite == null || d.prerequisite.isPurchased;
    }

    private IEnumerator ShowPopup(string msg)
    {
        if (popupText == null) yield break;
        popupText.text = msg;
        popupText.gameObject.SetActive(true);
        yield return new WaitForSeconds(displayDuration);
        popupText.gameObject.SetActive(false);
    }

    private void ApplyDecoration(DecorationEntry entry)
    {
        if (incomeManager == null) return;

        // --- SATIN ALMA İŞLEMİ ---
        if (!entry.isPurchased)
        {
            if (!PrereqOk(entry))
            {
                if (popupRoutine != null) StopCoroutine(popupRoutine);
                popupRoutine = StartCoroutine(ShowPopup("Önce gerekli öğeyi satın al!"));
                return;
            }

            if (!CanAfford(entry))
            {
                if (popupRoutine != null) StopCoroutine(popupRoutine);
                popupRoutine = StartCoroutine(ShowPopup("Yetersiz Para!"));
                return;
            }

            // Para düş
            incomeManager.totalMoney -= entry.itemCost;
            entry.isPurchased = true;

            // Çarpan Ekle
            incomeManager.AddDecorationMultiplier(entry.itemMultiplier, entry.affectedTypes, entry.applyToAllTypes);

            // Kaydet
            PlayerPrefs.SetInt("Decoration_" + entry.itemName, 1);
            PlayerPrefs.Save();

            TrySpawnPurchasedPrefab(entry);
        }

        // --- SEÇİM VE AKTİFLEŞTİRME İŞLEMİ ---

        // 1. Aynı gruptaki diğerlerini kapat
        foreach (var deco in decorations)
        {
            if (deco != entry && deco.groupId == entry.groupId)
            {
                deco.isSelected = false;

                // Normal eşyaysa kapat
                if (!deco.isWallSkin && deco.targetObject != null)
                    deco.targetObject.SetActive(false);
            }
        }

        // 2. Bu öğeyi seçili yap
        entry.isSelected = true;

        // 🔹 YENİ MANTIK: Duvar mı, Eşya mı?
        if (entry.isWallSkin)
        {
            // Duvar Boyası Mantığı: Image sprite'ını değiştir
            if (entry.targetWallImage != null && entry.skinSprite != null)
            {
                entry.targetWallImage.sprite = entry.skinSprite;
            }
        }
        else
        {
            // Normal Eşya Mantığı: Game Object'i aç
            if (entry.targetObject != null)
                entry.targetObject.SetActive(true);
        }

        // Seçimi Kaydet
        PlayerPrefs.SetString("SelectedDecoration_Group_" + entry.groupId, entry.itemName);
        PlayerPrefs.Save();

        UpdateDecorationButtons();
    }

    private void TrySpawnPurchasedPrefab(DecorationEntry entry)
    {
        if (entry.purchasedPrefab != null && entry.spawnedPurchased == null && entry.buyButton != null)
        {
            Transform parent = entry.buyButton.transform.parent;
            entry.spawnedPurchased = Instantiate(entry.purchasedPrefab, parent, false);
            entry.spawnedPurchased.transform.SetSiblingIndex(entry.buyButton.transform.GetSiblingIndex());
            entry.spawnedPurchased.transform.localScale = Vector3.one;
        }
    }

    private void UpdateDecorationButtons()
    {
        foreach (var deco in decorations)
        {
            if (deco.costText != null)
                deco.costText.text = deco.itemCost.ToString() + " $";

            if (!deco.isPurchased)
            {
                // SATIN ALINMAMIŞ DURUM
                if (deco.buyButton != null)
                {
                    deco.buyButton.gameObject.SetActive(true);
                    var label = deco.buyButton.GetComponentInChildren<TextMeshProUGUI>();
                    if (label != null) label.text = "BUY";

                    bool interact = PrereqOk(deco) && CanAfford(deco);
                    deco.buyButton.interactable = interact;

                    if (deco.costText != null)
                        deco.costText.color = CanAfford(deco) ? Color.white : Color.red;
                }

                if (deco.itemNameRect != null)
                    deco.itemNameRect.anchoredPosition = deco.originalItemNamePos;
            }
            else
            {
                // SATIN ALINMIŞ DURUM
                if (deco.buyButton != null)
                {
                    deco.buyButton.gameObject.SetActive(true);
                    var label = deco.buyButton.GetComponentInChildren<TextMeshProUGUI>();

                    if (deco.costText != null)
                        deco.costText.gameObject.SetActive(false); // Fiyatı gizle

                    // İsim yazısını kaydır
                    if (deco.itemNameRect != null)
                    {
                        Vector2 newPos = deco.originalItemNamePos;
                        newPos.y -= 35f;
                        deco.itemNameRect.anchoredPosition = newPos;
                    }

                    if (deco.isSelected)
                    {
                        if (label != null) label.text = "SELECTED";
                        deco.buyButton.interactable = false; // Zaten seçili, tıklanmasın
                    }
                    else
                    {
                        if (label != null) label.text = "SELECT";
                        deco.buyButton.interactable = true;
                    }
                }
            }

            if (deco.deselectButton != null)
                deco.deselectButton.gameObject.SetActive(deco.isSelected);
        }
    }

    public void LoadDecorations()
    {
        if (incomeManager != null)
            incomeManager.ResetDecorationMultipliers();

        // 1. Satın almaları yükle
        foreach (var deco in decorations)
        {
            int saved = PlayerPrefs.GetInt("Decoration_" + deco.itemName, 0);
            deco.isPurchased = (saved == 1);
            deco.isSelected = false;

            if (deco.isPurchased)
            {
                TrySpawnPurchasedPrefab(deco);
                if (incomeManager != null)
                    incomeManager.AddDecorationMultiplier(deco.itemMultiplier, deco.affectedTypes, deco.applyToAllTypes);
            }

            // Başlangıçta hepsini kapat (Duvar veya Obje)
            if (!deco.isWallSkin && deco.targetObject != null)
                deco.targetObject.SetActive(false);
        }

        // 2. Seçili olanları yükle ve uygula
        foreach (var deco in decorations)
        {
            if (deco.isPurchased)
            {
                string selectedName = PlayerPrefs.GetString("SelectedDecoration_Group_" + deco.groupId, "");

                if (selectedName == deco.itemName)
                {
                    deco.isSelected = true;

                    // 🔹 YENİ: Yüklerken de türüne göre davran
                    if (deco.isWallSkin)
                    {
                        if (deco.targetWallImage != null && deco.skinSprite != null)
                            deco.targetWallImage.sprite = deco.skinSprite;
                    }
                    else
                    {
                        if (deco.targetObject != null)
                            deco.targetObject.SetActive(true);
                    }
                }
            }
        }
        UpdateDecorationButtons();
    }

    private void DeselectDecoration(DecorationEntry entry)
    {
        if (!entry.isSelected) return;

        entry.isSelected = false;

        // Eşyaysa kapat
        if (!entry.isWallSkin && entry.targetObject != null)
            entry.targetObject.SetActive(false);

        // Duvar ise deselect genelde "Varsayılan duvara dön" anlamına gelir
        // İstersen buraya bir varsayılan duvar ataması ekleyebilirsin.

        PlayerPrefs.DeleteKey("SelectedDecoration_Group_" + entry.groupId);
        PlayerPrefs.Save();
        UpdateDecorationButtons();
    }

    // Reset fonksiyonunu da aynı mantıkla koruyoruz...
    public void ResetDecorations()
    {
        foreach (var deco in decorations)
        {
            PlayerPrefs.DeleteKey("Decoration_" + deco.itemName);
            PlayerPrefs.DeleteKey("SelectedDecoration_Group_" + deco.groupId);

            deco.isPurchased = false;
            deco.isSelected = false;

            // UI Reset
            if (deco.costText != null)
            {
                deco.costText.gameObject.SetActive(true);
                deco.costText.text = deco.itemCost.ToString() + " $";
            }
            if (deco.buyButton != null) deco.buyButton.gameObject.SetActive(true);

            // Prefab Reset
            if (deco.spawnedPurchased != null) { Destroy(deco.spawnedPurchased); deco.spawnedPurchased = null; }

            // Obje Reset
            if (!deco.isWallSkin && deco.targetObject != null)
                deco.targetObject.SetActive(false);
        }
        PlayerPrefs.Save();
        if (incomeManager != null) incomeManager.ResetDecorationMultipliers();
        UpdateDecorationButtons();
    }
}