using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCTRL : MonoBehaviour
{
    [Header("MenuOptions")]
    [SerializeField] GameObject menu;
    [SerializeField] List<Text> menuTxt;

    private int selected;

    public event Action<int> OnOptionChoosen;
    public event Action onMenuClose;

    public void HandleMenu()
    {
        selected = 0;
        if (menu.activeSelf == false)
        {
            menu.SetActive(true);
        }
        else if(menu.activeSelf)
        {
            menu.SetActive(false);
            onMenuClose?.Invoke();
        }
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ++selected;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            --selected;
        }
        selected = Mathf.Clamp(selected, 0, (menuTxt.Count - 1));
        HandleUIUpdate();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnOptionChoosen?.Invoke(selected);
        }
    }
    private void HandleUIUpdate()
    {
        for (int i = 0; i < menuTxt.Count; i++)
        {
            if (i== selected)
            {
                menuTxt[i].color = Color.blue;
            }
            else
            {
                menuTxt[i].color = Color.black;
            }
        }
    }
}
