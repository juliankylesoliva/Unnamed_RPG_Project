using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : StatusScript
{
    public override IEnumerator InitializeStatus(CharacterInfo chr, int numTurns)
    {
        if (!chr.containsStatus(statusName))
        {
            chr.giveStatus(statusName, numTurns);
            system.infoText.SetText($"{chr.Name} got poisoned!");
        }
        else
        {
            system.infoText.SetText($"{chr.Name} is already poisoned!");
        }
        yield return new WaitForSeconds(0.75f);
    }

    public override IEnumerator DoStatus(CharacterInfo chr)
    {
        int poisonDmg = (int)(chr.MaxHP * 0.20);

        //system.infoText.SetText($"{chr.charName}'s burn dealt {burnDmg} damage!");
        bool isKnockedOut = chr.dealDamage(poisonDmg);
        if (chr.UnitPosition < 4)
        {
            system.playerHealthbarRefs[chr.UnitPosition].setHeader("POISONED!");
            system.playerHealthbarRefs[chr.UnitPosition].setDamageNum(poisonDmg);
            StartCoroutine(system.statusPanelRefs[chr.UnitPosition].drainHPBar(chr));
            StartCoroutine(system.playerHealthbarRefs[chr.UnitPosition].drainHealthbar(chr));
        }
        else
        {
            system.healthbarRefs[chr.UnitPosition - 4].setHeader("POISONED!");
            system.healthbarRefs[chr.UnitPosition - 4].setDamageNum(poisonDmg);
            StartCoroutine(system.healthbarRefs[chr.UnitPosition - 4].flashHUD(chr));
        }

        if (isKnockedOut)
        {
            system.infoText.SetText($"{chr.Name} succumbed to the poison!");

            if (chr.UnitPosition > 3)
            {
                GameObject.Destroy(system.enemyStations[chr.UnitPosition - 4].transform.GetChild(0).gameObject);
            }
        }

        yield return new WaitForSeconds(0.75f);
    }

    public override IEnumerator StatusCleared(CharacterInfo chr)
    {
        chr.removeStatus(statusName);
        system.infoText.SetText($"{chr.Name} is no longer poisoned!");
        yield return new WaitForSeconds(0.75f);
    }
}
