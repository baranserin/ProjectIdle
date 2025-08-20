using UnityEngine;
using UnityEngine.UI;
using TMPro;  // TextMeshPro i�in gerekli

public class UIContentZoom : MonoBehaviour
{
    [Header("Target & Button")]
    public RectTransform target;       // Zoom yap�lacak Content
    public Button zoomButton;          // Zoom butonu
    public TMP_Text buttonText;        // Buton �zerindeki TMP yaz�

    [Header("Zoom Levels & Labels")]
    public float[] zoomLevels = { 0.3f, 0.6f, 1f };      // Ger�ek scale de�erleri
    public string[] zoomLabels = { "2x", "4x", "1x" };   // Butonda g�r�nen yaz�lar

    private int currentIndex = 0;

    void Start()
    {
        // Diziler uyumlu mu kontrol et
        if (zoomLabels.Length != zoomLevels.Length)
        {
            Debug.LogWarning("zoomLevels ve zoomLabels uzunluklar� e�it olmal�. Label dizisi otomatik ayarlanacak.");
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
        // Scale de�erini uygula
        if (target != null)
            target.localScale = new Vector3(zoomLevels[index], zoomLevels[index], 1);

        // Buton �zerindeki TMP yaz�y� uygula
        if (buttonText != null)
            buttonText.text = zoomLabels[index];
    }
}
