using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spedlo : SkillScript
{
    public override void PrepareSkill()
    {
        system.infoText.SetText("Click on an enemy!");
        system.ChangeToCamPosition(10);
        system.targetMode = TargetMode.AnyEnemy;
    }

    public override IEnumerator DoSkill(CharData src, CharData dst)
    {
        system.ChangeToCamPosition(src.currentBattlePosition);

        yield return StartCoroutine(DoSendInfoMessage($"{src.charName} casted Spedlo!"));

        yield return StartCoroutine(DoChangeCamToPlayerTargets(dst));

        yield return StartCoroutine(DoSPDDown(dst, 4));
    }

    public override IEnumerator DoMainActionUnit(CharData src, CharData dst)
    {
        yield return new WaitForSeconds(0.0f);
    }
}
