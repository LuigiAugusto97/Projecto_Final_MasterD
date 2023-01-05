using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShopKeeper_CTRL : MonoBehaviour
{
    [Header("Dialog Settings")]
    public Dialog inicialDialog;
    public List<string> inicialChoices;

    [Header("ShopItems Settings")]
    [SerializeField] List<ItemBase> availableItems;
    public List<ItemBase> AvailableItems
    {
        get { return availableItems; }
    }
    public IEnumerator HandleShop()
    {
        yield return ShopCTRL.i.StarShopping(this);
    }

}
