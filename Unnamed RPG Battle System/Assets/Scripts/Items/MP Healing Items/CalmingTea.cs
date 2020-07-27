using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalmingTea : ItemScript
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

        yield return StartCoroutine(DoSendInfoMessage($"{src.charName} used Calming Tea!"));

        system.ChangeToCamPosition(dst.currentBattlePosition);

        yield return StartCoroutine(DoMPHealing(dst, info.mpRestore));
    }

    public override IEnumerator DoMainActionUnit(CharData src, CharData dst)
    {
        yield return new WaitForSeconds(0.0f);
    }
}
