using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soak : StatusScript
{
    public override IEnumerator InitializeStatus(CharacterInfo chr, int numTurns)
    {
        if (chr.containsStatus(statusName))
        {
            chr.extendStatus(statusName, numTurns);
            system.infoText.SetText("Soak duration extended!");
        }
        else
        {
            chr.giveStatus(statusName, numTurns);
            system.infoText.SetText($"{chr.Name} got soaked!");
        }
        yield return new WaitForSeconds(0.75f);
    }

    public override IEnumerator DoStatus(CharacterInfo chr)
    {
        yield return new WaitForSeconds(0.0f);
    }

    public override IEnumerator StatusCleared(CharacterInfo chr)
    {
        chr.removeStatus(statusName);
        system.infoText.SetText($"{chr.Name} is all dry!");
        yield return new WaitForSeconds(0.75f);
    }
}
