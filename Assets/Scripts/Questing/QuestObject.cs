using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObject : MonoBehaviour
{
    [SerializeField] QuestBase requiredQuest;
    [SerializeField] ObjectFunctions onQuestIniciated;
    [SerializeField] ObjectFunctions onQuestCompleted;

    //References
    private PlayersQuests activequests;
    private void Start()
    {
        activequests = PlayersQuests.GetActiveQuests();
        activequests.onQuestUpdated += UpdateStoryTrigger;
        UpdateStoryTrigger();
    }

    //To avoid Bugs on destroy
    private void OnDestroy()
    {
        activequests.onQuestUpdated -= UpdateStoryTrigger;
    }

    //To control quest interfirance in Overworld gameplay
    public void UpdateStoryTrigger()
    {
        activequests = PlayersQuests.GetActiveQuests();
        if (onQuestIniciated != ObjectFunctions.None && activequests.IsIniciated(requiredQuest.QuestName))
        {
            if (onQuestIniciated == ObjectFunctions.Enable)
            {
                this.gameObject.SetActive(true);
            }
            else if (onQuestIniciated == ObjectFunctions.Disable)
            {
                this.gameObject.SetActive(false);
            }
        }
        if (onQuestCompleted != ObjectFunctions.None && activequests.IsCompleted(requiredQuest.QuestName))
        {
            if (onQuestCompleted == ObjectFunctions.Enable)
            {
                this.gameObject.SetActive(true);
            }
            else if (onQuestCompleted == ObjectFunctions.Disable)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
public enum ObjectFunctions { None, Enable, Disable}