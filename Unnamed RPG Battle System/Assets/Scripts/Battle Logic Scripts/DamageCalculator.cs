using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType {Physical, Magic, Healing, Status}
public enum ElementType {None, Bash, Slice, Explosion, Ranged, Fire, Water, Earth, Air}
public enum DamageType {Normal, Resist, Weakness, Critical}

public class DamageCalculator : MonoBehaviour
{
    public int calcDamage(CharData attacker, CharData target, int power, int deviation, AttackType type, ElementType elem)
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
                        ratio = (attacker.ATK * attacker.ATKMod) / (target.DEF * target.DEFMod * 0.9f);
                        break;
                    default:
                        ratio = (attacker.ATK * attacker.ATKMod) / (target.DEF * target.DEFMod);
                        break;
                }
                break;
            case AttackType.Magic:
                switch(elem)
                {
                    case ElementType.Earth:
                        ratio = (attacker.MAG * attacker.MAGMod) / (target.DEF * target.DEFMod);
                        break;
                    default:
                        ratio = (attacker.MAG * attacker.MAGMod) / (target.RES * target.RESMod);
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

    void conditionalDmgMods(CharData target, ElementType elem, ref float dmg)
    {
        if (target.statuses.ContainsKey(StatusCondition.Soak))
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

        if (target.statuses.ContainsKey(StatusCondition.Burn))
        {
            if (elem == ElementType.Explosion)
            {
                dmg *= 1.5f;
            }
        }

        if (target.isGuarding)
        {
            dmg *= 0.5f;
        }
    }

    public int calcHealing(CharData healer, int power)
    {
        float ratio = (power / 25.0f) + 2.0f;

        float totalHealing = healer.COM * ratio;

        return (int)totalHealing;
    }

    public bool calcHit(CharData attacker, CharData target, int accuracy)
    {
        float ratio = (attacker.PRC * attacker.PRCMod) / (target.SPD * target.SPDMod);
        int percentage = (int)(ratio * accuracy);
        int rng = Random.Range(1, 101);
        return rng <= percentage;
    }

    public bool calcCrit(CharData attacker, int crit)
    {
        int percentage = (attacker.SKL + crit) / 10;
        int rng = Random.Range(1, 101);
        return rng <= percentage;
    }

    public bool calcEndure(CharData playerData)
    {
        float ratio = playerData.BRV / 10.0f;
        int percentage;

        if(playerData.isGuarding)
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
}
