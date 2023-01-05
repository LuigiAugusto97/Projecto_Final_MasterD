using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleStates { START, INITIALACTION, PHYSICALMOVES, MAGICALMOVES, CHOOSEITEMS, ENEMYCHOOSE, FRIENDLYCHOOSE, ATTACKING, RUN, FORGETTINGMOVE , WON, LOSE, BUSY }
public class BattleHandler : MonoBehaviour
{
    private BattleStates states;
    [SerializeField] BattleUIText UIText;

    [Header("Teams")]
    [SerializeField] List<BattleUnit> PlayerTeam;
    [SerializeField] List<BattleUnit> DeadPlayerTeam;
    [SerializeField] List<BattleUnit> EnemyTeam;
    [SerializeField] List<BattleUnit> DeadEnemyTeam;

    [SerializeField] AttackMovesBase CurrentMove;

    public event Action<bool> OnBattleOver;
    public event Action OnRanAway;

    [SerializeField] Image backGround;

    [Header("Players Settings")]
    private Player_CTRL player;
    
    public BattleUnit playerUnit_1;
    public BattleUnit playerUnit_2;
    public BattleUnit playerUnit_3;
    private PartySystem playerParty;

    [Header("Enemys Settings")]
    public BattleUnit enemyUnit_1;
    public BattleUnit enemyUnit_2;
    public BattleUnit enemyUnit_3;  
    private EnemiesInTheArea enemyParty;
    
    private Boss_CTRL boss;
    private PartySystem bossParty;
    
    [Header("Move Learning")]
    [SerializeField] AttackMovesBase _moveToLearn;
    [SerializeField] BattleUnit currentPlayer;

    [Header("Items")]
    [SerializeField] BattleInventoryUI inventoryUI;

    //Other
    private int TotalExpGained = 0;
    private float TotalMoneyGained = 0;
    public bool _isBossBattle = false;
    private int _escapeAttemps = 0;



    //Move Selection
    private int currentAction = 0;
    public int currentSelected = 0;
    private bool usingItem = false;


    //Battle Start
    public void StartBattle(PartySystem playerparty, EnemiesInTheArea enemyparty)
    {
        this.playerParty = playerparty;
        player = playerparty.GetComponent<Player_CTRL>();
        enemyparty.Setup();
        this.enemyParty = enemyparty;
        backGround.sprite = enemyparty.backGround;
        states = BattleStates.START;
        UIText.EnableInitalBox(false);
        StartCoroutine(SetupBattle());
    }
    public void StartBossBattle(PartySystem playerparty, PartySystem Bossparty)
    {
        this.playerParty = playerparty;
        this.bossParty = Bossparty;
        states = BattleStates.START;
        UIText.EnableInitalBox(false);


        player = playerparty.GetComponent<Player_CTRL>();
        boss = Bossparty.GetComponent<Boss_CTRL>();
        backGround.sprite = boss.background;
        _isBossBattle = true;
        StartCoroutine(SetupBattle());
    }


    IEnumerator SetupBattle()
    {
        currentAction = 0;
        currentSelected = 0;
        playerUnit_1.Setup(playerParty.GetPlayer(0));
        AddPTeam(playerUnit_1);

        playerUnit_2.Setup(playerParty.GetPlayer(1));

        if (playerUnit_2.Unit == null)
        {
            playerUnit_2.playerUI.TakeExpAway();
            playerUnit_2.playerUI.gameObject.SetActive(false);
        }
        else
        {
            AddPTeam(playerUnit_2);
            playerUnit_2.playerUI.gameObject.SetActive(true);
        }


        playerUnit_3.Setup(playerParty.GetPlayer(2));

        if (playerUnit_3.Unit == null)
        {
            playerUnit_3.playerUI.TakeExpAway();
            playerUnit_3.playerUI.gameObject.SetActive(false);
        }
        else
        {
            AddPTeam(playerUnit_3);
            playerUnit_3.playerUI.gameObject.SetActive(true);
        }


        if (!_isBossBattle)
        {
            //Play battle music of Scene

            AudioManager.i.PlayMusic(GameCTRL.Instance.currentScene.MusicOfBattle, fadeMusic: true);

            float moreEnemies = UnityEngine.Random.Range(0f, 1f);
            enemyUnit_1.Setup(enemyParty.Enemy1);
            EnemyTeam.Add(enemyUnit_1);
            enemyUnit_1.enemyUI.SetEnemy(enemyUnit_1.Unit);

            enemyUnit_2.Setup(enemyParty.Enemy2);
            EnemyTeam.Add(enemyUnit_2);
            enemyUnit_2.enemyUI.SetEnemy(enemyUnit_2.Unit);

            enemyUnit_3.Setup(enemyParty.Enemy3);
            EnemyTeam.Add(enemyUnit_3);
            enemyUnit_3.enemyUI.SetEnemy(enemyUnit_3.Unit);


            if (moreEnemies >= 0.3f)
            {
                EnemyTeam.Remove(enemyUnit_2);
                enemyUnit_2.enemyUI.ZeroEnemy();
            }
            if (moreEnemies >= 0.15f)
            {
                EnemyTeam.Remove(enemyUnit_3);
                enemyUnit_3.enemyUI.ZeroEnemy();
            }
        }
        else
        {
            //Play Boss Battle Music

            AudioManager.i.PlayMusic(boss.BossBattleMusic, fadeMusic: true);

            enemyUnit_1.Setup(bossParty.GetPlayer(0));
            EnemyTeam.Add(enemyUnit_1);
            enemyUnit_1.enemyUI.SetEnemy(enemyUnit_1.Unit);

            enemyUnit_2.Setup(bossParty.GetPlayer(1));
            if (enemyUnit_2.Unit.isdead == false)
                EnemyTeam.Add(enemyUnit_2);
            
            enemyUnit_2.enemyUI.SetEnemy(enemyUnit_2.Unit);

            enemyUnit_3.Setup(bossParty.GetPlayer(2));
            if (enemyUnit_3.Unit.isdead == false)
                EnemyTeam.Add(enemyUnit_3);

            enemyUnit_3.enemyUI.SetEnemy(enemyUnit_3.Unit);
        }
        



        yield return StartCoroutine(UIText.TypeDialog("The Battle Starts"));        
        StartCoroutine(Player1Turn());
    }

