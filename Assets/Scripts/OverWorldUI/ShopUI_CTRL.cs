using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI_CTRL : MonoBehaviour
{
    [Header("Items")]
    [SerializeField] GameObject itemUIList;
    [SerializeField] ItemUI itemShopUIprefab;
    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;

    [Header("list of total items")]
    [SerializeField] List<ItemUI> totalItems;
    private List<ItemBase> shopsItems;

    [Header("Scrolling")]
    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;
    private RectTransform itemListUI_rect;
    private const int itemsInView = 7;

    //UIHandle
    private int selected;
    public int Selected
    {
        get { return selected; }
    }
    private void Awake()
    {
        itemListUI_rect = itemUIList.GetComponent<RectTransform>();
    }
    public void HandleMenu(List<ItemBase> shopsItems)
    {
        this.shopsItems = shopsItems;
        gameObject.SetActive(true);
        UpdateItemList();
    }
    public void HandleUpdate(Action OnBack= null, Action onSelected = null)
    {
        //Choose Item
        if (Input.GetKeyDown(KeyCode.S))
        {
            ++selected;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            --selected;
        }
        selected = Mathf.Clamp(selected, 0, (shopsItems.Count - 1));
        HandleUIUpdate();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            onSelected?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            OnBack?.Invoke();
        }
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
        foreach (var item in shopsItems)
        {
            var itemUI = Instantiate(itemShopUIprefab, itemUIList.transform);
            totalItems.Add(itemUI);
            itemUI.SetShopData(item);
        }
    }
    private void HandleUIUpdate()
    {
        HandleUIScrolling();
        for (int i = 0; i < totalItems.Count; i++)
        {
            if (i == selected)
            {
                totalItems[i].Indecate(true);
                itemIcon.sprite = shopsItems[i].Icon;
                itemDescription.text = shopsItems[i].Descritption;
            }
            else
            {
                totalItems[i].Indecate(false);
            }
        }
    }

    private void HandleUIScrolling()
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
}
