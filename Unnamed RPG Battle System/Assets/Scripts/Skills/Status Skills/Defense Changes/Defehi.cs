using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defehi : SkillScript
{
    public override void PrepareSkill()
    {
        system.infoText.SetText("Click on an ally!");
        system.ChangeToCamPosition(9);
        system.targetMode = TargetMode.AnyAlly;
    }

    public override IEnumerator DoSkill(CharData src, CharData dst)
    {
        system.ChangeToCamPosition(src.currentBattlePosition);

        yield return StartCoroutine(DoSendInfoMessage($"{src.charName} casted Defehi!"));

        yield return StartCoroutine(DoChangeCamToPlayerTargets(dst));

        yield return StartCoroutine(DoDEFUp(dst, 4));
    }

    public override IEnumerator DoMainActionUnit(CharData src, CharData dst)
    {
        yield return new WaitForSeconds(0.0f);
    }
}
