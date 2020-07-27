using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public ItemReferences itemRef;

    public Dictionary<string, int> items;

    public string[] inventoryContents;
    public int[] inventoryAmounts;

    public void InitInventory()
    {
        if(items != null)
        {
            return;
        }

        items = new Dictionary<string, int>();

        for (int i = 0; i < inventoryContents.Length; ++i)
        {
            DoAddItem(inventoryContents[i], inventoryAmounts[i]);
        }
    }

    public void DoAddItem(string name, int amount)
    {
        InitInventory();

        if (itemRef.LookForItem(name) == null)
        {
            return;
        }

        if(!items.ContainsKey(name))
        {
            items.Add(name, amount);
        }
        else
        {
            items[name] += amount;
            if(items[name] > 99)
            {
                items[name] = 99;
            }
        }
    }

    public void DoRemoveItem(string name, int amount)
    {
        InitInventory();

        if (itemRef.LookForItem(name) == null)
        {
            return;
        }

        if (!items.ContainsKey(name))
        {
            return;
        }
        else
        {
            items[name] -= amount;
            if(items[name] < 0)
            {
                items[name] = 0;
            }
        }
    }

    public ItemScript DoGetItem(string name)
    {
        InitInventory();

        ItemScript itemTemp;

        if(items.ContainsKey(name))
        {
            itemTemp = itemRef.LookForItem(name);
        }
        else
        {
            return null;
        }

        return itemTemp;
    }

    public bool DoConsumeItem(string name)
    {
        InitInventory();

        if (items.ContainsKey(name))
        {
            items[name]--;
            if(items[name] <= 0)
            {
                items.Remove(name);
            }
            return true;
        }

        return false;
    }
}
