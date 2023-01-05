using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleInventoryUI : MonoBehaviour
{
    [Header("MenuOptions")]
    [SerializeField] GameObject menu;

    [Header("Items")]
    [SerializeField] GameObject itemUIList;
    [SerializeField] ItemUI itemUIprefab;
    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;

    [Header("list of total items")]
    public Inventory playersInventory;
    [SerializeField] List<ItemUI> totalItems;

    [Header("Scrolling")]
    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;
    private RectTransform itemListUI_rect;
    private const int itemsInView = 8;

    //UIHandle
    public Action onMenuBack;

    private void Awake()
    {
        playersInventory = Inventory.GetInventory();
        itemListUI_rect = itemUIList.GetComponent<RectTransform>();
    }
    private void Start()
    {
        UpdateItemList();
    }
    public void HandleUIUpdate(int selected)
    {
        HandleUIScrolling(selected);
        for (int i = 0; i < totalItems.Count; i++)
        {
            if (i == selected)
            {
                totalItems[i].Indecate(true);
                itemIcon.sprite = playersInventory._Inventory[i].Item.Icon;
                itemDescription.text = playersInventory._Inventory[i].Item.Descritption;
            }
            else
            {
                totalItems[i].Indecate(false);
            }
        }
    }

    private void HandleUIScrolling(int selected)
    {
        if (totalItems.Count <= itemsInView) return;

        //Scroll view after middle point
        float scrollLocation = Mathf.Clamp(selected - (itemsInView / 2), 0, selected) * totalItems[0].ListBoxHeight;

        if (selected < totalItems.Count - (itemsInView / 2))
        {
            itemListUI_rect.localPosition = new Vector2(itemListUI_rect.localPosition.x, scrollLocation);
        }

        //show if more itemns above
        bool showUpArrow = selected > itemsInView / 2;
        upArrow.gameObject.SetActive(showUpArrow);
        //show if more item bellow
        bool showDownArrow = selected + itemsInView / 2 < totalItems.Count;
        downArrow.gameObject.SetActive(showDownArrow);
    }

    public void UpdateItemList()
    {
        // Clear Items
        foreach (Transform child in itemUIList.transform)
        {
            Destroy(child.gameObject);
            if (totalItems.Count != 0)
            {
                totalItems.Clear();
            }
        }
        // Add Item from inventory to UI
        foreach (var item in playersInventory._Inventory)
        {
            var itemUI = Instantiate(itemUIprefab, itemUIList.transform);
            totalItems.Add(itemUI);
            itemUI.SetData(item);
        }
    }
}

