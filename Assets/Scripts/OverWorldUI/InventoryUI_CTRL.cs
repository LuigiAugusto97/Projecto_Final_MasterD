using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI_CTRL : MonoBehaviour
{
    [Header("MenuOptions")]
    [SerializeField] GameObject menu;
    [SerializeField] PartyUI_CTRL partyMenu;
    [SerializeField] MoneyCounterUI moneyCounter;

    [Header("Items")]
    [SerializeField] GameObject itemUIList;
    [SerializeField] ItemUI itemUIprefab;
    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;

    [Header("list of total items")]
    private Inventory inventoryUI;
    [SerializeField] List<ItemUI> totalItems;

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
    public event Action onMenuBack;
    private InventoryUIStates state;

    private void Awake()
    {
        inventoryUI = Inventory.GetInventory();
        itemListUI_rect = itemUIList.GetComponent<RectTransform>();       
    }
    private void Start()
    {
        UpdateItemList();   
    }

 
    public void HandleMenu()
    {
        selected = 0;
        if (menu.activeSelf == false)
        {
            menu.SetActive(true);
            moneyCounter.HandleMenu();
            UpdateItemList();
        }
        else if (menu.activeSelf)
        {
            menu.SetActive(false);
            moneyCounter.HandleMenu();
            onMenuBack?.Invoke();
        }
    }
    public void HandleUpdate(Action OnBack, Action onSelected)
    {
        if(state == InventoryUIStates.ItemHandle)
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
            selected = Mathf.Clamp(selected, 0, (inventoryUI._Inventory.Count - 1));
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
        else if (state == InventoryUIStates.ChoosePartyMember)
        {
            //Choose Party Member
            Action onSelect = () => 
            {
                //Use Item
                StartCoroutine(UseItem());
            };
            Action onBack = () =>
            {
                ClosePartyUI();
            };
            partyMenu.HandleUpdate(onSelect,onBack);
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
                itemIcon.sprite = inventoryUI._Inventory[i].Item.Icon;
                itemDescription.text = inventoryUI._Inventory[i].Item.Descritption;
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

        if (selected < totalItems.Count-(itemsInView/2))
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
        foreach (var item in inventoryUI._Inventory)
        {
            var itemUI = Instantiate(itemUIprefab, itemUIList.transform);
            totalItems.Add(itemUI);
            itemUI.SetData(item);
        }
    }

    public void OpenPartyUI()
    {
        state = InventoryUIStates.ChoosePartyMember;
        menu.GetComponent<Image>().enabled = (false);
        partyMenu.gameObject.SetActive(true);
        
    }
    public void ClosePartyUI()
    {
        partyMenu.gameObject.SetActive(false);
        menu.GetComponent<Image>().enabled = (true);
        state = InventoryUIStates.ItemHandle;
        UpdateItemList();

    }

    private IEnumerator UseItem()
    {
        state = InventoryUIStates.Busy;
        if (inventoryUI._Inventory[selected].Item.Type == ItemType.OnEnemy)
        {
            yield return StartCoroutine(DialogManager.Instance.ShowDialogText($"That item is for enemies only!!"));
            ClosePartyUI();
        }
        else
        {
            var usedItem = inventoryUI.UseItem(selected, partyMenu.SelectedUnit);
            if (usedItem != null)
            {
                //CReat A field to specefy the context n55
                inventoryUI.DecreaseItemCount(usedItem);
                yield return StartCoroutine(DialogManager.Instance.ShowDialogText($"Used {usedItem.Name} on {partyMenu.SelectedUnit.UBase.Name}"));
                partyMenu.SelectedUnit.isRevived = false;
                ClosePartyUI();
            }
            else if (usedItem == null)
            {
                yield return StartCoroutine(DialogManager.Instance.ShowDialogText($"It cannot be used right now.."));
                state = InventoryUIStates.ChoosePartyMember;
            }
        }
    }
}
public enum InventoryUIStates { ItemHandle, ChoosePartyMember, Busy}
