using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillScript : MonoBehaviour
{
    public SkillInfo info;
    public BattleSystem system;
    public DamageCalculator calc;
    public StatusContainer statuses;

    public abstract void PrepareSkill(); // Set an info message, change the camera, and change the targeting mode.

    public abstract IEnumerator DoSkill(CharData src, CharData dst); // Add custom behavior to this skill while using the below functions.

    public abstract IEnumerator DoMainActionUnit(CharData src, CharData dst); // Use this in case you want (i.e.) to loop the main body of a multi-target skill.

    /* COMMON FUNCTIONS */

    public void DoTimeCost()
    {
        system.meter.SubFromTimer(info.timeCost);
    }

    public void DoMPCost(CharData src)
    {
        src.SpendMP(info.mpCost);
        if (src.currentBattlePosition < 4)
        {
            StartCoroutine(system.statusPanelRefs[src.currentBattlePosition].drainMPBar(src));
        }
    }

    public void DoHPCost(CharData src)
    {
        src.SpendHP(info.hpCostPercent);
        if(src.currentBattlePosition < 4)
        {
            StartCoroutine(system.statusPanelRefs[src.currentBattlePosition].drainHPBar(src));
        }
        else
        {
            StartCoroutine(system.healthbarRefs[src.currentBattlePosition - 4].flashHUD(src));
        }
    }

    public IEnumerator DoChangeCamToPlayerTargets(CharData dst)
    {
        if (dst.currentBattlePosition <= 3)
        {
            system.ChangeToCamPosition(dst.currentBattlePosition);
        }
        yield return new WaitForSeconds(0.8f);
    }

    public IEnumerator DoChangeCamToAllPlayerTargets(CharData dst)
    {
        if (dst.currentBattlePosition <= 3)
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

    public bool DoHitCheck(CharData src, CharData dst)
    {
        if (dst.isGuarding || calc.calcHit(src, dst, info.hitRate))
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

    public int DoDamageCalculation(CharData src, CharData dst)
    {
        return calc.calcDamage(src, dst, info.skillPower, info.deviationPercent, info.skillType, info.element);
    }

    public int DoHealingCalculation(CharData src)
    {
        return calc.calcHealing(src, info.skillPower);
    }

    public bool DoDamageMultiplication(CharData src, CharData dst, ref int dmg)
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
        else if (dst.weaknesses.IndexOf(info.element) != -1)
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
        else if (dst.resistances.IndexOf(info.element) != -1)
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

    public void DoHealthBarText(CharData dst, string header, int num)
    {
        if(dst.currentBattlePosition > 3)
        {
            system.healthbarRefs[dst.currentBattlePosition - 4].setHeader(header);
            system.healthbarRefs[dst.currentBattlePosition - 4].setDamageNum(num);
        }
        else
        {
            system.playerHealthbarRefs[dst.currentBattlePosition].setHeader(header);
            system.playerHealthbarRefs[dst.currentBattlePosition].setDamageNum(num);
        }
    }

    public IEnumerator DoDealDamage(CharData dst, int dmg)
    {
        //system.infoText.SetText($"{dst.charName} took {dmg} damage!");
        bool isKnockedOut = dst.DoDamage(dmg);
        if (dst.currentBattlePosition < 4)
        {
            StartCoroutine(system.statusPanelRefs[dst.currentBattlePosition].drainHPBar(dst));
            StartCoroutine(system.playerHealthbarRefs[dst.currentBattlePosition].flashHUD(dst));
        }
        else
        {
            StartCoroutine(system.healthbarRefs[dst.currentBattlePosition - 4].flashHUD(dst));
        }

        yield return new WaitForSeconds(0.25f);

        if (isKnockedOut)
        {
            system.infoText.SetText($"{dst.charName} got knocked out!");

            if(dst.currentBattlePosition > 3)
            {
                GameObject.Destroy(system.enemyStations[dst.currentBattlePosition - 4].transform.GetChild(0).gameObject);
            }

            if(info.element == ElementType.Air && system.meter.timeMeter.value > 0)
            {
                system.meter.AddToTimer(info.timeCost * 5);
            }
            //yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(0.35f);
    }

    public CharData GetAdjacentTarget(CharData dst, int shift)
    {
        CharData retData = null;
        int startPos = dst.currentBattlePosition;

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

    public CharData GetRandomTarget(CharData dst)
    {
        CharData randomTarget = null;

        while (randomTarget == null)
        {
            if (dst.currentBattlePosition > 3)
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

    public IEnumerator DoHPHealing(CharData dst, int heal)
    {
        system.infoText.SetText($"{dst.charName} recovered {heal} HP!");

        dst.DoHealing(heal);
        if (dst.currentBattlePosition < 4)
        {
            StartCoroutine(system.statusPanelRefs[dst.currentBattlePosition].fillHPBar(dst));
            StartCoroutine(system.playerHealthbarRefs[dst.currentBattlePosition].fillHealthbar(dst));
        }
        else
        {
            StartCoroutine(system.healthbarRefs[dst.currentBattlePosition - 4].fillHealthbar(dst));
        }

        yield return new WaitForSeconds(1.5f);
    }

    public IEnumerator DoMPHealing(CharData dst, int heal)
    {
        system.infoText.SetText($"{dst.charName} recovered {heal} MP!");

        dst.DoRestoreMP(heal);
        if (dst.currentBattlePosition < 4)
        {
            StartCoroutine(system.statusPanelRefs[dst.currentBattlePosition].fillMPBar(dst));
        }

        yield return new WaitForSeconds(1.5f);
    }

    public IEnumerator DoLoopMainAction(CharData src, CharData dst)
    {
        if (dst.currentBattlePosition > 3)
        {
            foreach (CharData enemy in system.enemyPartyData)
            {
                if (enemy != null)
                {
                    yield return StartCoroutine(DoMainActionUnit(src, enemy));
                }
            }
        }
        else
        {
            foreach (CharData player in system.playerPartyData)
            {
                if (player != null)
                {
                    yield return StartCoroutine(DoMainActionUnit(src, player));
                }
            }
        }
    }

    public IEnumerator DoATKUp(CharData dst, int numTurns)
    {
        StatusScript statTemp = statuses.LookForStatus(StatusCondition.ATK_Up);
        yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
    }

    public IEnumerator DoATKDown(CharData dst, int numTurns)
    {
        StatusScript statTemp = statuses.LookForStatus(StatusCondition.ATK_Down);
        yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
    }

    public IEnumerator DoDEFUp(CharData dst, int numTurns)
    {
        StatusScript statTemp = statuses.LookForStatus(StatusCondition.DEF_Up);
        yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
    }

    public IEnumerator DoDEFDown(CharData dst, int numTurns)
    {
        StatusScript statTemp = statuses.LookForStatus(StatusCondition.DEF_Down);
        yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
    }

    public IEnumerator DoSPDUp(CharData dst, int numTurns)
    {
        StatusScript statTemp = statuses.LookForStatus(StatusCondition.SPD_Up);
        yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
    }

    public IEnumerator DoSPDDown(CharData dst, int numTurns)
    {
        StatusScript statTemp = statuses.LookForStatus(StatusCondition.SPD_Down);
        yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
    }

    public IEnumerator DoMAGUp(CharData dst, int numTurns)
    {
        StatusScript statTemp = statuses.LookForStatus(StatusCondition.MAG_Up);
        yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
    }

    public IEnumerator DoMAGDown(CharData dst, int numTurns)
    {
        StatusScript statTemp = statuses.LookForStatus(StatusCondition.MAG_Down);
        yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
    }

    public IEnumerator DoRESUp(CharData dst, int numTurns)
    {
        StatusScript statTemp = statuses.LookForStatus(StatusCondition.RES_Up);
        yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
    }

    public IEnumerator DoRESDown(CharData dst, int numTurns)
    {
        StatusScript statTemp = statuses.LookForStatus(StatusCondition.RES_Down);
        yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
    }

    public IEnumerator DoPRCUp(CharData dst, int numTurns)
    {
        StatusScript statTemp = statuses.LookForStatus(StatusCondition.PRC_Up);
        yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
    }

    public IEnumerator DoPRCDown(CharData dst, int numTurns)
    {
        StatusScript statTemp = statuses.LookForStatus(StatusCondition.PRC_Down);
        yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
    }

    public bool DoPercentCheck(int percent)
    {
        return Random.Range(1, 101) <= percent;
    }

    public IEnumerator AddBurn(CharData dst, int numTurns)
    {
        if (!dst.isGuarding && dst.currentHP > 0 && !dst.statuses.ContainsKey(StatusCondition.Soak))
        {
            StatusScript statTemp = statuses.LookForStatus(StatusCondition.Burn);
            yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
        }
        yield return new WaitForSeconds(0.0f);
    }

    public IEnumerator RemoveBurn(CharData dst)
    {
        if (dst.statuses.ContainsKey(StatusCondition.Burn) && dst.currentHP > 0)
        {
            StatusScript statTemp = statuses.LookForStatus(StatusCondition.Burn);
            yield return StartCoroutine(statTemp.StatusCleared(dst));
        }
        yield return new WaitForSeconds(0.0f);
    }

    public IEnumerator AddSoak(CharData dst, int numTurns)
    {
        if(!dst.isGuarding && dst.currentHP > 0)
        {
            StatusScript statTemp = statuses.LookForStatus(StatusCondition.Soak);
            yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
        }
        yield return new WaitForSeconds(0.0f);
    }

    public IEnumerator RemoveSoak(CharData dst)
    {
        if(dst.statuses.ContainsKey(StatusCondition.Soak) && dst.currentHP > 0)
        {
            StatusScript statTemp = statuses.LookForStatus(StatusCondition.Soak);
            yield return StartCoroutine(statTemp.StatusCleared(dst));
        }
    }

    public IEnumerator AddPoison(CharData dst, int numTurns)
    {
        if (!dst.isGuarding && dst.currentHP > 0)
        {
            StatusScript statTemp = statuses.LookForStatus(StatusCondition.Poison);
            yield return StartCoroutine(statTemp.InitializeStatus(dst, numTurns));
        }
        yield return new WaitForSeconds(0.0f);
    }
}
