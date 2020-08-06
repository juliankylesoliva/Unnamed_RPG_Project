using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : StatusScript
{
    public override IEnumerator InitializeStatus(CharacterInfo chr, int numTurns)
    {
        if (!chr.containsStatus(statusName))
        {
            chr.giveStatus(statusName, numTurns);
            system.infoText.SetText($"{chr.Name} got burned!");
        }
        else
        {
            chr.extendStatus(statusName, numTurns);
            system.infoText.SetText("Extended burn duration!");
        }
        
        yield return new WaitForSeconds(0.75f);
    }

    public override IEnumerator DoStatus(CharacterInfo chr)
    {
        int burnDmg = (int)(chr.MaxHP * Random.Range(0.05f , 0.15f));

        if(burnDmg > chr.CurrentHP)
        {
            burnDmg = chr.CurrentHP - 1;
        }

        //system.infoText.SetText($"{chr.charName}'s burn dealt {burnDmg} damage!");
        chr.dealDamage(burnDmg);
        if (chr.UnitPosition < 4)
        {
            system.playerHealthbarRefs[chr.UnitPosition].setHeader("BURNED!");
            system.playerHealthbarRefs[chr.UnitPosition].setDamageNum(burnDmg);
            StartCoroutine(system.statusPanelRefs[chr.UnitPosition].drainHPBar(chr));
            StartCoroutine(system.playerHealthbarRefs[chr.UnitPosition].drainHealthbar(chr));
        }
        else
        {
            system.healthbarRefs[chr.UnitPosition - 4].setHeader("BURNED!");
            system.healthbarRefs[chr.UnitPosition - 4].setDamageNum(burnDmg);
            StartCoroutine(system.healthbarRefs[chr.UnitPosition - 4].flashHUD(chr));
        }

        yield return new WaitForSeconds(0.75f);
    }

    public override IEnumerator StatusCleared(CharacterInfo chr)
    {
        chr.removeStatus(statusName);
        system.infoText.SetText($"{chr.Name}'s burn healed!");
        yield return new WaitForSeconds(0.75f);
    }
}
