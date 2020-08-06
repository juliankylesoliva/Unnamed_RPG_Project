using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medicine : ItemScript
{
    public override void PrepareSkill()
    {
        system.infoText.SetText("Click on an ally to heal!");
        system.ChangeToCamPosition(9);
        system.targetMode = TargetMode.AnyAlly;
    }

    public override IEnumerator DoSkill(CharacterInfo src, CharacterInfo dst)
    {
        DoConsumeItem();

        system.ChangeToCamPosition(src.UnitPosition);

        yield return StartCoroutine(DoSendInfoMessage($"{src.Name} used Medicine!"));

        system.ChangeToCamPosition(dst.UnitPosition);

        yield return StartCoroutine(DoHPHealing(dst, info.hpRestore));
    }

    public override IEnumerator DoMainActionUnit(CharacterInfo src, CharacterInfo dst)
    {
        yield return new WaitForSeconds(0.0f);
    }
}
