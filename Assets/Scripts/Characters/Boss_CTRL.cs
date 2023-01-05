using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_CTRL : MonoBehaviour, IInteractables, ISavable
{
    [SerializeField] Dialog StartBattledialog;
    [SerializeField] Dialog Deaddialog;

    [SerializeField] AudioClip bossBattleMusic;
    public AudioClip BossBattleMusic
    {
        get { return bossBattleMusic; }
    }

    public Sprite background;
    private Character_CTRL character;
    private PartySystem bossParty;
    private Player_CTRL player;

    [Header("Quest Options")]
    [SerializeField] QuestBase questToStart;
    [SerializeField] QuestBase questToComplete;
    private Quest activeQuest;

    //Lost battle
    private bool _lostBattle;
    private void Awake()
    {
        character = GetComponent<Character_CTRL>();
        bossParty = GetComponent<PartySystem>();
    }


    public IEnumerator Interact(Transform Character)
    {
        player = FindObjectOfType<Player_CTRL>();
        //Look at player
        character.LookAt(player.transform.position);

        if (!_lostBattle)
        {
            //Show Dialog
            yield return DialogManager.Instance.ShowDialog(StartBattledialog);

            //StartBattle
            GameCTRL.Instance.StartBossBattle(this);
        }
        else
        {
            yield return DialogManager.Instance.ShowDialog(Deaddialog);
        }

    }

    // ON Player winning battler
    public void CheckIfbattleLost()
    {
        if (bossParty.GetPlayer(0).isdead == true)
        {
            _lostBattle = true;

            //Loop though quests
            if (questToStart != null)
            {
                activeQuest = new Quest(questToStart);
                StartCoroutine(activeQuest.IniciateQuest());
                questToStart = null;
                if (activeQuest.CanBeCompleted())
                {
                    StartCoroutine(activeQuest.CompleteQuest(player));
                    activeQuest = null;
                }
            }
            else if (activeQuest != null)
            {
                if (activeQuest.CanBeCompleted())
                {
                    StartCoroutine(activeQuest.CompleteQuest(player));
                    activeQuest = null;
                }
                else
                {
                    StartCoroutine(DialogManager.Instance.ShowDialog(activeQuest.questBase.InProgressDialog));
                }
            }
        }
        else
            _lostBattle = false;
    }

    public object SaveData()
    {
        return _lostBattle;
    }

    public void LoadData(object data)
    {
        _lostBattle = (bool)data;
    }
}
