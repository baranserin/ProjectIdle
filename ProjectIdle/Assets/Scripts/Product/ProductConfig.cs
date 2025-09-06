using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProductConfig", menuName = "Product/ProductConfig")]
public class ProductConfig : ScriptableObject
{
    public string productName;
    public float baseUpgradeCost = 10f;
    public float costGrowth = 1.5f;
    public float baseIncome = 1f;
    public float incomeGrowth = 1f;
    public int baseLevel = 1;
    public Sprite icon;

    [Header("Category")]
    public ProductType productType;

    [Header("Unlock Condition")]
    public bool isLockedInitially = false;
    public List<UnlockCondition> unlockConditions = new List<UnlockCondition>();
}

public enum ProductType
{
    Tea,
    Coffee,
    Dessert
}
