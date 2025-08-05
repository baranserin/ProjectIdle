using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum MissionType
{
    ReachProductLevel,
    UnlockProduct,
    ReachTotalMoney
}

[System.Serializable]
public class MissionCondition
{
    public MissionType type;

    [Header("Bağlı Ürün")]
    public ProductConfig targetProduct;  // 👈 Doğrudan config referansı

    [Header("Hedefler")]
    public int targetLevel;
    public double targetMoney;
}

[CreateAssetMenu(fileName = "NewMission", menuName = "Missions/Mission")]
public class MissionConfig : ScriptableObject
{
    public string missionName;
    public List<MissionCondition> conditions = new List<MissionCondition>();
    public int rewardGems = 10;
    public bool completed = false;
}
