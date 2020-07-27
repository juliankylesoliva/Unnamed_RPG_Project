using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemReferences : MonoBehaviour
{
    public ItemScript[] listOfItems;
    public Dictionary<string, ItemScript> itemDictionary;

    public void InitItemDictionary()
    {
        if(itemDictionary != null)
        {
            return;
        }

        itemDictionary = new Dictionary<string, ItemScript>();

        foreach(ItemScript item in listOfItems)
        {
            itemDictionary.Add(item.info.skillName, item);
        }
    }

    public ItemScript LookForItem(string name)
    {
        InitItemDictionary();

        if(itemDictionary.ContainsKey(name))
        {
            return itemDictionary[name];
        }

        return null;
    }
}
