using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStates { MainMenu,  FreeRoam  , Battle, Dialog, Cutscene, Menu, PartySelection, InventoryManagment, Shop ,Paused}
public class GameCTRL : MonoBehaviour
{
    [Header("Battler")]
    [SerializeField] Player_CTRL player;
    [SerializeField] BattleHandler battleHandler;
    [SerializeField] Camera worldCamera;
    [Header("Menus")]
    [SerializeField] MenuCTRL defaultMenu;
    [SerializeField] PartyUI_CTRL partyUIMenu;
    [SerializeField] InventoryUI_CTRL inventoryUIMenu;
    [SerializeField] AudioClip mainMenuTheme;

    //Game states
    private GameStates state;
    private GameStates beforestate;

    //Scene References
    public SceneLoader currentScene { get; private set; }
    public SceneLoader PreviousScene { get; private set; }


    public static GameCTRL Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        //get all SO
        ItemsDB.GetItems();
        Unit_MovesDB.GetMoves();
        UnitsBaseDB.GetUnits();
        QuestDB.GetQuests();
        ConditionsDB.GetStatusConditions();
    }
    private void Start()
    {
        AudioManager.i.PlayMusic(mainMenuTheme);
        battleHandler.OnBattleOver += EndBattle;
        battleHandler.OnRanAway += RanfromBattle;

        defaultMenu.onMenuClose += () => { state = GameStates.FreeRoam; };
        defaultMenu.OnOptionChoosen += DefaultMenuActionSelected;

        partyUIMenu.onMenuBack += () => { state = GameStates.Menu; };

        inventoryUIMenu.onMenuBack += () => { state = GameStates.Menu; };

        DialogManager.Instance.OnShowDialog += ShowDialog;
        DialogManager.Instance.OnCloseDialog  += CloseDialog;

        ShopCTRL.i.OnStartShopping += () => { state = GameStates.Shop; };
        ShopCTRL.i.OnFinishShopping += () => { state = GameStates.FreeRoam; };

    }
    
    //Handle game states
    private void Update()
    {
        if (state == GameStates.MainMenu)
        {
            MainMenuUI_CTRL.i.ForceUpdate();
        }                
        else if (state == GameStates.FreeRoam)
        {
            player.HandleUpdate();

            if (Input.GetKeyDown(KeyCode.Return))
            {
                defaultMenu.HandleMenu();
                player.Char.NPCANIM.isMoving = false;
                state = GameStates.Menu;
            }
        }
        else if (state == GameStates.Battle)
        {
            battleHandler.HandleUpdate();
        }
        else if(state == GameStates.Dialog)
        {
            player.Char.NPCANIM.isMoving = false;
            DialogManager.Instance.HandleUpdate();
        }
        else if (state == GameStates.Menu)
        {
            defaultMenu.HandleUpdate();
            if (Input.GetKeyDown(KeyCode.Return))
            {
                defaultMenu.HandleMenu();
            }         
        }
        else if (state == GameStates.PartySelection)
        {
            Action onSelect = () => { partyUIMenu.ChangePartyMember(); };
            Action onBack = () => { partyUIMenu.HandleMenu(); };
            partyUIMenu.HandleUpdate(onSelect,onBack); 
        }
        else if (state == GameStates.InventoryManagment)
        {
            inventoryUIMenu.HandleUpdate(inventoryUIMenu.HandleMenu, inventoryUIMenu.OpenPartyUI);  
        }
        else if (state == GameStates.Shop)
        {
            ShopCTRL.i.HandleUpdate();  
        }

    }

    //Dialog
    private void ShowDialog()
    {
        beforestate = state;
        state = GameStates.Dialog;
    }
    private void CloseDialog()
    {
        if (state == GameStates.Dialog)
        {
            state = beforestate;
        }
    }
    public void PauseGame(bool pause)
    {
        if (pause)
        {
            beforestate = state;
            state = GameStates.Paused;
        }
        else
        {
            state = beforestate;
        }
    }

    //Battle Settings
    public void StartBattle()
    {

        state = GameStates.Battle;
        SavingSystem.i.Save("SaveBeforeFight");
        battleHandler.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var PlayerParty = player.GetComponent<PartySystem>();
        var EnemyParty = currentScene.GetComponent<EnemiesInTheArea>();

        battleHandler.StartBattle(PlayerParty,EnemyParty);
    }
    public void StartBossBattle(Boss_CTRL boss_unit)
     {
         state = GameStates.Battle;
        SavingSystem.i.Save("SaveBeforeFight");
        battleHandler.gameObject.SetActive(true);
         worldCamera.gameObject.SetActive(false);

         var PlayerParty = player.GetComponent<PartySystem>();
         var BossParty = boss_unit.GetComponent<PartySystem>();

        battleHandler.StartBossBattle(PlayerParty, BossParty) ;
     }
    public void EndBattle(bool won)
    {
        if (won)
        {
            state = GameStates.FreeRoam;
            battleHandler.gameObject.SetActive(false);
            worldCamera.gameObject.SetActive(true);
            battleHandler._isBossBattle = false;
            AudioManager.i.PlayMusic(currentScene.MusicOfScene, fadeMusic: true);
        }
        else if (!won)
        {
            StartCoroutine(battleEndLoose());
        }

    }

    IEnumerator battleEndLoose()
    {
        state = GameStates.FreeRoam;
        yield return Fader.i.FadeIN(0.5f);
        battleHandler.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
        battleHandler._isBossBattle = false;
        SavingSystem.i.Load("SaveBeforeFight");
        yield return Fader.i.FadeOUT(1.2f);
        AudioManager.i.PlayMusic(currentScene.MusicOfScene, fadeMusic: true);
        yield return (DialogManager.Instance.ShowDialogText("How long was i out?"));
        var prevMoney = MoneyHandler.i.Money;
        MoneyHandler.i.DeathPay();
        yield return (DialogManager.Instance.ShowDialogText($"You lost {(prevMoney - MoneyHandler.i.Money)}$"));
    }
    public void RanfromBattle()
    {
            state = GameStates.FreeRoam;
            battleHandler.gameObject.SetActive(false);
            worldCamera.gameObject.SetActive(true);
            battleHandler._isBossBattle = false;
            AudioManager.i.PlayMusic(currentScene.MusicOfScene, fadeMusic: true);
    }

    //Scnese
    public void SetCurrentScene(SceneLoader currtScene)
    {
        PreviousScene = currentScene;
        currentScene = currtScene;
    }
    public bool StopPlayingMusic()
    {
        if (PreviousScene == null || !PreviousScene.DontStopMusic)
        {
            return true;
        }
        else if(PreviousScene.DontStopMusic)
        {
            return false;
        }
        return false;
    }

    //UI
    public void DefaultMenuActionSelected(int selected)
    {
        if (selected == 0)
        {
            //party
            partyUIMenu.HandleMenu();            
            partyUIMenu.SetParty();
            state = GameStates.PartySelection;
        }
        else if (selected == 1)
        {
            //Items
            inventoryUIMenu.HandleMenu();
            state = GameStates.InventoryManagment;

        }
        else if (selected == 2)
        {
            //Save
            defaultMenu.HandleMenu();
            SavingSystem.i.Save("PrimarySave");
            StartCoroutine(DialogManager.Instance.ShowDialogText("Successfully saved the game!"));
        }
        else if (selected == 3)
        {
            //Load
            defaultMenu.HandleMenu();
            SavingSystem.i.Load("PrimarySave");
            StartCoroutine(DialogManager.Instance.ShowDialogText("Loaded saved file successfully!"));
        }
    }


    //Main Menu Handle
    public void LoadGameplayScene(bool hasSavedFile)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Gameplay", UnityEngine.SceneManagement.LoadSceneMode.Single);
        if (hasSavedFile)
        {
            //IF load
            SavingSystem.i.Load("PrimarySave");
            state = GameStates.FreeRoam;
            StartCoroutine(DialogManager.Instance.ShowDialogText("Loaded saved file successfully!"));
            player.Char.SetToTile(player.transform.position);
        }
        else
        {
            //If new game
            state = GameStates.FreeRoam;
            player.Char.SetToTile(player.transform.position);
            SavingSystem.i.Save("PrimarySave");
        }
    }
}
