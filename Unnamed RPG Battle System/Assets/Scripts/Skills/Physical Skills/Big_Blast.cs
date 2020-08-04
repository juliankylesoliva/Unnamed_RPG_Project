﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Big_Blast : SkillScript
{
    public override void PrepareSkill()
    {
        system.infoText.SetText("Click on an enemy to attack!");
        system.ChangeToCamPosition(10);
        system.targetMode = TargetMode.AnyEnemy;
    }

    public override IEnumerator DoSkill(CharacterInfo src, CharacterInfo dst)
    {
        system.ChangeToCamPosition(src.UnitPosition);

        yield return StartCoroutine(DoSendInfoMessage($"{src.Name} used Big Blast!"));

        yield return StartCoroutine(DoChangeCamToPlayerTargets(dst));

        if (DoHitCheck(src, dst))
        {
            int dmg = DoDamageCalculation(src, dst);

            if (DoDamageMultiplication(src, dst, ref dmg))
            {
                yield return new WaitForSeconds(1.0f);
            }

            yield return StartCoroutine(DoDealDamage(dst, dmg));

            CharacterInfo dataRef = GetAdjacentTarget(dst, -1);
            if(dataRef != null)
            {
                DoHealthBarText(dataRef, "BLAST!", dmg / 3);
                yield return StartCoroutine(DoDealDamage(dataRef, dmg / 3));
            }

            dataRef = GetAdjacentTarget(dst, 1);
            if (dataRef != null)
            {
                DoHealthBarText(dataRef, "BLAST!", dmg / 3);
                yield return StartCoroutine(DoDealDamage(dataRef, dmg / 3));
            }
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
        }
    }

    public override IEnumerator DoMainActionUnit(CharacterInfo src, CharacterInfo dst)
    {
        yield return new WaitForSeconds(0.0f);
    }
}
