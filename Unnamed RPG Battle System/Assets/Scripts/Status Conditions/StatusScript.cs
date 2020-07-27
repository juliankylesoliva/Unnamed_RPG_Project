using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusScript : MonoBehaviour
{
    public StatusCondition statusName;

    public BattleSystem system;
    public DamageCalculator calc;

    public abstract IEnumerator InitializeStatus(CharData chr, int numTurns);

    public abstract IEnumerator DoStatus(CharData chr);

    public abstract IEnumerator StatusCleared(CharData chr);
}
