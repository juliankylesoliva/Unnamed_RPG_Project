using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : SkillScript
{
    public override void PrepareSkill()
    {
        system.infoText.SetText("Press Space to guard!");
        system.menuState = MenuState.Guard;
    }

    public override IEnumerator DoSkill(CharData src, CharData dst)
    {
        system.infoText.SetText($"{src.charName} guards.");
        src.isGuarding = true;

        yield return new WaitForSeconds(1.0f);
    }

    public override IEnumerator DoMainActionUnit(CharData src, CharData dst)
    {
        yield return new WaitForSeconds(0.0f);
    }
}
