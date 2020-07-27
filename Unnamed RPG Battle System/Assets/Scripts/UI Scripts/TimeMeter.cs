using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum MeterStates {Stopped, Draining, Filling}
public class TimeMeter : MonoBehaviour
{
    public Slider timeMeter;
    public TextMeshProUGUI meterText;

    public Color playerColor;
    public Color enemyColor;
    public Image fillObject;

    public BattleSystem system;
    public int meterRate;

    public MeterStates currentMeterState;

    void Update()
    {
        meterText.SetText($"{(int)timeMeter.value} TP");

        switch(system.currentState)
        {
            case BattleState.PlayerPhase:
                fillObject.color = playerColor;
                break;
            case BattleState.EnemyPhase:
                fillObject.color = enemyColor;
                break;
            default:
                break;
        }

        switch(currentMeterState)
        {
            case MeterStates.Stopped:
                break;
            case MeterStates.Draining:
                timeMeter.value -= meterRate * Time.deltaTime;
                if(timeMeter.value <= 0)
                {
                    currentMeterState = MeterStates.Stopped;
                    StartCoroutine(system.EndOfPhase());
                }
                break;
            case MeterStates.Filling:
                break;
            default:
                break;
        }
    }

    public void AddToTimer(int time)
    {
        timeMeter.value += time;
    }

    public void SubFromTimer(int time)
    {
        timeMeter.value -= time;
    }

    public void RefillTimer()
    {
        timeMeter.value = timeMeter.maxValue;
    }

    public void SetMeterRate(int rate)
    {
        meterRate = rate;
    }

    public void SetMeterState(MeterStates state)
    {
        currentMeterState = state;
    }

    public void HideMeter()
    {
        timeMeter.gameObject.SetActive(false);
    }

    public void ShowMeter()
    {
        timeMeter.gameObject.SetActive(true);
    }
}
