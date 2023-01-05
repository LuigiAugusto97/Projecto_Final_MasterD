using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour, ISavable
{
    [SerializeField] List<ItemData> _inventory;
    public List<ItemData> _Inventory
    {
        get { return _inventory; }
    }
    //Return to show possible dialog
    public ItemBase UseItem(int indexofItem, Units unit)
    {
        bool itemUsed = _inventory[indexofItem].Item.Use(unit);
        if (itemUsed)
        {
            return _inventory[indexofItem].Item;
        }
        return null;
    }
    //To remove an item count
    public void DecreaseItemCount(ItemBase item, int count = 1)
    {
        var itemToBeUsed = _inventory.First(invItem => invItem.Item == item);
        itemToBeUsed.ItemCount -= count;
        if (itemToBeUsed.ItemCount == 0)
        {
            _inventory.Remove(itemToBeUsed);
        }
    }

    //To add an Item to the Inventory
    public void AddItem(ItemBase itemToAdd, int count = 1)
    {
        var itemAlreadyinInventory = _inventory.FirstOrDefault(item => item.Item == itemToAdd);
        if (itemAlreadyinInventory != null)
        {
            itemAlreadyinInventory.ItemCount += count;
        }
        else
        {
            _inventory.Add(new ItemData()
            {
                Item = itemToAdd,
                ItemCount = count
            });
        }
    }

    //To check if inventory Contains Item
    public bool CheckIfItemPresent(ItemBase itemToCheck, int count = 1)
    {
        var itemAlreadyinInventory = _inventory.FirstOrDefault(item => item.Item == itemToCheck);
        if (itemAlreadyinInventory != null)
        {
            if (itemAlreadyinInventory.ItemCount >= count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    //Get the item count
    public int GetItemCount(ItemBase itemToCount)
    {
        var itemToGetCount = _inventory.FirstOrDefault(item => item.Item == itemToCount);
        if (itemToGetCount != null)
        {
            return itemToGetCount.ItemCount;
        }
        else
        {
            return 0;
        }

    }

    //Always find Players Inventory
    public static Inventory GetInventory()
    {
        return FindObjectOfType<Player_CTRL>().GetComponent<Inventory>();
    }


    //SAVE SYSTEM
    public object SaveData()
    {
        var saveData = new InventorySaveData
        {
            items = _inventory.Select(i => i.GetSavedData()).ToList()
        };
        return saveData;
    }

    public void LoadData(object data)
    {
        var saveData = (InventorySaveData)data;

        _inventory = saveData.items.Select(i => new ItemData(i)).ToList();
    }
}
[Serializable]
public class ItemData
{
    [SerializeField] ItemBase item;
    [SerializeField] int itemCount;

    public ItemData()
    {

    }
    public ItemData(ItemSaveData saveData)
    {
        item = ItemsDB.GetItemByItemName(saveData.name);
        itemCount = saveData.count;
    }
    public ItemSaveData GetSavedData()
    {
        var saveData = new ItemSaveData()
        {
            name = item.Name,
            count = itemCount
        };
        return saveData;
    }

    public ItemBase Item
    {
        get { return item; }
        set { item = value; }
    }
    public int ItemCount 
    {
        get { return itemCount; }
        set { itemCount = value; }
    }
}

[Serializable]
public class ItemSaveData
{
    public string name;
    public int count;
}

[Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> items;
}
