using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCounterUI : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] Text countTxt;
    [SerializeField] Text moneyTxt;

    private bool isSelected;
    private int currentCount;

    //to use outside the courotine
    private int _maxAmount;
    private float _moneyperItem;
    public IEnumerator ShowItemCounter(int maxAmount, float moneyPerItem, Action<int> onCountSelected)
    {
        _maxAmount = maxAmount;
        _moneyperItem = moneyPerItem;
        isSelected = false;
        currentCount = 1;
        gameObject.SetActive(true);
        HandleUIUpdate();

        yield return new WaitUntil(() => isSelected == true);

        onCountSelected?.Invoke(currentCount);
        gameObject.SetActive(false);
    }
    private void Update()
    {
        int prevCount = currentCount;
        if (Input.GetKeyDown(KeyCode.W))
        {
            currentCount++;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            currentCount--;
        }
        currentCount = Mathf.Clamp(currentCount, 1 , _maxAmount);

        if (currentCount != prevCount)
        {
            HandleUIUpdate();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isSelected = true;
        }
    }

    private void HandleUIUpdate()
    {
        countTxt.text = "x" + currentCount;
        moneyTxt.text = _moneyperItem * currentCount + "$";
    }
}
