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

    public override IEnumerator DoSkill(CharacterInfo src, CharacterInfo dst)
    {
        system.ChangeToCamPosition(src.UnitPosition);

        yield return StartCoroutine(DoSendInfoMessage($"{src.Name} casted Aid!"));

        int heal = DoHealingCalculation(src);

        system.ChangeToCamPosition(dst.UnitPosition);

        yield return StartCoroutine(DoHPHealing(dst, heal));
    }

    public override IEnumerator DoMainActionUnit(CharacterInfo src, CharacterInfo dst)
    {
        yield return new WaitForSeconds(0.0f);
    }
}
