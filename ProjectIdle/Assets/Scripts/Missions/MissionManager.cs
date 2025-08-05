using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    [Header("Tüm Görevler")]
    public List<MissionConfig> missions;

    void Update()
    {
        foreach (var mission in missions)
        {
            if (mission.completed)
                continue;

            bool allMet = true;

            foreach (var condition in mission.conditions)
            {
                if (!IsConditionMet(condition))
                {
                    allMet = false;
                    break;
                }
            }

            if (allMet)
            {
                CompleteMission(mission);
            }
        }
    }

    bool IsConditionMet(MissionCondition condition)
    {
        switch (condition.type)
        {
            case MissionType.ReachProductLevel:
                {
                    var product = IncomeManager.Instance.products
                        .Find(p => p.config == condition.targetProduct);
                    return product != null && product.level >= condition.targetLevel;
                }

            case MissionType.UnlockProduct:
                {
                    var product = IncomeManager.Instance.products
                        .Find(p => p.config == condition.targetProduct);
                    return product != null && product.uiObject != null && product.uiObject.activeSelf;
                }

            case MissionType.ReachTotalMoney:
                return IncomeManager.Instance.totalMoney >= condition.targetMoney;

            default:
                return false;
        }
    }


    void CompleteMission(MissionConfig mission)
    {
        mission.completed = true;
        Debug.Log($"✅ Görev tamamlandı: {mission.missionName}");

        // Ödül ver (örnek: elmas)
        int currentGems = PlayerPrefs.GetInt("Gems", 0);
        PlayerPrefs.SetInt("Gems", currentGems + mission.rewardGems);

        // TODO: UI bildirimi, animasyon, ses efekti vb.
    }
}