    //PLAYERS TURN
    IEnumerator Player1Turn()
    {
        currentPlayer = playerUnit_1;
        if (DeadPlayerTeam.Contains(playerUnit_1))
        {
            yield return new WaitForSeconds(.1f);
            CheckWhoNext();
        }
        else
        {
            UIText.EnableInitalBox(true);
            UIText.ResetMoves();
            UIText.SetMoves(playerUnit_1.Unit.Unit_Physical_Moves);
            UIText.SetMoves(playerUnit_1.Unit.Unit_Magical_Moves);
            playerUnit_1.playerUI.ChangeSelectedColor(Color.green);
            yield return StartCoroutine(UIText.TypeDialog("Choose an Action!"));
            states = BattleStates.INITIALACTION;
        }
    }
    IEnumerator Player2Turn()
    {
        currentPlayer = playerUnit_2;

        //CHECK FOR BETTER SOLUCION
        if (playerUnit_2.Unit == null)
        {
            yield return StartCoroutine(Player3Turn());
        }
        else
        {
             if (DeadPlayerTeam.Contains(playerUnit_2))
            {
                yield return new WaitForSeconds(.1f);
                CheckWhoNext();
            }
            else
            {
                UIText.EnableInitalBox(true);
                UIText.ResetMoves();
                UIText.SetMoves(playerUnit_2.Unit.Unit_Physical_Moves);
                UIText.SetMoves(playerUnit_2.Unit.Unit_Magical_Moves);
                playerUnit_2.playerUI.ChangeSelectedColor(Color.green);
                yield return StartCoroutine(UIText.TypeDialog("Choose an Action!"));
                states = BattleStates.INITIALACTION;
            }
        }

    }
    IEnumerator Player3Turn()
    {
        currentPlayer = playerUnit_3;
        if (playerUnit_3.Unit == null)
        {
            yield return StartCoroutine(EnemyTurn1());
        }
        else if (DeadPlayerTeam.Contains(playerUnit_3))
        {
            yield return new WaitForSeconds(.1f);
            CheckWhoNext();
        }
        else
        {
            UIText.EnableInitalBox(true);
            UIText.ResetMoves();
            UIText.SetMoves(playerUnit_3.Unit.Unit_Physical_Moves);
            UIText.SetMoves(playerUnit_3.Unit.Unit_Magical_Moves);
            playerUnit_3.playerUI.ChangeSelectedColor(Color.green);
            yield return StartCoroutine(UIText.TypeDialog("Choose an Action!"));
            states = BattleStates.INITIALACTION;
        }
    }

    //ENEMY TURN
    IEnumerator EnemyTurn1()
    {
        currentPlayer = enemyUnit_1;
        if (!DeadEnemyTeam.Contains(enemyUnit_1) && EnemyTeam.Contains(enemyUnit_1))
        {
            CurrentMove = enemyUnit_1.Unit.GetRandomMove().AMBase;
            var target = GetPossibleTarget();
            yield return StartCoroutine(UseMove(enemyUnit_1,target));
        }
        else
        {
            CheckWhoNext();
        }
    }
    IEnumerator EnemyTurn2()
    {
        currentPlayer = enemyUnit_2;
        if (!DeadEnemyTeam.Contains(enemyUnit_2) && EnemyTeam.Contains(enemyUnit_2))
        {
            CurrentMove = enemyUnit_2.Unit.GetRandomMove().AMBase;
            var targets = GetPossibleTarget();
            yield return StartCoroutine(UseMove(enemyUnit_2, targets));
        }
        else
        {
            CheckWhoNext();
        }
    }
    IEnumerator EnemyTurn3()
    {
        currentPlayer = enemyUnit_3;
        if (!DeadEnemyTeam.Contains(enemyUnit_3) && EnemyTeam.Contains(enemyUnit_3))
        {
            CurrentMove = enemyUnit_3.Unit.GetRandomMove().AMBase;
            var targets = GetPossibleTarget();
            yield return StartCoroutine(UseMove(enemyUnit_3, targets));
        }
        else
        {
            CheckWhoNext();
        }
    }


