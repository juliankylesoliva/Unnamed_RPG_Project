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

    public override IEnumerator DoSkill(CharData src, CharData dst)
    {
        DoConsumeItem();

        system.ChangeToCamPosition(src.currentBattlePosition);

        yield return StartCoroutine(DoSendInfoMessage($"{src.charName} used Medicine!"));

        system.ChangeToCamPosition(dst.currentBattlePosition);

        yield return StartCoroutine(DoHPHealing(dst, info.hpRestore));
    }

    public override IEnumerator DoMainActionUnit(CharData src, CharData dst)
    {
        yield return new WaitForSeconds(0.0f);
    }
}
