using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defelo : SkillScript
{
    public override void PrepareSkill()
    {
        system.infoText.SetText("Click on an enemy!");
        system.ChangeToCamPosition(10);
        system.targetMode = TargetMode.AnyEnemy;
    }

    public override IEnumerator DoSkill(CharacterInfo src, CharacterInfo dst)
    {
        system.ChangeToCamPosition(src.UnitPosition);

        yield return StartCoroutine(DoSendInfoMessage($"{src.Name} casted Defelo!"));

        yield return StartCoroutine(DoChangeCamToPlayerTargets(dst));

        yield return StartCoroutine(DoDEFDown(dst, 4));
    }

    public override IEnumerator DoMainActionUnit(CharacterInfo src, CharacterInfo dst)
    {
        yield return new WaitForSeconds(0.0f);
    }
}