    //ATACKS
    IEnumerator UseMove(BattleUnit attacker, BattleUnit defender)
    {
        states = BattleStates.ATTACKING;
        bool canUseMove = attacker.Unit.OnBeforeMove();
        if (!canUseMove)
        {
            yield return ShowStatusChanges(attacker.Unit);
            UpdateAllHPandMP();  
            CheckWhoNext();
            yield break;
        }
        yield return ShowStatusChanges(attacker.Unit);

        var move = new Unit_Moves(CurrentMove);
        //Apply mana costs
        attacker.Unit.Mana -= move.AMBase.ResourceCost;
        yield return StartCoroutine(UIText.TypeDialog($"{attacker.Unit.UBase.Name} used {move.AMBase.Name} on {defender.Unit.UBase.Name}"));

        if (CheckIfMoveHits(move, attacker.Unit, defender.Unit))
        {
            //Play ANimation

            //PLay MoveAudio
            AudioManager.i.PlaySFX(move.AMBase.MoveSound);

            // Apply a Status
            if (move.AMBase.MovDescriv == MoveDescriver.Status)
            {             
                yield return StartCoroutine(ApplyStatusMove(move.AMBase.Effects, attacker.Unit, defender.Unit, move.AMBase.Target));
            }
            //Heal a FriendlyUnit
            else if (move.AMBase.MovDescriv == MoveDescriver.Heal)
            {
                var healDetails = attacker.Unit.HealOther(move, defender.Unit);
                UpdateAllHPandMP();
                yield return StartCoroutine(UIText.TypeDialog($"Healing for {healDetails.HealAmount} health point leaving him with {healDetails.TargetHP} HP"));
            }
            //Revive a FriendlyUnit
            else if (move.AMBase.MovDescriv == MoveDescriver.Revive)
            {
                defender.Unit.isdead = false;
                if (move.AMBase.Power <= 50)
                {
                    defender.Unit.HealDamage(defender.Unit.MaxHP / 2);
                }
                else if( move.AMBase.Power > 50 && move.AMBase.Power <= 100)
                {
                    defender.Unit.HealDamage(defender.Unit.MaxHP);
                }
                else if(move.AMBase.Power > 100)
                {
                    defender.Unit.FullHeal();
                }
                DeadPlayerTeam.Remove(defender);
                UpdateAllHPandMP();
                defender.playerUI.SetPlayer(defender.Unit);
                yield return StartCoroutine(UIText.TypeDialog($"{defender.Unit.UBase.Name} is back to action"));
            }
            else if (move.AMBase.MovDescriv == MoveDescriver.Cure)
            {
                if (defender.Unit.Status.Id == move.AMBase.Effects.Status)
                    defender.Unit.CureStatus();
                else if (defender.Unit.VolatileStatus.Id == move.AMBase.Effects.Status)
                    defender.Unit.CureVolatileStatus();
                else
                    yield return StartCoroutine(UIText.TypeDialog($"It did nothing"));

                yield return StartCoroutine(ShowStatusChanges(defender.Unit));
            }
            //Deal Normal Damage
            else if (move.AMBase.MovDescriv == MoveDescriver.Normal)
            {
                AudioManager.i.PlaySFX(AudioId.GettingHit);
                var damagedetails = attacker.Unit.DealDamage(move, defender.Unit);
                UpdateAllHPandMP();
                yield return StartCoroutine(ShowDamageDetails(damagedetails));
                yield return StartCoroutine(UIText.TypeDialog($"It did {damagedetails.Dmg} amount of damage"));
            }

            //Apply the secondary effects
            if (move.AMBase.SecondaryEffects != null && move.AMBase.SecondaryEffects.Count > 0 && defender.Unit.HP > 0)
            {
                //Loop all the various effects
                foreach (var secondary in move.AMBase.SecondaryEffects)
                {
                    //Check for chance to apply
                    var rndchance = UnityEngine.Random.Range(1, 101);
                    if (rndchance <= secondary.Chance)
                    {
                        yield return StartCoroutine(ApplyStatusMove(secondary, attacker.Unit, defender.Unit,secondary.Target));
                    }
                }            
            }
        }
        else
        {
            yield return StartCoroutine(UIText.TypeDialog($"{attacker.Unit.UBase.Name}'s attack missed"));
        }

        CurrentMove = null;
        if (defender.Unit.isdead)
        {
            CheckIfDead(defender);
            yield return StartCoroutine(UIText.TypeDialog($"{defender.Unit.UBase.Name} is dead"));
            yield return StartCoroutine(CheckForEndBattle());
            attacker.Unit.OnAfterTurn();
            UpdateAllHPandMP();
            yield return StartCoroutine(ShowStatusChanges(attacker.Unit));
            CheckWhoNext();
        }
        else
        {
            attacker.Unit.OnAfterTurn();
            UpdateAllHPandMP();
            yield return StartCoroutine(ShowStatusChanges(attacker.Unit));
            //Checkar se a unidade morreu com o dano feito por status
            if (attacker.Unit.HP == 0)
            {
                attacker.Unit.isdead = true;
                CheckIfDead(attacker);
                yield return StartCoroutine(UIText.TypeDialog($"{attacker.Unit.UBase.Name} is dead"));
                yield return StartCoroutine(CheckForEndBattle());
                UpdateAllHPandMP();
                CheckWhoNext();
            }
            else
            {
                CheckWhoNext();
            }

                
        }
    }

    IEnumerator UseItem(int selected, BattleUnit unit)
    {
        states = BattleStates.BUSY;
        var prevHP = unit.Unit.HP;
        var usedItem = inventoryUI.playersInventory.UseItem(selected, unit.Unit);
        if (usedItem != null)
        {
            //CReat A field to specefy the context n55

            //Play Audio for using Battle Item
            AudioManager.i.PlaySFX(AudioId.ItemBattle);
            yield return StartCoroutine(UIText.TypeDialog($"Used {usedItem.Name} on {unit.Unit.UBase.Name}"));
            inventoryUI.playersInventory.DecreaseItemCount(usedItem);
            inventoryUI.UpdateItemList();
            yield return StartCoroutine(ShowStatusChanges(unit.Unit));
            UpdateAllHPandMP();
            if (inventoryUI.playersInventory._Inventory[selected].Item.Type == ItemType.OnEnemy)
            {
                if (prevHP != unit.Unit.HP)
                {
                    AudioManager.i.PlaySFX(AudioId.GettingHit);
                    yield return StartCoroutine(UIText.TypeDialog($"{unit.Unit.UBase.Name} took {(prevHP - unit.Unit.HP)} amount of damage"));
                }
                usingItem = false;
            }
            if (unit.Unit.isRevived)
            {
                DeadPlayerTeam.Remove(unit);
                UpdateAllHPandMP();
                unit.playerUI.SetPlayer(unit.Unit);
                yield return StartCoroutine(UIText.TypeDialog($"{unit.Unit.UBase.Name} is back to action"));
                unit.Unit.isRevived = false;
                usingItem = false;
                CheckWhoNext();
            }
            else if (unit.Unit.HP == 0)
            {
                unit.Unit.isdead = true;
                CheckIfDead(unit);

                yield return StartCoroutine(UIText.TypeDialog($"{unit.Unit.UBase.Name} is dead"));
                yield return StartCoroutine(CheckForEndBattle());
                usingItem = false;
                CheckWhoNext();

            }
            else
            {
                usingItem = false;
                CheckWhoNext();
            }

        }
        else if (usedItem == null)
        {
            yield return StartCoroutine(UIText.TypeDialog("It cannot be used right now.."));
            UIText.ItemBox.SetActive(true);
            usingItem = false;
            states = BattleStates.CHOOSEITEMS;
        }
    }

