using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wild_Aim : SkillScript
{
    public override void PrepareSkill()
    {
        system.infoText.SetText("Firing at random enemies!");
        system.ChangeToCamPosition(10);
        system.targetMode = TargetMode.AllEnemies;
    }

    public override IEnumerator DoSkill(CharacterInfo src, CharacterInfo dst)
    {
        system.ChangeToCamPosition(src.UnitPosition);

        yield return StartCoroutine(DoSendInfoMessage($"{src.Name} used Wild Aim!"));

        yield return StartCoroutine(DoChangeCamToPlayerTargets(dst));

        int numTimes = Random.Range(3, 6);
        int times = 0;

        for(int i = 0; i < numTimes; ++i)
        {
            CharacterInfo randomTarget = GetRandomTarget(dst);

            if(randomTarget != null)
            {
                yield return StartCoroutine(DoMainActionUnit(src, randomTarget));
            }
            else
            {
                break;
            }

            times++;
        }

        system.infoText.SetText($"Fired {times} times!");
        yield return new WaitForSeconds(0.75f);
    }

    public override IEnumerator DoMainActionUnit(CharacterInfo src, CharacterInfo dst)
    {
        if (DoHitCheck(src, dst))
        {
            int dmg = DoDamageCalculation(src, dst);

            if (DoDamageMultiplication(src, dst, ref dmg))
            {
                yield return new WaitForSeconds(0.4f);
            }

            yield return StartCoroutine(DoDealDamage(dst, dmg));
        }
        else
        {
            yield return new WaitForSeconds(0.4f);
        }
    }
}
