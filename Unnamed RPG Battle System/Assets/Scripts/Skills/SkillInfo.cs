using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Skill Info", order = 1)]
public class SkillInfo : ScriptableObject
{
    public string skillName;
    public string description;

    public AttackType skillType;

    public ElementType element;

    public int mpCost;
    public int hpCostPercent;

    public int timeCost;

    public int hpRestore;
    public int mpRestore;

    public int skillPower;

    public int deviationPercent;

    public int hitRate;

    public int critRate;
}
