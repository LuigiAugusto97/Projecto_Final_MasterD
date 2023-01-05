using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_CTRL : MonoBehaviour, IInteractables, ISavable
{
    [Header("Default Dialog Options")]
    [SerializeField] Dialog dialog;
    
    [Header("Quest Options")]
    [SerializeField] QuestBase questToStart;
    [SerializeField] QuestBase questToComplete;

    [Header("Walk Options")]
    private float idleTimer = 0;
    private NPCState state;
    [SerializeField] List<Vector2> Waypoints;
    [SerializeField] float TimeBetweenWaypoints;
    private int currentWaypoint = 0;


    //References
    private Character_CTRL character;
    private NPCGivesItem givesItem;
    private Quest activeQuest;
    private UnitJoinsParty unitJoins;
    private Healer_CRTL healer;
    private ShopKeeper_CTRL shopKeeper;

    private void Awake()
    {
        character = GetComponent<Character_CTRL>();
        givesItem = GetComponent<NPCGivesItem>();
        unitJoins = GetComponent<UnitJoinsParty>();
        healer = GetComponent<Healer_CRTL>();
        shopKeeper = GetComponent<ShopKeeper_CTRL>();
        state = NPCState.Idle;
    }
    //To interact
    public IEnumerator Interact(Transform personToTalk)
    {
        if (state == NPCState.Idle)
        {
            state = NPCState.Dialog;

            //Look at player
            character.LookAt(personToTalk.position);
            var player = personToTalk.GetComponent<Player_CTRL>();


            //Loop All possible outcomes
            if (questToComplete != null)
            {
                var quest = new Quest(questToComplete);
                yield return quest.CompleteQuest(player);
                questToComplete = null;
            }

            if (givesItem != null && givesItem.CanGive())
            {
                yield return givesItem.GiveItem(player);
            }
            else if (unitJoins != null && unitJoins.CanGive())
            {
                yield return unitJoins.AddUnitToParty(player);
            }
            else if (questToStart != null)
            {
                activeQuest = new Quest(questToStart);
                yield return activeQuest.IniciateQuest();
                questToStart = null;
                if (activeQuest.CanBeCompleted())
                {
                    yield return activeQuest.CompleteQuest(player);
                    activeQuest = null;
                }
            }
            else if (activeQuest != null)
            {
                if (activeQuest.CanBeCompleted())
                {
                    yield return activeQuest.CompleteQuest(player);
                    activeQuest = null;
                }
                else
                {
                    yield return DialogManager.Instance.ShowDialog(activeQuest.questBase.InProgressDialog);
                }
            }
            else if (healer != null)
            {
                yield return healer.HealPlayerParty(personToTalk, dialog);
            }
            else if (shopKeeper != null)
            {
                yield return shopKeeper.HandleShop();
            }
            else
            {
                yield return DialogManager.Instance.ShowDialog(dialog);
            }
            idleTimer = 0f;
            state = NPCState.Idle;
        }
     
    }

    //handle Movementent
    private void Update()
    {
        if (state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= TimeBetweenWaypoints)
            {
                idleTimer = 0f;
                if (Waypoints.Count > 0)
                {
                    StartCoroutine(Walk());
                }

            }
        }
        character.HandleUpdate();
        
    }
    IEnumerator Walk()
    {
        state = NPCState.Walk;
        var oldPos = transform.position;
        yield return StartCoroutine(character.MoveTo(Waypoints[currentWaypoint]));
        if (transform.position != oldPos)
        {
            currentWaypoint = (currentWaypoint + 1) % Waypoints.Count;
        }         
        state = NPCState.Idle;
    }

    //SAVE SYSTEM
    public object SaveData()
    {
        var saveData = new NPCQuestSaveData();
        if (activeQuest != null)
            saveData._activeQuest = activeQuest.GetSavedData();
        if (questToStart != null)
            saveData._questToStart = new Quest(questToStart).GetSavedData();
        if (questToComplete != null)
            saveData._questToComplete = new Quest(questToComplete).GetSavedData();

        return saveData;
    }

    public void LoadData(object data)
    {
        var saveData = (NPCQuestSaveData)data;
        if (saveData != null)
        {
            if (saveData._activeQuest != null)
            {
                activeQuest = new Quest(saveData._activeQuest);
            }
            else
            {
                activeQuest = null;
            } 
            if (saveData._questToStart != null)
            {
                questToStart = new Quest(saveData._questToStart).questBase;
            }
            else
            {
                questToStart = null;
            } 
            if (saveData._questToComplete != null)
            {
                questToComplete = new Quest(saveData._questToComplete).questBase;
            }
            else
            {
                questToComplete = null;
            }
        }
    }
}
[System.Serializable]
public class NPCQuestSaveData
{
    public QuestSaveData _activeQuest;
    public QuestSaveData _questToStart;
    public QuestSaveData _questToComplete;
}

