using UnityEngine;
using UnityEngine.UI;
using TMPro;  // TextMeshPro için gerekli

public class UIContentZoom : MonoBehaviour
{
    [Header("Target & Button")]
    public RectTransform target;       // Zoom yapýlacak Content
    public Button zoomButton;          // Zoom butonu
    public TMP_Text buttonText;        // Buton üzerindeki TMP yazý

    [Header("Zoom Levels & Labels")]
    public float[] zoomLevels = { 0.3f, 0.6f, 1f };      // Gerçek scale deðerleri
    public string[] zoomLabels = { "2x", "4x", "1x" };   // Butonda görünen yazýlar

    private int currentIndex = 0;

    void Start()
    {
        // Diziler uyumlu mu kontrol et
        if (zoomLabels.Length != zoomLevels.Length)
        {
            Debug.LogWarning("zoomLevels ve zoomLabels uzunluklarý eþit olmalý. Label dizisi otomatik ayarlanacak.");
            zoomLabels = new string[zoomLevels.Length];
            for (int i = 0; i < zoomLevels.Length; i++)
                zoomLabels[i] = zoomLevels[i].ToString("0.0") + "x";
        }

        SetZoom(currentIndex);

        if (zoomButton != null)
            zoomButton.onClick.AddListener(NextZoom);
    }

    public void NextZoom()
    {
        currentIndex = (currentIndex + 1) % zoomLevels.Length;
        SetZoom(currentIndex);
    }

    private void SetZoom(int index)
    {
        // Scale deðerini uygula
        if (target != null)
            target.localScale = new Vector3(zoomLevels[index], zoomLevels[index], 1);

        // Buton üzerindeki TMP yazýyý uygula
        if (buttonText != null)
            buttonText.text = zoomLabels[index];
    }
}
