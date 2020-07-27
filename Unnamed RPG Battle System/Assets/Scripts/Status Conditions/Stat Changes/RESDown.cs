using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RESDown : StatusScript
{
    public override IEnumerator InitializeStatus(CharData chr, int numTurns)
    {
        if (chr.statuses.ContainsKey(StatusCondition.RES_Up))
        {
            StatusScript statTemp = system.statusList.LookForStatus(StatusCondition.RES_Up);
            yield return StartCoroutine(statTemp.StatusCleared(chr));
        }
        else if (chr.statuses.ContainsKey(statusName))
        {
            chr.statuses[statusName] += numTurns;
            system.infoText.SetText("Resistance decrease extended!");
            yield return new WaitForSeconds(0.75f);
        }
        else
        {
            chr.statuses.Add(statusName, numTurns);
            chr.RESMod = 0.75f;
            system.infoText.SetText("Resistance down!");
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
        chr.RESMod = 1.00f;
        system.infoText.SetText("Resistance reverted!");
        yield return new WaitForSeconds(0.75f);
    }
}
