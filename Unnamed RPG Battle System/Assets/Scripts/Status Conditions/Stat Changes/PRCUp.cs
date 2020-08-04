using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PRCUp : StatusScript
{
    public override IEnumerator InitializeStatus(CharacterInfo chr, int numTurns)
    {
        if (chr.containsStatus(StatusCondition.PRC_Down))
        {
            StatusScript statTemp = system.statusList.LookForStatus(StatusCondition.PRC_Down);
            yield return StartCoroutine(statTemp.StatusCleared(chr));
        }
        else if (chr.containsStatus(statusName))
        {
            chr.extendStatus(statusName, numTurns);
            system.infoText.SetText("Precision increase extended!");
            yield return new WaitForSeconds(0.75f);
        }
        else
        {
            chr.giveStatus(statusName, numTurns);
            chr.BuffModPRC = 1;
            system.infoText.SetText("Precision up!");
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
        chr.BuffModPRC = 0;
        system.infoText.SetText("Precision reverted!");
        yield return new WaitForSeconds(0.75f);
    }
}
