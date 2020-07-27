using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasesAndGrowths", menuName = "Base Stats and Growth Rates", order = 3)]
public class BasesAndGrowths : ScriptableObject
{
    // base + (growth * (level - 1))

    [Header("Experience Points")]
    public int baseExpGoal;
    public float baseExpGoalGrowth;
    public int maxExpGoal;

    [Header("HP")]
    public int baseHP;
    public float growthHP;

    [Header("Attack")]
    public int baseATK;
    public float growthATK;

    [Header("Defense")]
    public int baseDEF;
    public float growthDEF;

    [Header("Speed")]
    public int baseSPD;
    public float growthSPD;

    [Header("MP")]
    public int baseMP;
    public float growthMP;

    [Header("Magic")]
    public int baseMAG;
    public float growthMAG;

    [Header("Resistance")]
    public int baseRES;
    public float growthRES;

    [Header("Precision")]
    public int basePRC;
    public float growthPRC;

    [Header("Bravery")]
    public int baseBRV;
    public float growthBRV;

    [Header("Charisma")]
    public int baseCHA;
    public float growthCHA;

    [Header("Compassion")]
    public int baseCOM;
    public float growthCOM;

    [Header("Skill")]
    public int baseSKL;
    public float growthSKL;
}
