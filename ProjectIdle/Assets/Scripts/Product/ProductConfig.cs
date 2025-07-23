using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProductConfig", menuName = "Product/ProductConfig")]


public class ProductConfig : ScriptableObject
{
    public string productName;
    public float baseMultiplier = 1f;
    public float baseUpgradeCost = 10f;
    public float multiplierGrowth = 1.1f;
    public float baseIncome = 1f;
    public float incomeGrowth = 1f;
    public float costGrowth = 1.5f;
    public int baseLevel = 1;
    public Sprite icon;

    [Header("Unlock Condition")]
    public bool isLockedInitially = false;
    public List<UnlockCondition> unlockConditions = new List<UnlockCondition>();

}