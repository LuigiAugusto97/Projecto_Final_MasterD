using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDB 
{
    static Dictionary<string, QuestBase> quest_Data;

    //Get All SO Quests
    public static void GetQuests()
    {
        quest_Data = new Dictionary<string, QuestBase>();

        var AllQuestsData = Resources.LoadAll<QuestBase>("");
        foreach (var questData in AllQuestsData)
        {
            if (quest_Data.ContainsKey(questData.QuestName))
            {
                Debug.LogError($"That quest,{questData.QuestName}, already exist");
                continue;
            }
            quest_Data[questData.QuestName] = questData;
        }
    }
    //get single quest by name
    public static QuestBase GetQuestByName(string questName)
    {
        if (!quest_Data.ContainsKey(questName))
        {
            Debug.LogError($"There is no quest with the name:{questName}");
            return null;
        }

        return quest_Data[questName];
    }
}
