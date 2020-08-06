using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RESUp : StatusScript
{
    public override IEnumerator InitializeStatus(CharacterInfo chr, int numTurns)
    {
        if (chr.containsStatus(StatusCondition.RES_Down))
        {
            StatusScript statTemp = system.statusList.LookForStatus(StatusCondition.RES_Down);
            yield return StartCoroutine(statTemp.StatusCleared(chr));
        }
        else if (chr.containsStatus(statusName))
        {
            chr.extendStatus(statusName, numTurns);
            system.infoText.SetText("Resistance increase extended!");
            yield return new WaitForSeconds(0.75f);
        }
        else
        {
            chr.giveStatus(statusName, numTurns);
            chr.BuffModRES = 1;
            system.infoText.SetText("Resistance up!");
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
        chr.BuffModRES = 0;
        system.infoText.SetText("Resistance reverted!");
        yield return new WaitForSeconds(0.75f);
    }
}
