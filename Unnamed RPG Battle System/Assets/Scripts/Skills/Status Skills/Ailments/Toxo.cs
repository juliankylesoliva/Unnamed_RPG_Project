using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toxo : SkillScript
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

        yield return StartCoroutine(DoSendInfoMessage($"{src.charName} casted Toxo!"));

        yield return StartCoroutine(DoChangeCamToPlayerTargets(dst));

        if (DoHitCheck(src, dst))
        {
            yield return StartCoroutine(AddPoison(dst, 10));
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
        }
    }

    public override IEnumerator DoMainActionUnit(CharData src, CharData dst)
    {
        yield return new WaitForSeconds(0.0f);
    }
}
