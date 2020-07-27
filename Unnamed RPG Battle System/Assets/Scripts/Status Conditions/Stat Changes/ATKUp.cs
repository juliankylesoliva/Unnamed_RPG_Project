using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATKUp : StatusScript
{
    public override IEnumerator InitializeStatus(CharData chr, int numTurns)
    {
        if(chr.statuses.ContainsKey(StatusCondition.ATK_Down))
        {
            StatusScript statTemp = system.statusList.LookForStatus(StatusCondition.ATK_Down);
            yield return StartCoroutine(statTemp.StatusCleared(chr));
        }
        else if(chr.statuses.ContainsKey(statusName))
        {
            chr.statuses[statusName] += numTurns;
            system.infoText.SetText("Attack increase extended!");
            yield return new WaitForSeconds(0.75f);
        }
        else
        {
            chr.statuses.Add(statusName, numTurns);
            chr.ATKMod = 1.25f;
            system.infoText.SetText("Attack up!");
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
        chr.ATKMod = 1.00f;
        system.infoText.SetText("Attack reverted!");
        yield return new WaitForSeconds(0.75f);
    }
}
