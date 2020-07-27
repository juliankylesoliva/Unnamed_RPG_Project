using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemScript : SkillScript
{
    public Inventory inventory;

    public void DoConsumeItem()
    {
        inventory.DoConsumeItem(info.skillName);
    }
}
