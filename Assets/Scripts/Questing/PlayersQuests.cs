using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayersQuests : MonoBehaviour, ISavable
{
    public List<Quest> activeQuests = new List<Quest>();
    public event Action onQuestUpdated;

    //To add quests to the players totalquest
    public void AddQuest(Quest questToAdd)
    {
        if (!activeQuests.Contains(questToAdd))
        {
            activeQuests.Add(questToAdd);
        }
        onQuestUpdated?.Invoke();
    }

    //Check if quest progress is Iniciated
    public bool IsIniciated(string questName)
    {
        var questProgress = activeQuests.FirstOrDefault(x => x.questBase.QuestName == questName)?.progress;

        return (questProgress == QuestProgress.Iniciated || questProgress == QuestProgress.Completed);
    }
    //Check if quest progress is Completed
    public bool IsCompleted(string questName)
    {
        var questProgress = activeQuests.FirstOrDefault(x => x.questBase.QuestName == questName)?.progress;

        return (questProgress == QuestProgress.Completed);
    }

    //Always find players quest
    public static PlayersQuests GetActiveQuests()
    {
        return FindObjectOfType<Player_CTRL>().GetComponent<PlayersQuests>();
    }

    //SAVE SYSTEM
    public object SaveData()
    {
        return activeQuests.Select(x => x.GetSavedData()).ToList();
    }

    public void LoadData(object data)
    {
        var saveData = (List<QuestSaveData>)data;
        if (saveData != null)
        {
            activeQuests = saveData.Select(x => new Quest(x)).ToList();
            onQuestUpdated?.Invoke();
        }
    }
}
