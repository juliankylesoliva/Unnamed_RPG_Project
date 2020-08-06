using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spedhi : SkillScript
{
    public override void PrepareSkill()
    {
        system.infoText.SetText("Click on an ally!");
        system.ChangeToCamPosition(9);
        system.targetMode = TargetMode.AnyAlly;
    }

    public override IEnumerator DoSkill(CharacterInfo src, CharacterInfo dst)
    {
        system.ChangeToCamPosition(src.UnitPosition);

        yield return StartCoroutine(DoSendInfoMessage($"{src.Name} casted Spedhi!"));

        yield return StartCoroutine(DoChangeCamToPlayerTargets(dst));

        yield return StartCoroutine(DoSPDUp(dst, 4));
    }

    public override IEnumerator DoMainActionUnit(CharacterInfo src, CharacterInfo dst)
    {
        yield return new WaitForSeconds(0.0f);
    }
}
