using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : StatusScript
{
    public override IEnumerator InitializeStatus(CharData chr, int numTurns)
    {
        if (!chr.statuses.ContainsKey(statusName))
        {
            chr.statuses.Add(statusName, numTurns);
            system.infoText.SetText($"{chr.charName} got poisoned!");
        }
        else
        {
            system.infoText.SetText($"{chr.charName} is already poisoned!");
        }
        yield return new WaitForSeconds(0.75f);
    }

    public override IEnumerator DoStatus(CharData chr)
    {
        int poisonDmg = (int)(chr.maxHP * 0.20);

        //system.infoText.SetText($"{chr.charName}'s burn dealt {burnDmg} damage!");
        bool isKnockedOut = chr.DoDamage(poisonDmg);
        if (chr.currentBattlePosition < 4)
        {
            system.playerHealthbarRefs[chr.currentBattlePosition].setHeader("POISONED!");
            system.playerHealthbarRefs[chr.currentBattlePosition].setDamageNum(poisonDmg);
            StartCoroutine(system.statusPanelRefs[chr.currentBattlePosition].drainHPBar(chr));
            StartCoroutine(system.playerHealthbarRefs[chr.currentBattlePosition].drainHealthbar(chr));
        }
        else
        {
            system.healthbarRefs[chr.currentBattlePosition - 4].setHeader("POISONED!");
            system.healthbarRefs[chr.currentBattlePosition - 4].setDamageNum(poisonDmg);
            StartCoroutine(system.healthbarRefs[chr.currentBattlePosition - 4].flashHUD(chr));
        }

        if (isKnockedOut)
        {
            system.infoText.SetText($"{chr.charName} succumbed to the poison!");

            if (chr.currentBattlePosition > 3)
            {
                GameObject.Destroy(system.enemyStations[chr.currentBattlePosition - 4].transform.GetChild(0).gameObject);
            }
        }

        yield return new WaitForSeconds(0.75f);
    }

    public override IEnumerator StatusCleared(CharData chr)
    {
        chr.statuses.Remove(statusName);
        system.infoText.SetText($"{chr.charName} is no longer poisoned!");
        yield return new WaitForSeconds(0.75f);
    }
}
