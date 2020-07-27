using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soak : StatusScript
{
    public override IEnumerator InitializeStatus(CharData chr, int numTurns)
    {
        if (chr.statuses.ContainsKey(statusName))
        {
            chr.statuses[statusName] += numTurns;
            system.infoText.SetText("Soak duration extended!");
        }
        else
        {
            chr.statuses.Add(statusName, numTurns);
            system.infoText.SetText($"{chr.charName} got soaked!");
        }
        yield return new WaitForSeconds(0.75f);
    }

    public override IEnumerator DoStatus(CharData chr)
    {
        yield return new WaitForSeconds(0.0f);
    }

    public override IEnumerator StatusCleared(CharData chr)
    {
        chr.statuses.Remove(statusName);
        system.infoText.SetText($"{chr.charName} is all dry!");
        yield return new WaitForSeconds(0.75f);
    }
}
