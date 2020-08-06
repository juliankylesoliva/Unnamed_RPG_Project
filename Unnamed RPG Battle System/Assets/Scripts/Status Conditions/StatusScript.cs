using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusScript : MonoBehaviour
{
    public StatusCondition statusName;

    public BattleSystem system;
    public DamageCalculator calc;

    public abstract IEnumerator InitializeStatus(CharacterInfo chr, int numTurns);

    public abstract IEnumerator DoStatus(CharacterInfo chr);

    public abstract IEnumerator StatusCleared(CharacterInfo chr);
}
