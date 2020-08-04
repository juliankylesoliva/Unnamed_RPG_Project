using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType {Physical, Magic, Healing, Status}
public enum ElementType {None, Bash, Slice, Explosion, Ranged, Fire, Water, Earth, Air}
public enum DamageType {Normal, Resist, Weakness, Critical}

public class DamageCalculator : MonoBehaviour
{
    public int calcDamage(CharacterInfo attacker, CharacterInfo target, int power, int deviation, AttackType type, ElementType elem)
    {
        float totalDamage = 0.0f;

        float ratio = 0.0f;
        float rngBase = ((float)(100 - deviation) / 100.0f);
        float rngMod = Random.Range(rngBase, 1.00f);

        switch(type)
        {
            case AttackType.Physical:
                switch(elem)
                {
                    case ElementType.Slice:
                        ratio = (attacker.Attack * getBuffModMultiplier(attacker.BuffModATK)) / (target.Defense * getBuffModMultiplier(target.BuffModDEF) * 0.9f);
                        break;
                    default:
                        ratio = (attacker.Attack * getBuffModMultiplier(attacker.BuffModATK)) / (target.Defense * getBuffModMultiplier(target.BuffModDEF));
                        break;
                }
                break;
            case AttackType.Magic:
                switch(elem)
                {
                    case ElementType.Earth:
                        ratio = (attacker.Magic * getBuffModMultiplier(attacker.BuffModMAG)) / (target.Defense * getBuffModMultiplier(target.BuffModDEF));
                        break;
                    default:
                        ratio = (attacker.Magic * getBuffModMultiplier(attacker.BuffModMAG)) / (target.Resistance * getBuffModMultiplier(target.BuffModRES));
                        break;
                }
                break;
            default:
                break;
        }

        totalDamage = (ratio * power);

        conditionalDmgMods(target, elem, ref totalDamage);

        totalDamage *= rngMod;

        return (int)totalDamage;
    }

    void conditionalDmgMods(CharacterInfo target, ElementType elem, ref float dmg)
    {
        if (target.containsStatus(StatusCondition.Soak))
        {
            switch (elem)
            {
                case ElementType.Fire:
                    dmg *= 0.8f;
                    break;
                case ElementType.Air:
                    dmg *= 0.8f;
                    break;
                default:
                    dmg *= 1.2f;
                    break;
            }
        }

        if (target.containsStatus(StatusCondition.Burn))
        {
            if (elem == ElementType.Explosion)
            {
                dmg *= 1.5f;
            }
        }

        if (target.IsGuarding)
        {
            dmg *= 0.5f;
        }
    }

    public int calcHealing(CharacterInfo healer, int power)
    {
        float ratio = (power / 25.0f) + 2.0f;

        float totalHealing = healer.Compassion * ratio;

        return (int)totalHealing;
    }

    public bool calcHit(CharacterInfo attacker, CharacterInfo target, int accuracy)
    {
        float ratio = (attacker.Precision * getBuffModMultiplier(attacker.BuffModPRC)) / (target.Speed * getBuffModMultiplier(target.BuffModSPD));
        int percentage = (int)(ratio * accuracy);
        int rng = Random.Range(1, 101);
        return rng <= percentage;
    }

    public bool calcCrit(CharacterInfo attacker, int crit)
    {
        int percentage = (attacker.Skill + crit) / 10;
        int rng = Random.Range(1, 101);
        return rng <= percentage;
    }

    public bool calcEndure(CharacterInfo playerData)
    {
        float ratio = playerData.Bravery / 10.0f;
        int percentage;

        if(playerData.IsGuarding)
        {
            percentage = (int)(ratio * 2);
        }
        else
        {
            percentage = (int)ratio;
        }

        int rng = Random.Range(1, 101);
        return rng <= percentage;
    }

    public float getBuffModMultiplier(int stage)
    {
        switch (stage)
        {
            case 1:
                return 1.20f;
            case -1:
                return 0.80f;
            default:
                return 1.00f;
        }
    }
}
