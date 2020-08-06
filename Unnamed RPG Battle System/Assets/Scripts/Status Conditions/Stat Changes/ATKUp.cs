﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATKUp : StatusScript
{
    public override IEnumerator InitializeStatus(CharacterInfo chr, int numTurns)
    {
        if(chr.containsStatus(StatusCondition.ATK_Down))
        {
            StatusScript statTemp = system.statusList.LookForStatus(StatusCondition.ATK_Down);
            yield return StartCoroutine(statTemp.StatusCleared(chr));
        }
        else if(chr.containsStatus(statusName))
        {
            chr.extendStatus(statusName, numTurns);
            system.infoText.SetText("Attack increase extended!");
            yield return new WaitForSeconds(0.75f);
        }
        else
        {
            chr.giveStatus(statusName, numTurns);
            chr.BuffModATK = 1;
            system.infoText.SetText("Attack up!");
            yield return new WaitForSeconds(0.75f);
        }
    }

    public override IEnumerator DoStatus(CharacterInfo chr)
    {
        yield return new WaitForSeconds(0.0f);
    }

    public override IEnumerator StatusCleared(CharacterInfo chr)
    {
        chr.removeStatus(statusName);
        chr.BuffModATK = 0;
        system.infoText.SetText("Attack reverted!");
        yield return new WaitForSeconds(0.75f);
    }
}
