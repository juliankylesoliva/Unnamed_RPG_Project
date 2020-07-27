using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPDDown : StatusScript
{
    public override IEnumerator InitializeStatus(CharData chr, int numTurns)
    {
        if (chr.statuses.ContainsKey(StatusCondition.SPD_Up))
        {
            StatusScript statTemp = system.statusList.LookForStatus(StatusCondition.SPD_Up);
            yield return StartCoroutine(statTemp.StatusCleared(chr));
        }
        else if (chr.statuses.ContainsKey(statusName))
        {
            chr.statuses[statusName] += numTurns;
            system.infoText.SetText("Speed decrease extended!");
            yield return new WaitForSeconds(0.75f);
        }
        else
        {
            chr.statuses.Add(statusName, numTurns);
            chr.SPDMod = 0.75f;
            system.infoText.SetText("Speed down!");
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
        chr.SPDMod = 1.00f;
        system.infoText.SetText("Speed reverted!");
        yield return new WaitForSeconds(0.75f);
    }
}
