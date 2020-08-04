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

    public override IEnumerator DoSkill(CharacterInfo src, CharacterInfo dst)
    {
        system.infoText.SetText($"{src.Name} guards.");
        src.IsGuarding = true;

        yield return new WaitForSeconds(1.0f);
    }

    public override IEnumerator DoMainActionUnit(CharacterInfo src, CharacterInfo dst)
    {
        yield return new WaitForSeconds(0.0f);
    }
}
