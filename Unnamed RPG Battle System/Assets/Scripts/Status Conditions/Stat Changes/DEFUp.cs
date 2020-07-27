using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEFUp : StatusScript
{
    public override IEnumerator InitializeStatus(CharData chr, int numTurns)
    {
        if (chr.statuses.ContainsKey(StatusCondition.DEF_Down))
        {
            StatusScript statTemp = system.statusList.LookForStatus(StatusCondition.DEF_Down);
            yield return StartCoroutine(statTemp.StatusCleared(chr));
        }
        else if (chr.statuses.ContainsKey(statusName))
        {
            chr.statuses[statusName] += numTurns;
            system.infoText.SetText("Defense increase extended!");
            yield return new WaitForSeconds(0.75f);
        }
        else
        {
            chr.statuses.Add(statusName, numTurns);
            chr.DEFMod = 1.25f;
            system.infoText.SetText("Defense up!");
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
        chr.DEFMod = 1.00f;
        system.infoText.SetText("Defense reverted!");
        yield return new WaitForSeconds(0.75f);
    }
}
