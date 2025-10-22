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

        [Header("Bağlantılar")]
        public GameObject targetObject;         // Sahnedeki obje (seçilince aktif)
        public Button buyButton;                // Satın alma / Seç butonu
        public TextMeshProUGUI costText;        // Fiyat yazısı

        [Header("Grup ve Önkoşul")]
        public int groupId;                     // Aynı gruptakilerden sadece biri seçili
        public DecorationEntry prerequisite;    // Önkoşul öğe (satın alınmış olmalı)

        [Header("Satın Alındı Prefab")]
        public GameObject purchasedPrefab;      // Inspector’dan atanacak
        [HideInInspector] public GameObject spawnedPurchased; // Oluşturulan prefab referansı

        [HideInInspector] public bool isPurchased = false; // Satın alındı mı?
        [HideInInspector] public bool isSelected = false;  // Seçili mi?

        public void Initialize(System.Action<DecorationEntry> onBuy)
        {
            if (targetObject != null)
                targetObject.SetActive(false);

            if (costText != null)
                costText.text = itemCost.ToString() + " $";

            if (buyButton != null)
            {
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(() =>
                {
                    onBuy?.Invoke(this);
                });
            }
        }
    }

    [Header("Dekorasyonlar")]
    public List<DecorationEntry> decorations = new List<DecorationEntry>();

    [Header("Bağımlılıklar")]
    public IncomeManager incomeManager;

    [Header("UI - Popup")]
    public TextMeshProUGUI popupText;          // Inspector'dan bağla
    public float displayDuration = 2.0f;       // Kaç saniye gözükecek

    [Header("Otomatik UI Yenileme")]
    public bool autoRefresh = true;            // Dilersen kapat
    public float refreshInterval = 0.25f;      // 4 kez/sn

    private Coroutine popupRoutine;
    private float _nextRefresh;

    private void Start()
    {
        // Başlat
        foreach (var deco in decorations)
            deco.Initialize(ApplyDecoration);

        LoadDecorations();          // Kaydedilmiş satın alma durumlarını yükle
        UpdateDecorationButtons();  // UI ilk durum
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

    // ---- Helpers ----
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

    // ---- Ana Akış ----
    private void ApplyDecoration(DecorationEntry entry)
    {
        if (incomeManager == null)
        {
            Debug.LogWarning("[DecorationIncome] IncomeManager bağlı değil.");
            return;
        }

        // Satın alma akışı
        if (!entry.isPurchased)
        {
            // Önkoşul
            if (!PrereqOk(entry))
            {
                // İsteğe bağlı: kullanıcıya bilgi ver
                if (popupRoutine != null) StopCoroutine(popupRoutine);
                popupRoutine = StartCoroutine(ShowPopup("Önce gerekli dekorasyonu satın al."));
                return;
            }

            // Para kontrolü
            if (!CanAfford(entry))
            {
                // İsteğe bağlı uyarı
                if (popupRoutine != null) StopCoroutine(popupRoutine);
                popupRoutine = StartCoroutine(ShowPopup("Yetersiz para!"));
                return;
            }

            // Satın al
            incomeManager.totalMoney -= entry.itemCost;
            entry.isPurchased = true;

            // Gelire etkisini uygula
            incomeManager.AddDecorationMultiplier(entry.itemMultiplier);

            // Kaydet
            PlayerPrefs.SetInt("Decoration_" + entry.itemName, 1);
            PlayerPrefs.Save();

            // Purchased prefab’ı oluştur
            TrySpawnPurchasedPrefab(entry);
        }

        // Seçim akışı: Aynı gruptaki diğerlerini deselect et
        foreach (var deco in decorations)
        {
            if (deco != entry && deco.groupId == entry.groupId)
            {
                deco.isSelected = false;
                if (deco.targetObject != null)
                    deco.targetObject.SetActive(false);
            }
        }

        // Bu dekorasyonu seç
        entry.isSelected = true;
        if (entry.targetObject != null)
            entry.targetObject.SetActive(true);

        // UI güncelle
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
            // Fiyat yazısı
            if (deco.costText != null)
                deco.costText.text = deco.itemCost.ToString() + " $";

            if (!deco.isPurchased)
            {
                if (deco.buyButton != null)
                {
                    deco.buyButton.gameObject.SetActive(true);

                    var label = deco.buyButton.GetComponentInChildren<TextMeshProUGUI>();
                    if (label != null) label.text = "BUY";

                    // Satın alınmamışsa: hem önkoşul sağlanmalı hem para yetmeli
                    bool interact = PrereqOk(deco) && CanAfford(deco);
                    deco.buyButton.interactable = interact;

                    // Görsel feedback: para yetmiyorsa fiyat yazısı kırmızı
                    if (deco.costText != null)
                        deco.costText.color = CanAfford(deco) ? Color.white : Color.red;
                }
            }
            else
            {
                if (deco.buyButton != null)
                {
                    deco.buyButton.gameObject.SetActive(true);
                    var label = deco.buyButton.GetComponentInChildren<TextMeshProUGUI>();

                    if (deco.isSelected)
                    {
                        if (label != null) label.text = "SELECTED";
                        deco.buyButton.interactable = false;
                    }
                    else
                    {
                        if (label != null) label.text = "SELECT";
                        deco.buyButton.interactable = true; // Seçmek ücretsiz
                    }

                    if (deco.costText != null)
                        deco.costText.color = Color.white;
                }
            }
        }
    }

    // ---- Persist ----
    public void LoadDecorations()
    {
        foreach (var deco in decorations)
        {
            int saved = PlayerPrefs.GetInt("Decoration_" + deco.itemName, 0);
            deco.isPurchased = (saved == 1);
            deco.isSelected = false;

            // Satın alınmışsa "Purchased" prefab'ını yeniden üret
            if (deco.isPurchased)
                TrySpawnPurchasedPrefab(deco);

            // Sahne objesini başlangıçta kapalı tut (seçim yapılınca açılır)
            if (deco.targetObject != null)
                deco.targetObject.SetActive(false);
        }

        UpdateDecorationButtons();
    }

    public void ResetDecorations()
    {
        foreach (var deco in decorations)
        {
            PlayerPrefs.DeleteKey("Decoration_" + deco.itemName);
            deco.isPurchased = false;
            deco.isSelected = false;

            if (deco.targetObject != null)
                deco.targetObject.SetActive(false);

            if (deco.buyButton != null)
                deco.buyButton.gameObject.SetActive(true);

            if (deco.spawnedPurchased != null)
            {
                Destroy(deco.spawnedPurchased);
                deco.spawnedPurchased = null;
            }

            if (deco.costText != null)
            {
                deco.costText.text = deco.itemCost.ToString() + " $";
                deco.costText.color = Color.white;
            }
        }
        PlayerPrefs.Save();

        UpdateDecorationButtons();
    }
}
