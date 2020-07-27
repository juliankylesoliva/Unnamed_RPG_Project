using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mulpyre : SkillScript
{
    public override void PrepareSkill()
    {
        system.infoText.SetText("All enemies targeted!");
        system.ChangeToCamPosition(10);
        system.targetMode = TargetMode.AllEnemies;
    }

    public override IEnumerator DoSkill(CharData src, CharData dst)
    {
        system.ChangeToCamPosition(src.currentBattlePosition);

        yield return StartCoroutine(DoSendInfoMessage($"{src.charName} casted Mulpyre!"));

        yield return StartCoroutine(DoChangeCamToAllPlayerTargets(dst));

        yield return DoLoopMainAction(src, dst);
    }

    public override IEnumerator DoMainActionUnit(CharData src, CharData dst)
    {
        if (DoHitCheck(src, dst))
        {
            int dmg = DoDamageCalculation(src, dst);

            DoDamageMultiplication(src, dst, ref dmg);

            yield return StartCoroutine(DoDealDamage(dst, dmg));

            if (dst.statuses.ContainsKey(StatusCondition.Burn) || DoPercentCheck(10))
            {
                yield return StartCoroutine(AddBurn(dst, 3));
            }

            yield return StartCoroutine(RemoveSoak(dst));
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }
    }
}
