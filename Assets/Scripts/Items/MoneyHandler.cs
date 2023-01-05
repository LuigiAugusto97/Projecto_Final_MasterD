using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyHandler : MonoBehaviour, ISavable
{
    [SerializeField] float money;
    public float Money
    {
        get { return money; }
        set { money = value; }
    }
    public static MoneyHandler i { get; private set; }

    public event Action onMoneyChanged;

    private void Awake()
    {
        i = this;
    }
    public void AddMoney(float amount)
    {
        money += amount;
        onMoneyChanged?.Invoke();
    }
    public void SpendMoney(float amount)
    {
        money -= amount;
        onMoneyChanged?.Invoke(); 
    }

    public bool CanPay(float amount)
    {
        return amount <= money;
    }
    public void DeathPay()
    {
        money = money - (money / 10);
    }

    //SAVE SYSTEM
    public object SaveData()
    {
        return money;
    }

    public void LoadData(object data)
    {
        money = (float)data;
    }
}
