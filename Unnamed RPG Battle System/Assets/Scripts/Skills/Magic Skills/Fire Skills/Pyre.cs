using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pyre : SkillScript
{
    public override void PrepareSkill()
    {
        system.infoText.SetText("Click on an enemy to attack!");
        system.ChangeToCamPosition(10);
        system.targetMode = TargetMode.AnyEnemy;
    }

    public override IEnumerator DoSkill(CharacterInfo src, CharacterInfo dst)
    {
        system.ChangeToCamPosition(src.UnitPosition);

        yield return StartCoroutine(DoSendInfoMessage($"{src.Name} casted Pyre!"));

        yield return StartCoroutine(DoChangeCamToPlayerTargets(dst));

        if (DoHitCheck(src, dst))
        {
            int dmg = DoDamageCalculation(src, dst);

            DoDamageMultiplication(src, dst, ref dmg);

            yield return StartCoroutine(DoDealDamage(dst, dmg));

            if(dst.containsStatus(StatusCondition.Burn) || DoPercentCheck(10))
            {
                yield return StartCoroutine(AddBurn(dst, 3));
            }

            yield return StartCoroutine(RemoveSoak(dst));
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
        }
    }

    public override IEnumerator DoMainActionUnit(CharacterInfo src, CharacterInfo dst)
    {
        yield return new WaitForSeconds(0.0f);
    }
}
