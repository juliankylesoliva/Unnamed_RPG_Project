using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEFUp : StatusScript
{
    public override IEnumerator InitializeStatus(CharacterInfo chr, int numTurns)
    {
        if (chr.containsStatus(StatusCondition.DEF_Down))
        {
            StatusScript statTemp = system.statusList.LookForStatus(StatusCondition.DEF_Down);
            yield return StartCoroutine(statTemp.StatusCleared(chr));
        }
        else if (chr.containsStatus(statusName))
        {
            chr.extendStatus(statusName, numTurns);
            system.infoText.SetText("Defense increase extended!");
            yield return new WaitForSeconds(0.75f);
        }
        else
        {
            chr.giveStatus(statusName, numTurns);
            chr.BuffModDEF = 1;
            system.infoText.SetText("Defense up!");
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
        chr.BuffModDEF = 0;
        system.infoText.SetText("Defense reverted!");
        yield return new WaitForSeconds(0.75f);
    }
}
