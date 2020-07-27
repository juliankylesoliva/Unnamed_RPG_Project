using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : SkillScript
{
    public override void PrepareSkill()
    {
        system.infoText.SetText("Click on an enemy to attack!");
        system.ChangeToCamPosition(10);
        system.menuState = MenuState.Attack;
    }

    public override IEnumerator DoSkill(CharData src, CharData dst)
    {
        system.ChangeToCamPosition(src.currentBattlePosition);

        yield return StartCoroutine(DoSendInfoMessage($"{src.charName} attacks!"));

        yield return StartCoroutine(DoChangeCamToPlayerTargets(dst));

        if (DoHitCheck(src, dst))
        {
            int dmg = DoDamageCalculation(src, dst);

            if(DoDamageMultiplication(src, dst, ref dmg))
            {
                yield return new WaitForSeconds(1.0f);
            }

            yield return StartCoroutine(DoDealDamage(dst, dmg));
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
