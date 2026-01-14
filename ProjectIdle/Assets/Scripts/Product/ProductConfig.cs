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
    public float costSineFrequency = 0.5f;
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

    [Header("Level Boosts (çarpanlar)")]
    public LevelBoost[] levelBoosts = new LevelBoost[10]; // 👈 Inspector’dan doldurulacak
}



public enum ProductType
{
    Tea,
    Coffee,
    Dessert,
    Barista
}
