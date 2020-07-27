using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharData : MonoBehaviour
{
    public int currentBattlePosition = -1;  // Used as an array index for data references
    public bool isStandby = false;
    public bool isGuarding = false;         // Guarding halves incoming damage, resets to false when this unit's turn comes again

    public Dictionary<StatusCondition, int> statuses;   // Current status conditions

    /* Battle Stats */
    public string charName = "Name";
    public int charLvl = 1;

    public int maxHP = 10;
    public int currentHP = 10;
    public int ATK = 2;
    public int DEF = 1;
    public int SPD = 3;

    public int maxMP = 5;
    public int currentMP = 5;
    public int MAG = 2;
    public int RES = 1;
    public int PRC = 4;

    public int BRV = 1;
    public int CHA = 1;
    public int COM = 1;
    public int SKL = 1;

    /* Stat Modifiers */
    public float ATKMod = 1.0f;
    public float MAGMod = 1.0f;

    public float DEFMod = 1.0f;
    public float RESMod = 1.0f;

    public float SPDMod = 1.0f;
    public float PRCMod = 1.0f;

    /* Skills */
    public string[] skillNames;         // Skill names to be used for lookup at the start
    public List<SkillScript> skillSet;  // Holds the references to the actual skills used

    /* Affinities */
    public List<ElementType> weaknesses;
    public List<ElementType> resistances;

    void Start()
    {
        statuses = new Dictionary<StatusCondition, int>();
    }

    // Returns if the damage was enough to defeat the unit
    public bool DoDamage(int dmg)
    {
        currentHP -= dmg;
        if(currentHP <= 0)
        {
            currentHP = 0;
            return true;
        }

        return false;
    }

    public void DoHealing(int heal)
    {
        currentHP += heal;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }

    public void DoRestoreMP(int mp)
    {
        currentMP += mp;
        if(currentMP > maxMP)
        {
            currentMP = maxMP;
        }
    }

    public bool CheckMP(int cost)
    {
        if(cost > currentMP)
        {
            return false;
        }

        return true;
    }

    public void SpendMP(int cost)
    {
        currentMP -= cost;

        if(currentMP < 0)
        {
            currentMP = 0;
        }
    }

    public int getHPPercent(int costPercent)
    {
        return (int)(maxHP * (costPercent / 100.0f));
    }

    public bool CheckHP(int costPercent)
    {
        if(getHPPercent(costPercent) > currentHP - 1)
        {
            return false;
        }

        return true;
    }

    public void SpendHP(int costPercent)
    {
        currentHP -= getHPPercent(costPercent);

        if(currentHP < 0)
        {
            currentHP = 0;
        }
    }
}
