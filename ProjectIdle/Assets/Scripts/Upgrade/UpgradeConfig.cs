using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeConfig", menuName = "Upgrade/UpgradeConfig")]


public class UpgradeConfig : ScriptableObject
{
    public string upgradeName;
    public Sprite icon;
}