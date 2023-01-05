using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] string _name;
    [SerializeField] string description;
    [SerializeField] Sprite icon;
    [SerializeField] ItemType type;
    [SerializeField] float price;
    [SerializeField] bool canSell;

    public string Name
    {
        get { return _name; }
    }
    public string Descritption
    {
        get { return description; }
    }
    public Sprite Icon
    {
        get { return icon; }
    }
    public ItemType Type
    {
        get { return type; }
    }
    public float Price
    {
        get { return price; }
    }
    public bool CanSell
    {
        get { return canSell; }
    }

    //To be used By all subClasses and can always return false
    public virtual bool Use(Units unit)
    {
        return false;
    }
}
public enum ItemType
{
    OnFriendly, OnEnemy
}