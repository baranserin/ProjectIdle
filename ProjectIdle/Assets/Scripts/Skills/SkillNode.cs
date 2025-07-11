using System.Linq;

[System.Serializable]
public class SkillNode
{
    public SkillConfig config;
    public bool unlocked = false;

    public void Unlock(IncomeManager manager)
    {
        if (unlocked) return;

        unlocked = true;

        switch (config.effectType)
        {
            case SkillConfig.SkillEffectType.MultiplyIncome:
                manager.prestigeMultiplier *= config.effectValue;
                break;

            case SkillConfig.SkillEffectType.ReduceUpgradeCost:
                foreach (var p in manager.products)
                    p.config.baseUpgradeCost *= (1f - config.effectValue);
                break;
        }
    }

    public bool CanUnlock(SkillTree tree)
    {
        if (unlocked || tree.availablePoints < config.cost)
            return false;

        return config.prerequisites.All(pr => tree.skills
            .FirstOrDefault(s => s.config == pr)?.unlocked == true);
    }
}
