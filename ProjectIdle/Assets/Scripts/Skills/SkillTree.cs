using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SkillTree : MonoBehaviour
{
    public List<SkillNode> skills;
    public int availablePoints = 0;
    public IncomeManager incomeManager;

    public void UnlockSkill(SkillNode node)
    {
        if (node.CanUnlock(this))
        {
            availablePoints -= node.config.cost;
            node.Unlock(incomeManager);
            SaveSkills();
        }
    }

    public void SaveSkills()
    {
        foreach (var skill in skills)
        {
            PlayerPrefs.SetInt("Skill_" + skill.config.skillName, skill.unlocked ? 1 : 0);
        }
        PlayerPrefs.SetInt("SkillPoints", availablePoints);
        PlayerPrefs.Save();
    }

    public void LoadSkills()
    {
        foreach (var skill in skills)
        {
            skill.unlocked = PlayerPrefs.GetInt("Skill_" + skill.config.skillName, 0) == 1;
            if (skill.unlocked)
                skill.Unlock(incomeManager);
        }

        availablePoints = PlayerPrefs.GetInt("SkillPoints", 0);
    }
}
