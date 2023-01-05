using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyCounterUI : MonoBehaviour
{
    [SerializeField] Text moneyTxt;
    private void Awake()
    {
        MoneyHandler.i.onMoneyChanged += SetMoneyUI;
    }
    public void HandleMenu()
    {
        if (gameObject.activeSelf == false)
        {
            gameObject.SetActive(true);
            SetMoneyUI();
        }
        else if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    private void SetMoneyUI()
    {
        moneyTxt.text = MoneyHandler.i.Money + "$";
    }


}
