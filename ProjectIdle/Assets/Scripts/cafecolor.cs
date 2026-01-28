using UnityEngine;
using UnityEngine.UI;

public class WallSkinItem : MonoBehaviour
{
    [Header("Ayarlar")]
    public Sprite skinSprite;      // Satılacak duvar resmi
    public TMPro.TMP_Text buttonText;       // Butonun içindeki Text (Yazı) bileşeni

    // Tüm butonların birbiriyle konuşmasını sağlayan statik olay
    // (Biri seçilince diğerleri bunu duyar)
    public static System.Action<string> OnSkinChanged;

    private Image targetWallImage;
    private Button myButton;
    private const string SAVE_KEY = "SelectedSkin";

    void Start()
    {
        myButton = GetComponent<Button>();

        // 1. Duvarı Bul
        GameObject wallObj = GameObject.FindGameObjectWithTag("Wall");
        if (wallObj != null)
        {
            targetWallImage = wallObj.GetComponent<Image>();
        }

        // 2. Tıklama olayını ekle
        myButton.onClick.AddListener(BuyAndEquip);

        // 3. Haberleşme sistemine abone ol
        // "Birisi deri değiştirdiğinde bana haber ver" diyoruz.
        OnSkinChanged += UpdateUI;

        // 4. Oyun açıldığında durumunu kontrol et
        string savedName = PlayerPrefs.GetString(SAVE_KEY, "");
        UpdateUI(savedName);

        // Eğer açılışta kayıtlı olan bendeysem, duvarı boyayayım
        if (savedName == skinSprite.name && targetWallImage != null)
        {
            targetWallImage.sprite = skinSprite;
        }
    }

    // Obje yok olunca abonelikten çık (Hata almamak için şart)
    void OnDestroy()
    {
        OnSkinChanged -= UpdateUI;
    }

    void BuyAndEquip()
    {
        if (targetWallImage == null) return;

        // Duvarı değiştir
        targetWallImage.sprite = skinSprite;

        // Kaydet
        PlayerPrefs.SetString(SAVE_KEY, skinSprite.name);
        PlayerPrefs.Save();

        // HERKESE HABER VER: "Şu isimli deri seçildi!"
        OnSkinChanged?.Invoke(skinSprite.name);
    }

    // Bu fonksiyon her tetiklendiğinde buton kendi durumuna bakar
    void UpdateUI(string activeSkinName)
    {
        if (skinSprite.name == activeSkinName)
        {
            // Eğer seçilen ben isem:
            buttonText.text = "Selected";
            myButton.interactable = false; // Seçili olana tekrar basılmasın
        }
        else
        {
            // Eğer seçilen ben değilsem:
            buttonText.text = "Select";
            myButton.interactable = true;
        }
    }
}