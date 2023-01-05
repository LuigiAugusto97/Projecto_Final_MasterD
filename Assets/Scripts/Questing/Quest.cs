using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestProgress{ None, Iniciated, InProgress, Completed }
[System.Serializable]
public class Quest
{
    public QuestBase questBase { get; private set; }
    public QuestProgress progress { get; private set; }

    public Quest(QuestBase quest)
    {
        questBase = quest;
    }

    public IEnumerator IniciateQuest()
    {
        progress = QuestProgress.Iniciated;
        yield return DialogManager.Instance.ShowDialog(questBase.StartDialog);
        
        //Add to track quest
        var playersQuest = PlayersQuests.GetActiveQuests();
        playersQuest.AddQuest(this);
    }

    public IEnumerator CompleteQuest(Player_CTRL player)
    {
        progress = QuestProgress.Completed;
        yield return DialogManager.Instance.ShowDialog(questBase.FinishedDialog);

        if (questBase.RequiredItem != null)
        {
            player.GetComponent<Inventory>().DecreaseItemCount(questBase.RequiredItem, questBase.RequiredItemCount);
        }
        if (questBase.RewardItem != null)
        {
            player.GetComponent<Inventory>().AddItem(questBase.RewardItem, questBase.RewardItemCount);

            //PlayPickup Music
            AudioManager.i.PlaySFX(AudioId.ItemGained, true);

            string dialogtext = $"{player.Name} aquired a {questBase.RewardItem.Name}!";
            if (questBase.RewardItemCount > 1)
            {
                dialogtext = $"{player.Name} aquired x{questBase.RewardItemCount} {questBase.RewardItem.Name}!";
            }
            yield return DialogManager.Instance.ShowDialogText(dialogtext);
        }
        if (questBase.RewardMoneyAmount > 0)
        {
            MoneyHandler.i.AddMoney(questBase.RewardMoneyAmount);

            //PlayPickup Music
            AudioManager.i.PlaySFX(AudioId.MoneyGained, true);

            string dialogtext = $"{player.Name} gained {questBase.RewardMoneyAmount}$!";
            yield return DialogManager.Instance.ShowDialogText(dialogtext);
        }

        //Add to track quest
        var playersQuest = PlayersQuests.GetActiveQuests();
        playersQuest.AddQuest(this);
    }

    public bool CanBeCompleted()
    {
        var playersInventory = Inventory.GetInventory();
       if (questBase.RewardItem != null)
       {
            if (!playersInventory.CheckIfItemPresent(questBase.RequiredItem, questBase.RequiredItemCount))
                return false;
       }

        return true;
    }

    //TO SAVE
    public QuestSaveData GetSavedData()
    {
        var saveData = new QuestSaveData
        {
            questname = questBase.QuestName,
            questProgress = progress
        };
        return saveData;
    }

    //TO LOAD
    public Quest(QuestSaveData saveData)
    {
        questBase = QuestDB.GetQuestByName(saveData.questname);
        progress = saveData.questProgress;
    }
}
[System.Serializable]
public class QuestSaveData
{
    public string questname;
    public QuestProgress questProgress;
}
