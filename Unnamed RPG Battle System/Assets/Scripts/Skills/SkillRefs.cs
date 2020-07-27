using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRefs : MonoBehaviour
{
    public SkillScript[] listOfSkills;
    public Dictionary<string, SkillScript> skillDictionary;

    public void InitSkillDictionary()
    {
        if(skillDictionary != null)
        {
            return;
        }

        skillDictionary = new Dictionary<string, SkillScript>();

        foreach (SkillScript skl in listOfSkills)
        {
            skillDictionary.Add(skl.info.skillName, skl);
        }
    }

    public SkillScript LookForSkill(string name)
    {
        InitSkillDictionary();

        if(skillDictionary.ContainsKey(name))
        {
            return skillDictionary[name];
        }

        return null;
    }
}