    bool CheckIfMoveHits(Unit_Moves move, Units source, Units target)
    {

        if (move.AMBase.AlwaysHit)
        {
            return true;
        }
        float moveAccuracy = CurrentMove.Accuracy;

        int accuracy = source.StatBost[Stat.Accuracy];
        int evasion = source.StatBost[Stat.Evasion];

        //Boost Values for move accuracy 
        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        //Change accuraccy acording to stat boosting or base accuracy
        if (accuracy > 0)
        {
            moveAccuracy *= boostValues[accuracy];
        }
        else if (accuracy < 0)
        {
            moveAccuracy /= boostValues[-accuracy];
        }

        //Change accuraccy acording to stat boosting or base evasion
        if (evasion > 0)
        {
            moveAccuracy /= boostValues[evasion];
        }
        else if (evasion < 0)
        {
            moveAccuracy *= boostValues[-evasion];
        }


        return (UnityEngine.Random.Range(1, 101) <= moveAccuracy);               
    }
    IEnumerator ApplyStatusMove(MoveEffects effects, Units source, Units target,MoveTarget moveTarget)
    {
        if (moveTarget == MoveTarget.Self)
        {
            //Aumentar ou diminuir os Stats
            if (effects.Boosts != null)
            {
                source.ApplyBoosts(effects.Boosts);
                UpdateAllHPandMP();
                yield return StartCoroutine(ShowStatusChanges(source));
            }
            //Aplicar um Status effect
            if (effects.Status != ConditionID.None)
            {
                source.SetStatus(effects.Status);
                yield return StartCoroutine(ShowStatusChanges(source));
            }
            //Aplicar um VolatileStatus effect
            if (effects.VolatileStatus != ConditionID.None)
            {
                source.SetVolatileStatus(effects.VolatileStatus);
                yield return StartCoroutine(ShowStatusChanges(source));
            }
        }
        else
        {
            //Aumentar ou diminuir os Stats
            if (effects.Boosts != null)
            {
                target.ApplyBoosts(effects.Boosts);
                UpdateAllHPandMP();
                yield return StartCoroutine(ShowStatusChanges(target));
            }
            //Aplicar um Status effect
            if (effects.Status != ConditionID.None)
            {
                target.SetStatus(effects.Status);
                yield return StartCoroutine(ShowStatusChanges(target));
            }
            //Aplicar um VolatileStatus effect
            if (effects.VolatileStatus != ConditionID.None)
            {
                target.SetVolatileStatus(effects.VolatileStatus);
                yield return StartCoroutine(ShowStatusChanges(target));
            }
        }
    }
    IEnumerator ShowStatusChanges(Units unit)
    {
        if (unit.StatusChange != null)
        {
            while (unit.StatusChange.Count > 0)
            {
                var message = unit.StatusChange.Dequeue();
                yield return StartCoroutine(UIText.TypeDialog(message));
            }
        }
    }
    IEnumerator ShowDamageDetails(DamageDetails details)
    {
        if (details.TypeEfect > 1f)
        {
            if (details.CriticalDamage > 1f)
            {
                yield return StartCoroutine(UIText.TypeDialog($"A critical hit!!"));
            }

            yield return StartCoroutine(UIText.TypeDialog($"It was super effective!!"));
        }
        else if (details.TypeEfect < 1f)
        {
            if (details.CriticalDamage > 1f)
            {
                yield return StartCoroutine(UIText.TypeDialog($"A critical hit!!"));
            }

            yield return StartCoroutine(UIText.TypeDialog($"It was not very effective!!"));
        }
    }
    

