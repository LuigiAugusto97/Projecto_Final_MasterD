using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsDB
{
    static Dictionary<string, ItemBase> item_Data;
    public static void GetItems()
    {
        item_Data = new Dictionary<string, ItemBase>();

        var AllItemssData = Resources.LoadAll<ItemBase>("");
        foreach (var itemData in AllItemssData)
        {
            if (item_Data.ContainsKey(itemData.Name))
            {
                Debug.LogError($"That item,{itemData.Name}, already exist");
                continue;
            }
            item_Data[itemData.Name] = itemData;
        }
    }

    public static ItemBase GetItemByItemName(string itemName)
    {
        if (!item_Data.ContainsKey(itemName))
        {
            Debug.LogError($"There is no item with the name:{itemName}");
            return null;
        }

        return item_Data[itemName];
    }
}
