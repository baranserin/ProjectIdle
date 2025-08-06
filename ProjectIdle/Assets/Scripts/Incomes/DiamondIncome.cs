using UnityEngine;
using TMPro;

public class DiamondIncome : MonoBehaviour
{
    public static DiamondIncome Instance;

    [Header("UI")]
    public TextMeshProUGUI diamondText;

    private int diamonds = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        LoadDiamonds();
        UpdateUI();
    }

    public void AddDiamonds(int amount)
    {
        diamonds += amount;
        SaveDiamonds();
        UpdateUI();
        Debug.Log($"💎 {amount} elmas eklendi. Toplam: {diamonds}");
    }

    public bool SpendDiamonds(int amount)
    {
        if (diamonds >= amount)
        {
            diamonds -= amount;
            SaveDiamonds();
            UpdateUI();
            Debug.Log($"💎 {amount} elmas harcandı. Kalan: {diamonds}");
            return true;
        }
        else
        {
            Debug.Log("❌ Yetersiz elmas.");
            return false;
        }
    }

    public void UpdateUI()
    {
        if (diamondText != null)
            diamondText.text = diamonds.ToString();
    }

    private void SaveDiamonds()
    {
        PlayerPrefs.SetInt("Diamonds", diamonds);
        PlayerPrefs.Save();
    }

    private void LoadDiamonds()
    {
        diamonds = PlayerPrefs.GetInt("Diamonds", 0);
    }

    public int GetDiamondCount()
    {
        return diamonds;
    }
}