    //BEFORE ATACKS  
    public void PlayerPhysicalAttack(int indexofmove)
    {
        if (indexofmove <= (UIText.attmoves.Count - 1))
        {
            CurrentMove = UIText.attmoves[indexofmove];
            if (CurrentMove.Target == MoveTarget.Foe)
            {
                UIText.EnemyBox.SetActive(true);
                states = BattleStates.ENEMYCHOOSE;
            }
            else if (CurrentMove.Target == MoveTarget.Friendly)
            {
                UIText.FriendlyBox.SetActive(true);
                states = BattleStates.FRIENDLYCHOOSE;
            }
            else if (CurrentMove.Target == MoveTarget.Self)
            {
                UIText.FightBox.SetActive(false);
                StartCoroutine(ChooseTargetToAttack(currentPlayer, currentPlayer));
            }
        }
        else
        {
            UIText.FightBox.SetActive(true);
            UIText.EnemyBox.SetActive(false);
            UIText.FriendlyBox.SetActive(false);
        }
    }
    public void PlayerMagicalAttack(int indexofmove)
    {
        if (indexofmove <= (UIText.magicmoves.Count - 1))
        {
            CurrentMove = UIText.magicmoves[indexofmove];

            if (CurrentMove.Target == MoveTarget.Foe)
            {
                UIText.EnemyBox.SetActive(true);
                states = BattleStates.ENEMYCHOOSE;
            }
            else if (CurrentMove.Target == MoveTarget.Friendly)
            {
                UIText.FriendlyBox.SetActive(true);
                states = BattleStates.FRIENDLYCHOOSE;
            }
            else if (CurrentMove.Target == MoveTarget.Self)
            {
                UIText.MagicBox.SetActive(false);
                StartCoroutine(ChooseTargetToAttack(currentPlayer, currentPlayer));
            }
        }
        else
        {
            UIText.MagicBox.SetActive(true);
            UIText.EnemyBox.SetActive(false);
            UIText.FriendlyBox.SetActive(false);
        }
    } 
    public IEnumerator ChooseTargetToAttack(BattleUnit attacker, BattleUnit defender)
    {
        states = BattleStates.BUSY;
        if (CurrentMove.MovDescriv == MoveDescriver.Revive && defender.Unit.isdead)
        {
            if (attacker.Unit.Mana >= CurrentMove.ResourceCost)
            {
                attacker.playerUI.ChangeSelectedColor(Color.blue);
                yield return StartCoroutine(UseMove(attacker, defender));
            }
            else
            {
                yield return StartCoroutine(UIText.TypeDialog("You dont have enought mana, please choose another action"));
                UIText.InitialBox.SetActive(true);
                UIText.EnemyBox.SetActive(false);
                UIText.FriendlyBox.SetActive(false);
                CurrentMove = null;
                states = BattleStates.INITIALACTION;
            }
        }
        else if (CurrentMove.MovDescriv != MoveDescriver.Revive && !defender.Unit.isdead)
        {
            if (attacker.Unit.Mana >= CurrentMove.ResourceCost)
            {
                attacker.playerUI.ChangeSelectedColor(Color.blue);
                yield return StartCoroutine(UseMove(attacker, defender));
            }
            else
            {
                yield return StartCoroutine(UIText.TypeDialog("You dont have enought mana, please choose another action"));
                UIText.InitialBox.SetActive(true);
                UIText.EnemyBox.SetActive(false);
                UIText.FriendlyBox.SetActive(false);
                CurrentMove = null;
                states = BattleStates.INITIALACTION;
            }
        }      
        else if (CurrentMove.MovDescriv != MoveDescriver.Revive && defender.Unit.isdead)
        {
            yield return StartCoroutine(UIText.TypeDialog("You can't target dead units with that move"));
            UIText.InitialBox.SetActive(true);
            UIText.EnemyBox.SetActive(false);
            UIText.FriendlyBox.SetActive(false);
            CurrentMove = null;
            states = BattleStates.INITIALACTION;
        }
    }
    private BattleUnit GetPossibleTarget()
    {
        var possibleTarget = PlayerTeam[UnityEngine.Random.Range(0, PlayerTeam.Count)];
        while (possibleTarget.Unit.isdead)
        {
            possibleTarget = PlayerTeam[UnityEngine.Random.Range(0, PlayerTeam.Count)];
            if (!possibleTarget.Unit.isdead)
            {
                break;
            }
        }
        return possibleTarget;
    }

    //AFTER ATACKS
    public void CheckWhoNext()
    {
        if (states != BattleStates.WON || states != BattleStates.LOSE)
        {
            if (currentPlayer == playerUnit_1)
            {
                if (currentPlayer.Unit.isdead)
                    currentPlayer.playerUI.ChangeSelectedColor(Color.gray);
                else
                    currentPlayer.playerUI.ChangeSelectedColor(Color.blue);
                StartCoroutine(Player2Turn());
            }
            else if (currentPlayer == playerUnit_2)
            {
                if (currentPlayer.Unit.isdead)
                    currentPlayer.playerUI.ChangeSelectedColor(Color.gray);
                else
                    currentPlayer.playerUI.ChangeSelectedColor(Color.blue);
                StartCoroutine(Player3Turn());
            }
            else if (currentPlayer == playerUnit_3)
            {
                if (currentPlayer.Unit.isdead)
                    currentPlayer.playerUI.ChangeSelectedColor(Color.gray);
                else
                    currentPlayer.playerUI.ChangeSelectedColor(Color.blue);
                StartCoroutine(EnemyTurn1());
            }
            else if (currentPlayer == enemyUnit_1)
            {
                StartCoroutine(EnemyTurn2());
            }
            else if (currentPlayer == enemyUnit_2)
            {
                StartCoroutine(EnemyTurn3());
            }
            else if (currentPlayer == enemyUnit_3)
            {
                StartCoroutine(Player1Turn());
            }
            else
            {
                return;
            }
        }
        else
        {
            return;
        }
    }
    private void CheckIfDead(BattleUnit unit)
    {
        if (unit.Unit.isdead)
        {
            if (PlayerTeam.Contains(unit))
            {
                unit.playerUI.ZeroPlayer();
                //PlayerTeam.Remove(unit);
                DeadPlayerTeam.Add(unit);
            }
            else if (EnemyTeam.Contains(unit))
            {
                unit.enemyUI.ZeroEnemy();
                //EnemyTeam.Remove(unit);
                DeadEnemyTeam.Add(unit);
                TotalExpGained += HandleExpGain(unit);
                TotalMoneyGained += HandleMoneyGain(unit);
            }
        }        
    }
    private void ClearTeams()
    {
        PlayerTeam.Clear();
        EnemyTeam.Clear();
        DeadPlayerTeam.Clear();
        DeadEnemyTeam.Clear();
    }

