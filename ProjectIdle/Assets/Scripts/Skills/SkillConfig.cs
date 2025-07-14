using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill/SkillConfig")]
public class SkillConfig : ScriptableObject
{
    public string skillName;
    [TextArea] public string description;
    public int cost = 1;
    public List<SkillConfig> prerequisites;
    public Sprite icon;

    public enum SkillEffectType { MultiplyIncome, ReduceUpgradeCost }
    public SkillEffectType effectType;
    public float effectValue = 1f;
}
