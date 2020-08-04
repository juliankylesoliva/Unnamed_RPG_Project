using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PRCDown : StatusScript
{
    public override IEnumerator InitializeStatus(CharacterInfo chr, int numTurns)
    {
        if (chr.containsStatus(StatusCondition.PRC_Up))
        {
            StatusScript statTemp = system.statusList.LookForStatus(StatusCondition.PRC_Up);
            yield return StartCoroutine(statTemp.StatusCleared(chr));
        }
        else if (chr.containsStatus(statusName))
        {
            chr.extendStatus(statusName, numTurns);
            system.infoText.SetText("Precision decrease extended!");
            yield return new WaitForSeconds(0.75f);
        }
        else
        {
            chr.giveStatus(statusName, numTurns);
            chr.BuffModPRC = -1;
            system.infoText.SetText("Precision down!");
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