    //UI CONTROLS
    public void HandleUpdate()
    {
        if (states == BattleStates.INITIALACTION)
        {
            UIText.SetDialog("Choose an Action!");
            HandleAction(UIText.InitialBoxTexts);
        }
        else if (states == BattleStates.PHYSICALMOVES)
        {
            HandleAction(UIText.FightBoxTexts);
            if (currentAction < UIText.attmoves.Count)
            {
                UIText.SetDialog($"Type:{UIText.attmoves[currentAction].Type}              Power:{UIText.attmoves[currentAction].Power} \r\nAccuracy:{UIText.attmoves[currentAction].Accuracy}                     Cost:{UIText.attmoves[currentAction].ResourceCost}");
            }
            else
            {
                UIText.SetDialog("");
            }

        }
        else if (states == BattleStates.MAGICALMOVES)
        {
            HandleAction(UIText.MagicBoxTexts);
            if (currentAction < UIText.magicmoves.Count)
            {
                UIText.SetDialog($"Type:{UIText.magicmoves[currentAction].Type}              Power:{UIText.magicmoves[currentAction].Power} \r\nAccuracy:{UIText.magicmoves[currentAction].Accuracy}                     Cost:{UIText.magicmoves[currentAction].ResourceCost}");
            }
            else
            {
                UIText.SetDialog("");
            }
        }
        else if (states == BattleStates.CHOOSEITEMS)
        {
            HandleAction(UIText.ItemBoxTexts);
            UIText.SetDialog("Choose a Potion");
        }
        else if(states == BattleStates.ENEMYCHOOSE)
        {
            HandleAction(UIText.EnemyBoxTexts);
            if (currentAction == 0)
            {
                UIText.SetDialog("Center Enemy");
            }
            if(currentAction == 1)
            {
                UIText.SetDialog("Left Enemy");
            }
            if(currentAction == 2)
            {
                UIText.SetDialog("Right Enemy");
            }
            if(currentAction == 3)
            {
                UIText.SetDialog("");
            }
        }
        else if (states == BattleStates.FRIENDLYCHOOSE)
        {
            HandleAction(UIText.FriendlyBoxTexts);
        }
        else if(states == BattleStates.FORGETTINGMOVE)
        {
            HandleAction(UIText.moveSelector.movesText);
            if (currentAction != 3)
            {
                if (_moveToLearn != null)
                {
                    if (_moveToLearn.MovCatg == MoveCategory.Physical)
                    {
                        UIText.SetDialog($"Type:{currentPlayer.Unit.Unit_Physical_Moves[currentAction].AMBase.Type}              Power:{currentPlayer.Unit.Unit_Physical_Moves[currentAction].AMBase.Power} \r\nAccuracy:{currentPlayer.Unit.Unit_Physical_Moves[currentAction].AMBase.Accuracy}                   Cost:{currentPlayer.Unit.Unit_Physical_Moves[currentAction].AMBase.ResourceCost}");
                    }
                    else if (_moveToLearn.MovCatg == MoveCategory.Magical)
                    {
                        UIText.SetDialog($"Type:{currentPlayer.Unit.Unit_Magical_Moves[currentAction].AMBase.Type}              Power:{currentPlayer.Unit.Unit_Magical_Moves[currentAction].AMBase.Power} \r\nAccuracy:{currentPlayer.Unit.Unit_Magical_Moves[currentAction].AMBase.Accuracy}                   Cost:{currentPlayer.Unit.Unit_Magical_Moves[currentAction].AMBase.ResourceCost}");
                    }
                }
            }
            else
            {
                UIText.SetDialog($"Type:{_moveToLearn.Type}              Power:{_moveToLearn.Power} \r\nAccuracy:{_moveToLearn.Accuracy}                     Cost:{_moveToLearn.ResourceCost}");
            }
        }
        else
        {
            UIText.SetDialog("", false);
        }

    }

    private void HandleAction(List<Text> currMenu)
    {
        if (states != BattleStates.FORGETTINGMOVE)
        {
            if (states == BattleStates.CHOOSEITEMS)
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    ++currentSelected;
                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    --currentSelected;
                }
                currentSelected = Mathf.Clamp(currentSelected, 0, (inventoryUI.playersInventory._Inventory.Count - 1));
                inventoryUI.HandleUIUpdate(currentSelected);
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    UIText.SetDialog("", false);
                    CheckMenuAction(currentSelected, currMenu);
                }
                else if (Input.GetKeyDown(KeyCode.Return))
                {
                    UIText.ItemBox.SetActive(false);
                    UIText.InitialBox.SetActive(true);
                    states = BattleStates.INITIALACTION;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    if (currentAction < 2)
                        currentAction += 2;
                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    if (currentAction > 1)
                        currentAction -= 2;
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    if (currentAction == 1 || currentAction == 3)
                        --currentAction;
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    if (currentAction == 0 || currentAction == 2)
                        ++currentAction;
                }
                UIText.UpdateHandleAction(currentAction, currMenu);

                if (Input.GetKeyUp(KeyCode.Space))
                {
                    UIText.SetDialog("", false);
                    CheckMenuAction(currentAction, currMenu);
                    currentAction = 0;
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (currentAction < UnitBase.MaxNumberOfMoves)
                    ++currentAction;
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                if (currentAction > 0)
                    --currentAction;
            }
            UIText.UpdateHandleAction(currentAction, currMenu);

