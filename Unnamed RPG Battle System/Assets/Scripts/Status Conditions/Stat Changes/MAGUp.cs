using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAGUp : StatusScript
{
    public override IEnumerator InitializeStatus(CharData chr, int numTurns)
    {
        if (chr.statuses.ContainsKey(StatusCondition.MAG_Down))
        {
            StatusScript statTemp = system.statusList.LookForStatus(StatusCondition.MAG_Down);
            yield return StartCoroutine(statTemp.StatusCleared(chr));
        }
        else if (chr.statuses.ContainsKey(statusName))
        {
            chr.statuses[statusName] += numTurns;
            system.infoText.SetText("Magic increase extended!");
            yield return new WaitForSeconds(0.75f);
        }
        else
        {
            chr.statuses.Add(statusName, numTurns);
            chr.MAGMod = 1.25f;
            system.infoText.SetText("Magic up!");
            yield return new WaitForSeconds(0.75f);
        }
    }

    public override IEnumerator DoStatus(CharData chr)
    {
        yield return new WaitForSeconds(0.0f);
    }

    public override IEnumerator StatusCleared(CharData chr)
    {
        chr.statuses.Remove(statusName);
        chr.MAGMod = 1.00f;
        system.infoText.SetText("Magic reverted!");
        yield return new WaitForSeconds(0.75f);
    }
}
