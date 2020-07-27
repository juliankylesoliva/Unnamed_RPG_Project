using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : StatusScript
{
    public override IEnumerator InitializeStatus(CharData chr, int numTurns)
    {
        if (!chr.statuses.ContainsKey(statusName))
        {
            chr.statuses.Add(statusName, numTurns);
            system.infoText.SetText($"{chr.charName} got burned!");
        }
        else
        {
            chr.statuses[statusName] += numTurns;
            system.infoText.SetText("Extended burn duration!");
        }
        
        yield return new WaitForSeconds(0.75f);
    }

    public override IEnumerator DoStatus(CharData chr)
    {
        int burnDmg = (int)(chr.maxHP * Random.Range(0.05f , 0.15f));

        if(burnDmg > chr.currentHP)
        {
            burnDmg = chr.currentHP - 1;
        }

        //system.infoText.SetText($"{chr.charName}'s burn dealt {burnDmg} damage!");
        chr.DoDamage(burnDmg);
        if (chr.currentBattlePosition < 4)
        {
            system.playerHealthbarRefs[chr.currentBattlePosition].setHeader("BURNED!");
            system.playerHealthbarRefs[chr.currentBattlePosition].setDamageNum(burnDmg);
            StartCoroutine(system.statusPanelRefs[chr.currentBattlePosition].drainHPBar(chr));
            StartCoroutine(system.playerHealthbarRefs[chr.currentBattlePosition].drainHealthbar(chr));
        }
        else
        {
            system.healthbarRefs[chr.currentBattlePosition - 4].setHeader("BURNED!");
            system.healthbarRefs[chr.currentBattlePosition - 4].setDamageNum(burnDmg);
            StartCoroutine(system.healthbarRefs[chr.currentBattlePosition - 4].flashHUD(chr));
        }

        yield return new WaitForSeconds(0.75f);
    }

    public override IEnumerator StatusCleared(CharData chr)
    {
        chr.statuses.Remove(statusName);
        system.infoText.SetText($"{chr.charName}'s burn healed!");
        yield return new WaitForSeconds(0.75f);
    }
}
