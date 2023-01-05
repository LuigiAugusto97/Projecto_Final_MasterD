using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField] Text itemName;
    [SerializeField] Text itemCount;
    [SerializeField] Image indecator;

    //For Scrolling options
    private RectTransform itemUI_rect;
    public float ListBoxHeight
    {
        get { return itemUI_rect.rect.height; }
    }

    private bool isSelected;
    private void Awake()
    {
        itemUI_rect = GetComponent<RectTransform>();
    }
    public void SetData(ItemData itemUI)
    {
        itemName.text = itemUI.Item.Name;
        itemCount.text = $"x {itemUI.ItemCount}";
    }
    public void SetShopData(ItemBase shopItem)
    {
        itemName.text = shopItem.Name;
        itemCount.text = $"{shopItem.Price}$";
    }

    public void Indecate(bool selected)
    {
        if (isSelected)
        {
            indecator.color = Color.green;
            indecator.gameObject.SetActive(true);
        }
        else
        {
            indecator.color = Color.black;
            if (selected)
            {
                indecator.gameObject.SetActive(true);
            }
            else
            {
                indecator.gameObject.SetActive(false);
            }
        }

    }
}
