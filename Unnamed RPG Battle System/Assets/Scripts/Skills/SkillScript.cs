using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillScript : MonoBehaviour
{
    public SkillInfo info;
    public BattleSystem system;
    public DamageCalculator calc;
    public StatusContainer currentStatuses;

    public abstract void PrepareSkill(); // Set an info message, change the camera, and change the targeting mode.

    public abstract IEnumerator DoSkill(CharacterInfo src, CharacterInfo dst); // Add custom behavior to this skill while using the below functions.

    public abstract IEnumerator DoMainActionUnit(CharacterInfo src, CharacterInfo dst); // Use this in case you want (i.e.) to loop the main body of a multi-target skill.

    /* COMMON FUNCTIONS */

    public void DoTimeCost()
    {
        system.meter.SubFromTimer(info.timeCost);
    }

    public void DoMPCost(CharacterInfo src)
    {
        src.spendMP(info.mpCost);
        if (src.UnitPosition < 4)
        {
            StartCoroutine(system.statusPanelRefs[src.UnitPosition].drainMPBar(src));
        }
    }

    public void DoHPCost(CharacterInfo src)
    {
        src.spendHP(info.hpCostPercent);
        if(src.UnitPosition < 4)
        {
            StartCoroutine(system.statusPanelRefs[src.UnitPosition].drainHPBar(src));
        }
        else
        {
            StartCoroutine(system.healthbarRefs[src.UnitPosition - 4].flashHUD(src));
        }
    }

    public IEnumerator DoChangeCamToPlayerTargets(CharacterInfo dst)
    {
        if (dst.UnitPosition <= 3)
        {
            system.ChangeToCamPosition(dst.UnitPosition);
        }
        yield return new WaitForSeconds(0.8f);
    }

    public IEnumerator DoChangeCamToAllPlayerTargets(CharacterInfo dst)
    {
        if (dst.UnitPosition <= 3)
        {
            system.ChangeToCamPosition(9);
        }
        yield return new WaitForSeconds(0.8f);
    }

    public IEnumerator DoSendInfoMessage(string text)
    {
        system.infoText.SetText(text);
        yield return new WaitForSeconds(0.75f);
    }

    public bool DoHitCheck(CharacterInfo src, CharacterInfo dst)
    {
        if (dst.IsGuarding || calc.calcHit(src, dst, info.hitRate))
        {
            return true;
        }
        else
        {
            system.infoText.SetText("The attack missed!");
            system.meter.SubFromTimer(info.timeCost);
        }

        return false;
    }

    public int DoDamageCalculation(CharacterInfo src, CharacterInfo dst)
    {
        return calc.calcDamage(src, dst, info.skillPower, info.deviationPercent, info.skillType, info.element);
    }

    public int DoHealingCalculation(CharacterInfo src)
    {
        return calc.calcHealing(src, info.skillPower);
    }

    public bool DoDamageMultiplication(CharacterInfo src, CharacterInfo dst, ref int dmg)
    {
        if (calc.calcCrit(src, info.critRate))
        {
            dmg *= 3;
            system.infoText.SetText("CRITICAL HIT!!!");
            if(system.meter.timeMeter.value > 0)
            {
                system.meter.AddToTimer(info.timeCost * 2);
            }
            DoHealthBarText(dst, "CRITICAL!!!", dmg);
            return true;
        }
        else if (dst.weakness.IndexOf(info.element) != -1)
        {
            dmg *= 2;
            system.infoText.SetText("Weakness!");
            if (system.meter.timeMeter.value > 0)
            {
                system.meter.AddToTimer(info.timeCost);
            }
            DoHealthBarText(dst, "WEAK!", dmg);
            return true;
        }
        else if (dst.resists.IndexOf(info.element) != -1)
        {
            dmg = (int)(dmg * 0.5f);
            system.infoText.SetText("Resisted...");
            system.meter.SubFromTimer(info.timeCost / 2);
            DoHealthBarText(dst, "RESIST", dmg);
            return true;
        }
        else
        {
            system.infoText.SetText("Hit!");
            DoHealthBarText(dst, "", dmg);
        }

        return false;
    }

    public void DoHealthBarText(CharacterInfo dst, string header, int num)
    {
        if(dst.UnitPosition > 3)
        {
            system.healthbarRefs[dst.UnitPosition - 4].setHeader(header);
            system.healthbarRefs[dst.UnitPosition - 4].setDamageNum(num);
        }
        else
        {
            system.playerHealthbarRefs[dst.UnitPosition].setHeader(header);
            system.playerHealthbarRefs[dst.UnitPosition].setDamageNum(num);
        }
    }

    public IEnumerator DoDealDamage(CharacterInfo dst, int dmg)
    {
        //system.infoText.SetText($"{dst.Name} took {dmg} damage!");
        bool isKnockedOut = dst.dealDamage(dmg);
        if (dst.UnitPosition < 4)
        {
            StartCoroutine(system.statusPanelRefs[dst.UnitPosition].drainHPBar(dst));
            StartCoroutine(system.playerHealthbarRefs[dst.UnitPosition].flashHUD(dst));
        }
        else
        {
            StartCoroutine(system.healthbarRefs[dst.UnitPosition - 4].flashHUD(dst));
        }

        yield return new WaitForSeconds(0.25f);

        if (isKnockedOut)
        {
            system.infoText.SetText($"{dst.Name} got knocked out!");

            if(dst.UnitPosition > 3)
            {
                GameObject.Destroy(system.enemyStations[dst.UnitPosition - 4].transform.GetChild(0).gameObject);
            }

            if(info.element == ElementType.Air && system.meter.timeMeter.value > 0)
            {
                system.meter.AddToTimer(info.timeCost * 5);
            }
            //yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(0.35f);
    }

    public CharacterInfo GetAdjacentTarget(CharacterInfo dst, int shift)
    {
        CharacterInfo retData = null;
        int startPos = dst.UnitPosition;

        switch(shift)
        {
            case -1:
                if(startPos > 0 && startPos <= 3)
                {
                    retData = system.playerPartyData[startPos + shift];
                }
                else if(startPos > 4 && startPos <= 8)
                {
                    retData = system.enemyPartyData[startPos - 4 + shift];
                }
                else { }
                break;
            case 1:
                if (startPos < 3 && startPos >= 0)
                {
                    retData = system.playerPartyData[startPos + shift];
                }
                else if (startPos < 8 && startPos >= 4)
                {
                    retData = system.enemyPartyData[startPos - 4 + shift];
                }
                else {  }
                break;
            default:
                break;
        }

        if(retData != null)
        {
            return retData;
        }

        return null;
    }

    public CharacterInfo GetRandomTarget(CharacterInfo dst)
    {
        CharacterInfo randomTarget = null;

        while (randomTarget == null)
        {
            if (dst.UnitPosition > 3)
            {
                if(system.areAllEnemiesDefeated())
                {
                    return null;
                }
                randomTarget = system.enemyPartyData[Random.Range(0, 5)];
            }
            else
            {
                if(system.areAllPlayersDefeated())
                {
                    return null;
                }
                randomTarget = system.playerPartyData[Random.Range(0, 4)];
            }
        }

        return randomTarget;
    }

    public IEnumerator DoHPHealing(CharacterInfo dst, int heal)
    {
        system.infoText.SetText($"{dst.Name} recovered {heal} HP!");

        dst.healDamage(heal);
        if (dst.UnitPosition < 4)
        {
            StartCoroutine(system.statusPanelRefs[dst.UnitPosition].fillHPBar(dst));
            StartCoroutine(system.playerHealthbarRefs[dst.UnitPosition].fillHealthbar(dst));
        }
        else
        {
            StartCoroutine(system.healthbarRefs[dst.UnitPosition - 4].fillHealthbar(dst));
        }

        yield return new WaitForSeconds(1.5f);
    }

    public IEnumerator DoMPHealing(CharacterInfo dst, int heal)
    {
        system.infoText.SetText($"{dst.Name} recovered {heal} MP!");

        dst.restoreMP(heal);
        if (dst.UnitPosition < 4)
        {
            StartCoroutine(system.statusPanelRefs[dst.UnitPosition].fillMPBar(dst));
        }

        yield return new WaitForSeconds(1.5f);
    }

    public IEnumerator DoLoopMainAction(CharacterInfo src, CharacterInfo dst)
    {
        if (dst.UnitPosition > 3)
        {
            foreach (CharacterInfo enemy in system.enemyPartyData)
            {
                if (enemy != null)
                {
                    yield return StartCoroutine(DoMainActionUnit(src, enemy));
                }
            }
        }
        else
        {
            foreach (CharacterInfo player in system.playerPartyData)
            {
                if (player != null)
                {
                    yield return StartCoroutine(DoMainActionUnit(src, player));
                }
            }
        }
    }

    public IEnumerator DoATKUp(CharacterInfo dst, int numTurns)
    {
        StatusScript statTemp = currentStatuses.LookForStatus(StatusCondition.ATK_Up);
        yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
    }

    public IEnumerator DoATKDown(CharacterInfo dst, int numTurns)
    {
        StatusScript statTemp = currentStatuses.LookForStatus(StatusCondition.ATK_Down);
        yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
    }

    public IEnumerator DoDEFUp(CharacterInfo dst, int numTurns)
    {
        StatusScript statTemp = currentStatuses.LookForStatus(StatusCondition.DEF_Up);
        yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
    }

    public IEnumerator DoDEFDown(CharacterInfo dst, int numTurns)
    {
        StatusScript statTemp = currentStatuses.LookForStatus(StatusCondition.DEF_Down);
        yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
    }

    public IEnumerator DoSPDUp(CharacterInfo dst, int numTurns)
    {
        StatusScript statTemp = currentStatuses.LookForStatus(StatusCondition.SPD_Up);
        yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
    }

    public IEnumerator DoSPDDown(CharacterInfo dst, int numTurns)
    {
        StatusScript statTemp = currentStatuses.LookForStatus(StatusCondition.SPD_Down);
        yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
    }

    public IEnumerator DoMAGUp(CharacterInfo dst, int numTurns)
    {
        StatusScript statTemp = currentStatuses.LookForStatus(StatusCondition.MAG_Up);
        yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
    }

    public IEnumerator DoMAGDown(CharacterInfo dst, int numTurns)
    {
        StatusScript statTemp = currentStatuses.LookForStatus(StatusCondition.MAG_Down);
        yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
    }

    public IEnumerator DoRESUp(CharacterInfo dst, int numTurns)
    {
        StatusScript statTemp = currentStatuses.LookForStatus(StatusCondition.RES_Up);
        yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
    }

    public IEnumerator DoRESDown(CharacterInfo dst, int numTurns)
    {
        StatusScript statTemp = currentStatuses.LookForStatus(StatusCondition.RES_Down);
        yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
    }

    public IEnumerator DoPRCUp(CharacterInfo dst, int numTurns)
    {
        StatusScript statTemp = currentStatuses.LookForStatus(StatusCondition.PRC_Up);
        yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
    }

    public IEnumerator DoPRCDown(CharacterInfo dst, int numTurns)
    {
        StatusScript statTemp = currentStatuses.LookForStatus(StatusCondition.PRC_Down);
        yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
    }

    public bool DoPercentCheck(int percent)
    {
        return Random.Range(1, 101) <= percent;
    }

    public IEnumerator AddBurn(CharacterInfo dst, int numTurns)
    {
        if (!dst.IsGuarding && dst.CurrentHP > 0 && !dst.containsStatus(StatusCondition.Soak))
        {
            StatusScript statTemp = currentStatuses.LookForStatus(StatusCondition.Burn);
            yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
        }
        yield return new WaitForSeconds(0.0f);
    }

    public IEnumerator RemoveBurn(CharacterInfo dst)
    {
        if (dst.containsStatus(StatusCondition.Burn) && dst.CurrentHP > 0)
        {
            StatusScript statTemp = currentStatuses.LookForStatus(StatusCondition.Burn);
            yield return StartCoroutine(statTemp.StatusCleared(dst));
        }
        yield return new WaitForSeconds(0.0f);
    }

    public IEnumerator AddSoak(CharacterInfo dst, int numTurns)
    {
        if(!dst.IsGuarding && dst.CurrentHP > 0)
        {
            StatusScript statTemp = currentStatuses.LookForStatus(StatusCondition.Soak);
            yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
        }
        yield return new WaitForSeconds(0.0f);
    }

    public IEnumerator RemoveSoak(CharacterInfo dst)
    {
        if(dst.containsStatus(StatusCondition.Soak) && dst.CurrentHP > 0)
        {
            StatusScript statTemp = currentStatuses.LookForStatus(StatusCondition.Soak);
            yield return StartCoroutine(statTemp.StatusCleared(dst));
        }
    }

    public IEnumerator AddPoison(CharacterInfo dst, int numTurns)
    {
        if (!dst.IsGuarding && dst.CurrentHP > 0)
        {
            StatusScript statTemp = currentStatuses.LookForStatus(StatusCondition.Poison);
            yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
        }
        yield return new WaitForSeconds(0.0f);
    }
}
