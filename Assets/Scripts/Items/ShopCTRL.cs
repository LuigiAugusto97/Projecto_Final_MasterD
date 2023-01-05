using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShoppingState { Menu, Buying, Selling, Busy }
public class ShopCTRL : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] InventoryUI_CTRL inventoryUI;
    [SerializeField] MoneyCounterUI moneyCounter;
    [SerializeField] ItemCounterUI itemCounter;
    [SerializeField] ShopUI_CTRL shopUI;
    private Inventory playerInventory;

    public event Action OnStartShopping;
    public event Action OnFinishShopping;

    private ItemBase itemChosen;

    private ShoppingState state;
    public static ShopCTRL i { get; private set; }
    private ShopKeeper_CTRL currentShopKeeper;
    private void Awake()
    {
        i = this;
        playerInventory = Inventory.GetInventory();
    }
    public IEnumerator StarShopping(ShopKeeper_CTRL shopKeeper)
    {
        currentShopKeeper = shopKeeper;
        OnStartShopping?.Invoke();
        yield return HandleShopMenu(shopKeeper.inicialDialog, shopKeeper.inicialChoices);
    }

    private IEnumerator HandleShopMenu(Dialog inicialDialog, List<string> inicialChoices)
    {
        state = ShoppingState.Menu;
        int selected = 0;
        yield return DialogManager.Instance.ShowDialog(inicialDialog, inicialChoices, (choiceIndex) => selected = choiceIndex);
        if (selected == 0)
        {
            //Buy from Shop
            state = ShoppingState.Buying;
            moneyCounter.HandleMenu();
            shopUI.HandleMenu(currentShopKeeper.AvailableItems);
        }
        else if (selected == 1)
        {
            //Sell items
            state = ShoppingState.Selling;
            inventoryUI.HandleMenu();
        }
        else if (selected == 2)
        {
            //Stops Shopping
            OnFinishShopping.Invoke();
            yield break;
        }
    }

    public void HandleUpdate()
    {
        if (state == ShoppingState.Buying)
        {
            shopUI.HandleUpdate(OnBackFromBuying,() => StartCoroutine(BuyItem(currentShopKeeper.AvailableItems[shopUI.Selected])));
        }       
        else if (state == ShoppingState.Selling)
        {
            inventoryUI.HandleUpdate(OnBackFromSelling,() => StartCoroutine(SellItem(playerInventory._Inventory[inventoryUI.Selected].Item)));
        }
    }

    private void OnBackFromSelling()
    {
        inventoryUI.gameObject.SetActive(false);
        StartCoroutine(StarShopping(currentShopKeeper));
    }  
    private void OnBackFromBuying()
    {
        shopUI.gameObject.SetActive(false);
        moneyCounter.HandleMenu();
        StartCoroutine(StarShopping(currentShopKeeper));
    }
    private IEnumerator SellItem(ItemBase item)
    {
        state = ShoppingState.Busy;
        if (!item.CanSell)
        {
            yield return DialogManager.Instance.ShowDialogText("Sorry, can't sell that item.");
            state = ShoppingState.Selling;
            yield break;
        }
        else
        {
            //Recive information
            int selected = 0;
            int countToSell = 1;

            float sellingPrice = Mathf.Round(item.Price / 3);
            
            var ItemCount = playerInventory.GetItemCount(item);
            if (ItemCount > 1 )
            {
                yield return DialogManager.Instance.ShowDialogText("Choose amount to sell.", false, false);
                yield return itemCounter.ShowItemCounter(ItemCount, sellingPrice, (currentCount) => countToSell = currentCount);

                DialogManager.Instance.CloseDialog();
            }

            sellingPrice = sellingPrice * countToSell;
            yield return DialogManager.Instance.ShowDialogText($"It sells for {sellingPrice}, are you sure you want to sell?",false, true,
                new List<string>() {"Yes", "No" }, (choiceIndex) => selected = choiceIndex);
            if (selected == 0)
            {
                //Yes
                playerInventory.DecreaseItemCount(item, countToSell);
                inventoryUI.UpdateItemList();

                //Add Money
                MoneyHandler.i.AddMoney(sellingPrice);
                yield return DialogManager.Instance.ShowDialogText($"Received {sellingPrice} from selling {item.Name}");
            }
            moneyCounter.HandleMenu();
        }
        state = ShoppingState.Selling;
    }
    private IEnumerator BuyItem(ItemBase item)
    {
        state = ShoppingState.Busy;
        //Receive information
        int selected = 0;
        int countToBuy = 1;

        yield return DialogManager.Instance.ShowDialogText("Choose amount to buy.", false, false);
        yield return itemCounter.ShowItemCounter(100, item.Price, (currentCount) => countToBuy = currentCount);
        DialogManager.Instance.CloseDialog();

        float totalMoney = item.Price * countToBuy;

        if (MoneyHandler.i.CanPay(totalMoney))
        {
            yield return DialogManager.Instance.ShowDialogText($"It costs a total of {totalMoney}, are you sure you want to buy?", false, true,
            new List<string>() { "Yes", "No" }, (choiceIndex) => selected = choiceIndex);
            if (selected == 0)
            {
                //Yes
                playerInventory.AddItem(item, countToBuy);
                shopUI.UpdateItemList();

                //Add Money
                MoneyHandler.i.SpendMoney(totalMoney);
                if (countToBuy > 1 )
                {
                    yield return DialogManager.Instance.ShowDialogText($"Received x{countToBuy} {item.Name} from the purchase");
                }
                else
                {
                    yield return DialogManager.Instance.ShowDialogText($"Received {item.Name} from the purchase");
                }
            }

        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText("You can't afford that.");
        }
        state = ShoppingState.Buying;
    }
}
