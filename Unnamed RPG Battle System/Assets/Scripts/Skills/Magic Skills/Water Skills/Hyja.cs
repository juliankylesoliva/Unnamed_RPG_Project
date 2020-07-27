using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hyja : SkillScript
{
    public override void PrepareSkill()
    {
        system.infoText.SetText("Click on an enemy to attack!");
        system.ChangeToCamPosition(10);
        system.targetMode = TargetMode.AnyEnemy;
    }

    public override IEnumerator DoSkill(CharData src, CharData dst)
    {
        system.ChangeToCamPosition(src.currentBattlePosition);

        yield return StartCoroutine(DoSendInfoMessage($"{src.charName} casted Hyja!"));

        yield return StartCoroutine(DoChangeCamToPlayerTargets(dst));

        if (DoHitCheck(src, dst))
        {
            int dmg = DoDamageCalculation(src, dst);

            DoDamageMultiplication(src, dst, ref dmg);

            yield return StartCoroutine(DoDealDamage(dst, dmg));

            yield return StartCoroutine(RemoveBurn(dst));

            yield return StartCoroutine(AddSoak(dst, 1));
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
