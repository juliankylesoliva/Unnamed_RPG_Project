using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemButtonUI : MonoBehaviour
{
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI descriptionText;

    public void setButtonText(SkillInfo info, int amount)
    {
        headerText.SetText($"{info.skillName} (x{amount})");
        descriptionText.SetText($"[{info.skillType}] {info.description}");
    }
}
