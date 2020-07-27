using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aid : SkillScript
{
    public override void PrepareSkill()
    {
        system.infoText.SetText("Click on an ally to heal!");
        system.ChangeToCamPosition(9);
        system.targetMode = TargetMode.AnyAlly;
    }

    public override IEnumerator DoSkill(CharData src, CharData dst)
    {
        system.ChangeToCamPosition(src.currentBattlePosition);

        yield return StartCoroutine(DoSendInfoMessage($"{src.charName} casted Aid!"));

        int heal = DoHealingCalculation(src);

        system.ChangeToCamPosition(dst.currentBattlePosition);

        yield return StartCoroutine(DoHPHealing(dst, heal));
    }

    public override IEnumerator DoMainActionUnit(CharData src, CharData dst)
    {
        yield return new WaitForSeconds(0.0f);
    }
}
