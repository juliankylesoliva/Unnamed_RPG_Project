using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusCondition {Burn, Soak, Poison, ATK_Up, ATK_Down, DEF_Up, DEF_Down, SPD_Up, SPD_Down, MAG_Up, MAG_Down, RES_Up, RES_Down, PRC_Up, PRC_Down}

public class StatusContainer : MonoBehaviour
{
    public StatusScript[] listOfStatuses;
    public Dictionary<StatusCondition, StatusScript> statusDictionary;

    public void InitStatusDictionary()
    {
        if(statusDictionary != null)
        {
            return;
        }

        statusDictionary = new Dictionary<StatusCondition, StatusScript>();

        foreach (StatusScript stat in listOfStatuses)
        {
            statusDictionary.Add(stat.statusName, stat);
        }
    }

    public StatusScript LookForStatus(StatusCondition name)
    {
        InitStatusDictionary();

        if(statusDictionary.ContainsKey(name))
        {
            return statusDictionary[name];
        }

        return null;
    }
}
