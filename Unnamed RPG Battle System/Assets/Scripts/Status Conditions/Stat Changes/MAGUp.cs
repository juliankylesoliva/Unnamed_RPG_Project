using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAGUp : StatusScript
{
    public override IEnumerator InitializeStatus(CharacterInfo chr, int numTurns)
    {
        if (chr.containsStatus(StatusCondition.MAG_Down))
        {
            StatusScript statTemp = system.statusList.LookForStatus(StatusCondition.MAG_Down);
            yield return StartCoroutine(statTemp.StatusCleared(chr));
        }
        else if (chr.containsStatus(statusName))
        {
            chr.extendStatus(statusName, numTurns);
            system.infoText.SetText("Magic increase extended!");
            yield return new WaitForSeconds(0.75f);
        }
        else
        {
            chr.giveStatus(statusName, numTurns);
            chr.BuffModMAG = 1;
            system.infoText.SetText("Magic up!");
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
        chr.BuffModMAG = 0;
        system.infoText.SetText("Magic reverted!");
        yield return new WaitForSeconds(0.75f);
    }
}
