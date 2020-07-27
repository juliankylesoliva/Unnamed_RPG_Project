using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Big_Blast : SkillScript
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

        yield return StartCoroutine(DoSendInfoMessage($"{src.charName} used Big Blast!"));

        yield return StartCoroutine(DoChangeCamToPlayerTargets(dst));

        if (DoHitCheck(src, dst))
        {
            int dmg = DoDamageCalculation(src, dst);

            if (DoDamageMultiplication(src, dst, ref dmg))
            {
                yield return new WaitForSeconds(1.0f);
            }

            yield return StartCoroutine(DoDealDamage(dst, dmg));

            CharData dataRef = GetAdjacentTarget(dst, -1);
            if(dataRef != null)
            {
                DoHealthBarText(dataRef, "BLAST!", dmg / 3);
                yield return StartCoroutine(DoDealDamage(dataRef, dmg / 3));
            }

            dataRef = GetAdjacentTarget(dst, 1);
            if (dataRef != null)
            {
                DoHealthBarText(dataRef, "BLAST!", dmg / 3);
                yield return StartCoroutine(DoDealDamage(dataRef, dmg / 3));
            }
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
