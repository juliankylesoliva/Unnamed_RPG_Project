using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPDDown : StatusScript
{
    public override IEnumerator InitializeStatus(CharacterInfo chr, int numTurns)
    {
        if (chr.containsStatus(StatusCondition.SPD_Up))
        {
            StatusScript statTemp = system.statusList.LookForStatus(StatusCondition.SPD_Up);
            yield return StartCoroutine(statTemp.StatusCleared(chr));
        }
        else if (chr.containsStatus(statusName))
        {
            chr.extendStatus(statusName, numTurns);
            system.infoText.SetText("Speed decrease extended!");
            yield return new WaitForSeconds(0.75f);
        }
        else
        {
            chr.giveStatus(statusName, numTurns);
            chr.BuffModSPD = -1;
            system.infoText.SetText("Speed down!");
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
        chr.BuffModSPD = 0;
        system.infoText.SetText("Speed reverted!");
        yield return new WaitForSeconds(0.75f);
    }
}