            if (Input.GetKeyUp(KeyCode.Space))
            {
                CheckMenuAction(currentAction, currMenu);
                currentAction = 0;
            }
        }
    }
    public void CheckMenuAction(int selected, List<Text> uiTexts)
    {
        if (uiTexts == UIText.InitialBoxTexts)
        {
            if (selected == 0)
            {
                UIText.InitialBox.SetActive(false);
                UIText.FightBox.SetActive(true);
                states = BattleStates.PHYSICALMOVES;
            }
            if (selected == 1)
            {
                UIText.InitialBox.SetActive(false);
                UIText.MagicBox.SetActive(true);
                states = BattleStates.MAGICALMOVES;
            }
            if (selected == 2)
            {
                states = BattleStates.CHOOSEITEMS;
                UIText.InitialBox.SetActive(false);
                UIText.ItemBox.SetActive(true);

            }
            if (selected == 3)
            {
                RunFromBattle();
            }
        }
        if (uiTexts == UIText.FightBoxTexts)
        {
            UIText.FightBox.SetActive(false);
            if (selected == 3)
            {
                UIText.InitialBox.SetActive(true);
                states = BattleStates.INITIALACTION;
            }
            else
            {
                PlayerPhysicalAttack(selected);
            }            
        }
        if (uiTexts == UIText.MagicBoxTexts)
        {
            UIText.MagicBox.SetActive(false);
            if (selected == 3)
            {
                UIText.InitialBox.SetActive(true);
                states = BattleStates.INITIALACTION;
            }
            else
            {
                PlayerMagicalAttack(selected);
            }           
        }
        if (uiTexts == UIText.ItemBoxTexts)
        {
            UIText.ItemBox.SetActive(false);
            var itemToUse = inventoryUI.playersInventory._Inventory[selected];
            if (itemToUse.Item.Type == ItemType.OnFriendly)
            {
                UIText.FriendlyBox.SetActive(true);
                states = BattleStates.FRIENDLYCHOOSE;
            }
            else
            {
                UIText.EnemyBox.SetActive(true);
                states = BattleStates.ENEMYCHOOSE;
            }
            usingItem = true;
        }
        if (uiTexts == UIText.EnemyBoxTexts)
        {
            UIText.EnemyBox.SetActive(false);
            if (selected <= (EnemyTeam.Count - 1))
            {
                if (usingItem)
                {
                    //Use Item
                    StartCoroutine(UseItem(currentSelected, EnemyTeam[selected]));
                }
                else
                {
                    //UseMove
                    StartCoroutine(ChooseTargetToAttack(currentPlayer, EnemyTeam[selected]));
                }
            }
            else
            {
                if (selected == 3)
                {
                    UIText.InitialBox.SetActive(true);
                    states = BattleStates.INITIALACTION;
                    usingItem = false;
                }
                else
                {
                    UIText.EnemyBox.SetActive(true);
                }
            }
        }
        if (uiTexts == UIText.FriendlyBoxTexts)
        {
            UIText.FriendlyBox.SetActive(false);
            if (selected <= (PlayerTeam.Count - 1))
            {
                if (usingItem)
                {
                    //Use Item
                    StartCoroutine(UseItem(currentSelected, PlayerTeam[selected]));
                }
                else
                {
                    //Use Move
                    StartCoroutine(ChooseTargetToAttack(currentPlayer, PlayerTeam[selected]));
                }
            }
            else
            {
                if (selected == 3)
                {
                    UIText.InitialBox.SetActive(true);
                    usingItem = false;
                    states = BattleStates.INITIALACTION;
                }
                else
                {
                    UIText.FriendlyBox.SetActive(true);
                }
            }
        }
        if (uiTexts == UIText.moveSelector.movesText)
        {
            OnMoveSelected(selected);
        }
    }

    //UI DISPLAYS
    private void UpdateAllHPandMP()
    {
        foreach (var player in PlayerTeam)
        {
            player.playerUI.UpdatePlayerHPandMP();
        }
        foreach (var enemy in EnemyTeam)
        {
            enemy.enemyUI.UpdateEnemyHP();
        }

    }
    private void AddPTeam(BattleUnit Unit)
    {
        PlayerTeam.Add(Unit);
        Unit.playerUI.SetPlayer(Unit.Unit);

        if (Unit.Unit.isdead)
        {
            DeadPlayerTeam.Add(Unit);
        }
    }
    public void RemoveCurrentMove()
    {
        CurrentMove = null;
    }
    

    //BATTLE ENDS

    IEnumerator CheckForEndBattle()
    {
        if (DeadPlayerTeam.Count == PlayerTeam.Count)
        {         
            //Play battle lost Music

            states = BattleStates.LOSE;
            yield return StartCoroutine(UIText.TypeDialog("Your team is all dead, you lose"));
            ClearTeams();
            playerParty.Party.ForEach(u => u.OnBattleOver());
            yield return new WaitForSeconds(1f);
            TotalExpGained = 0;
            TotalMoneyGained = 0;
            OnBattleOver(false);
        }
        else if (DeadEnemyTeam.Count == EnemyTeam.Count)
        {   
            //Play battle won Music

            states = BattleStates.WON;
            yield return StartCoroutine(UIText.TypeDialog("Congractulations, you win"));
            yield return StartCoroutine(EndBattleScreen());
            ClearTeams();
            yield return new WaitForSeconds(1f);
            playerParty.Party.ForEach(u => u.OnBattleOver());
            TotalExpGained = 0;
            TotalMoneyGained = 0;
            OnBattleOver(true);          
        }
        else
        {
            yield return new WaitForSeconds(0.0001f);
        }
    }
 
    IEnumerator EndBattleScreen()
    {
        currentPlayer = null;
        int expPerPerson = TotalExpGained / PlayerTeam.Count;
        UIText.EXPDisplay.SetActive(true);
        foreach (var player in PlayerTeam)
        {
            var prevLvl = player.Unit.ULevel;
            currentPlayer = player;
            player.Unit.EXP += expPerPerson;
            //visualisation of growing exp bar
            yield return StartCoroutine(UIText.TypeDialog($"{player.Unit.UBase.Name} gained {expPerPerson} exp points"));
            yield return StartCoroutine(player.playerUI.GrowEXP());
            //check if lvled UP
            while (player.Unit.CheckForLvlUP())
            {
                yield return StartCoroutine(UIText.TypeDialog($"{player.Unit.UBase.Name} gained a level!!"));
                var newMove = player.Unit.GetMoveAtLvl();
                if(newMove != null)
                {
                    // check if Move is Physical or Not
                    if (player.Unit.CheckMovePhysical(newMove))
                    {
                        if (player.Unit.Unit_Physical_Moves.Count < UnitBase.MaxNumberOfMoves)
                        {
                            //Learn Move
                            player.Unit.LearnMove(newMove, player.Unit.Unit_Physical_Moves);
                            yield return StartCoroutine(UIText.TypeDialog($"{player.Unit.UBase.Name} learn {newMove.AttMovBase.Name}"));
                        }
                        else
                        {
                            //Forget Move                            
                            yield return StartCoroutine(UIText.TypeDialog($"{player.Unit.UBase.Name} can learn a new move!!"));
                            yield return StartCoroutine(UIText.TypeDialog($"But {player.Unit.UBase.Name} can only remember up to {UnitBase.MaxNumberOfMoves} of this type.."));
                            yield return StartCoroutine(ForgetMove(newMove.AttMovBase, player.Unit.Unit_Physical_Moves));
                            yield return new WaitUntil(() => states != BattleStates.FORGETTINGMOVE);
                        }
                    }
                    else
                    {
                        if (player.Unit.Unit_Magical_Moves.Count < UnitBase.MaxNumberOfMoves)
                        {
                            //Learn Move
                            player.Unit.LearnMove(newMove, player.Unit.Unit_Magical_Moves);
                            yield return StartCoroutine(UIText.TypeDialog($"{player.Unit.UBase.Name} learn {newMove.AttMovBase.Name}"));
                        }
                        else
                        {
                            //Forget Move
                            yield return StartCoroutine(UIText.TypeDialog($"{player.Unit.UBase.Name} can learn a new move!!"));
                            yield return StartCoroutine(UIText.TypeDialog($"But {player.Unit.UBase.Name} can only remember up to {UnitBase.MaxNumberOfMoves} of this type.."));
                            yield return StartCoroutine(ForgetMove(newMove.AttMovBase, player.Unit.Unit_Magical_Moves));
                            yield return new WaitUntil(() => states != BattleStates.FORGETTINGMOVE);
                        }
                    }
                }
                yield return new WaitForSeconds(1f);
                yield return StartCoroutine(player.playerUI.GrowEXP(true));
            }
            //If unit lvled Up 
            if (prevLvl != player.Unit.ULevel)
            {
                //Tell new lvl
                yield return StartCoroutine(UIText.TypeDialog($"{player.Unit.UBase.Name} is now level {player.Unit.ULevel}."));
                //Calculate new stats
                player.Unit.CalculateStats();
            }
        }
        yield return new WaitForSeconds(1f); 
        UIText.EXPDisplay.SetActive(false);

        yield return UIText.TypeDialog($"{player.Name}'s party gained {TotalMoneyGained}$.");
        MoneyHandler.i.AddMoney(TotalMoneyGained);
        states = BattleStates.WON;
        if (_isBossBattle)
        {
            boss.CheckIfbattleLost();
        }
       
    }

    //MOVES AND EXP
    public void OnMoveSelected(int moveIndex)
    {
        UIText.moveSelector.gameObject.SetActive(false);
        if (moveIndex == UnitBase.MaxNumberOfMoves)
        {
            //Dont learn new move
            StartCoroutine(UIText.TypeDialog($"{currentPlayer.Unit.UBase.Name} decided not to learn {_moveToLearn.Name}"));
        }
        else
        {
            //Forget the selected move and learn new move                     
            if (_moveToLearn.MovCatg == MoveCategory.Physical)
            {
                var oldMove = currentPlayer.Unit.Unit_Physical_Moves[moveIndex];
                currentPlayer.Unit.Unit_Physical_Moves[moveIndex] = new Unit_Moves(_moveToLearn);
                StartCoroutine(UIText.TypeDialog($"{currentPlayer.Unit.UBase.Name} forgot about {oldMove.AMBase.Name} and learned {_moveToLearn.Name}"));   
            }
            else if (_moveToLearn.MovCatg == MoveCategory.Magical)
            {
                var oldMove = currentPlayer.Unit.Unit_Physical_Moves[moveIndex];
                currentPlayer.Unit.Unit_Physical_Moves[moveIndex] = new Unit_Moves(_moveToLearn);
                StartCoroutine(UIText.TypeDialog($"{currentPlayer.Unit.UBase.Name} forgot about {oldMove.AMBase.Name} and learned {_moveToLearn.Name}"));
            }
            _moveToLearn = null;
            currentPlayer = null;
        }
        states = BattleStates.WON;
    }
    private int HandleExpGain(BattleUnit unit)
    {
        //Calculate EXP gain
        int expYield = unit.Unit.UBase.ExpYield;
        int lvl = unit.Unit.ULevel;
        float bossBonus = (_isBossBattle) ? 1.5f : 1f;

        int expGained = Mathf.FloorToInt((expYield * lvl * bossBonus) / 7);

        return expGained;

    }
    private float HandleMoneyGain(BattleUnit unit)
    {
        //Calculate EXP gain
        float moneYield = unit.Unit.UBase.MoneyYield;
        int lvl = unit.Unit.ULevel;
        float bossBonus = (_isBossBattle) ? 1.5f : 1f;

        float moneyGained = Mathf.FloorToInt((moneYield * lvl * bossBonus) / 7); ;

        return moneyGained;

    }
    IEnumerator ForgetMove(AttackMovesBase newMove, List<Unit_Moves> moveGroup)
    {
        states = BattleStates.BUSY;
        UIText.moveSelector.gameObject.SetActive(true);
        UIText.moveSelector.SetMoveData(newMove, moveGroup);
        _moveToLearn = newMove;
        yield return StartCoroutine(UIText.TypeDialog($"Choose a move you want to forget."));
        states = BattleStates.FORGETTINGMOVE;
    }
 
    //RUNNING FROM BATTLE  
    public void RunFromBattle()
    {
        UIText.InitialBox.SetActive(false);
        states = BattleStates.RUN;
        StartCoroutine(TryToRunFromBattle());
    }
    IEnumerator TryToRunFromBattle()
    {  
        
        if (_isBossBattle)
        {
            yield return StartCoroutine(UIText.TypeDialog("You can't run away!"));
            CheckWhoNext();
        }
        else
        {
            float x = UnityEngine.Random.Range(0, 76);
            float y = UnityEngine.Random.Range(0, 36);
            ++_escapeAttemps;

            float r = (x * 128) / y + 30 * _escapeAttemps;
            r = r % 256;

            if (UnityEngine.Random.Range(0,256) < r)
            {
                yield return StartCoroutine(UIText.TypeDialog("Ran away safely!"));
                ClearTeams();
                OnRanAway?.Invoke();
            }
            else
            {
                yield return StartCoroutine(UIText.TypeDialog("Couldn't escape!"));
                CheckWhoNext();
            }
        }

    }

}
