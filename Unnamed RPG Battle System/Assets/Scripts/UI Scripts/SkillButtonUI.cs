using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillButtonUI : MonoBehaviour
{
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI descriptionText;

    public int skillMenuPosition = -1;

    public void setButtonText(SkillInfo info, CharacterInfo chr)
    {
        if(info.mpCost > 0)
        {
            headerText.SetText($"{info.skillName} ({info.mpCost} MP, {info.timeCost} TP)");
        }
        else if(info.hpCostPercent > 0)
        {
            headerText.SetText($"{info.skillName} ({chr.getHPPercent(info.hpCostPercent)} HP, {info.timeCost} TP)");
        }
        else
        {
            headerText.SetText($"{info.skillName} (??? HP/MP/TP)");
        }
        
        descriptionText.SetText($"[{info.skillType}] {info.description}");
    }
}
