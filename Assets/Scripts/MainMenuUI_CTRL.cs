using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MenuStates { MainMenu, NewGame, LoadGame, Options}
public class MainMenuUI_CTRL : MonoBehaviour
{
    [Header("Menu Settings")]
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject newGameConfirmation;
    [SerializeField] GameObject loadGameConfirmation;

    [Header("List Settings")]
    [SerializeField] List<Text> mainMenuTextList;
    [SerializeField] List<Text> newGameTextList;
    [SerializeField] List<Text> loadGameTextList;

    public int selected;
    private int otherSelected;
    public MenuStates states;
    public static MainMenuUI_CTRL i { get; private set; }
    private void Awake()
    {
        i = this;
        states = MenuStates.MainMenu;
        selected = 0;
    }

    // UI HANDLE
    public void ForceUpdate()
    {
        if (states == MenuStates.MainMenu)
        {
            HandleUpdate(MainMenuOnSelected, mainMenuTextList);
        }
        else if (states == MenuStates.NewGame)
        {
            HandleUpdate(NewGameOnSelected, newGameTextList);
        }
        else if (states == MenuStates.LoadGame)
        {
                HandleUpdate(LoadGameOnSelected, loadGameTextList);
        }
    }
    private void HandleUpdate( Action onSelected, List<Text> menuList)
    {
        //Choose menu Option
        if (states == MenuStates.MainMenu)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                ++selected;
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                --selected;
            }
            selected = Mathf.Clamp(selected, 0, menuList.Count - 1);
            HandleUIUpdate(menuList);
            if (Input.GetKeyUp(KeyCode.Space))
            {
                onSelected?.Invoke();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                ++selected;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                --selected;
            }
            selected = Mathf.Clamp(selected, 0, menuList.Count - 1);
            HandleUIUpdate(menuList);
            if (Input.GetKeyUp(KeyCode.Space))
            {
                onSelected?.Invoke();
            }
        }

    }

    private void HandleUIUpdate(List<Text> menuList)
    {
        for (int i = 0; i < menuList.Count; i++)
        {
            if (i == selected)
            {
                menuList[i].color = Color.blue;
            }
            else
            {
                if (states == MenuStates.MainMenu)
                {
                    menuList[i].color = Color.white;
                }
                else
                {
                    menuList[i].color = Color.black;
                }

            }
        }
    }

    //Handle options
    private void MainMenuOnSelected()
    {
        if (selected == 0)
        {
            //new game
            //mainMenu.SetActive(false);
            newGameConfirmation.SetActive(true);
            selected = 0;
            states = MenuStates.NewGame;
        }
        else if (selected == 1)
        {
            //Load 
            //mainMenu.SetActive(false);
            loadGameConfirmation.SetActive(true);
            selected = 0;
            states = MenuStates.LoadGame;

        }
        else if (selected == 2)
        {
            Application.Quit();
        }
    }

    private void NewGameOnSelected()
    {
        if (selected == 0)
        {
            GameCTRL.Instance.LoadGameplayScene(false);
        }
        if (selected == 1)
        {
            selected = 0;
            mainMenu.SetActive(true);
            newGameConfirmation.SetActive(false);
            states = MenuStates.MainMenu;
        }
    }
    private void LoadGameOnSelected()
    {
        if (selected == 0)
        {
            GameCTRL.Instance.LoadGameplayScene(true);
        }
        if (selected == 1)
        {
            selected = 0;
            mainMenu.SetActive(true);
            loadGameConfirmation.SetActive(false);
            states = MenuStates.MainMenu;
        }
    }
}
