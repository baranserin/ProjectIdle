using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[System.Serializable] // ✅ sadece bu yeterli
public class LevelBoost
{
    public int requiredLevel;      // Kaçıncı seviyede açılacak
    public float incomeMultiplier; // Çarpan değeri (ör: 2.0 = x2, 1.5 = x1.5)
}

[CreateAssetMenu(fileName = "ProductConfig", menuName = "Product/ProductConfig")]
public class ProductConfig : ScriptableObject
{
    public string productName;
    public float baseUpgradeCost = 10f;
    public float costGrowth = 1.5f;
    public float baseIncome = 1f;
    public float incomeGrowth = 1f;
    public float incomeSineAmplitude = 2.0f;
    public float incomeSineFrequency = 0.5f;
    public float costSineAmplitude = 2.0f;
    public float costSineFrequency = 0.3f;
    public double costSinePhase = Math.PI / 2;
    public int baseLevel = 1;
    public Sprite icon;

    [Header("Category")]
    public ProductType productType;


    [Header("Machine Requirement")]
    public bool requiresMachine = false;  // Bu ürün makineye bağlı mı?

    [Header("Unlock Condition")]
    public bool isLockedInitially = true;
    public List<UnlockCondition> unlockConditions = new List<UnlockCondition>();

    public static class GlobalLevelBoosts
    {
        // 11 adet değişken (seviye ve çarpan çifti)
        public static readonly Dictionary<int, float> BoostTable = new Dictionary<int, float>
    {
        { 10,  1.5f },
        { 25,  1.5f },
        { 50,  1.5f },
        { 100, 1.5f },
        { 200, 1.5f },
        { 300, 1.5f },
        { 400, 1.5f },
        { 500, 1.5f },
        { 600, 1.5f },
        { 800, 1.5f },
        { 1000, 1.5f }
    };
    }
}



public enum ProductType
{
    Tea,
    Coffee,
    Dessert,
    Barista
}
