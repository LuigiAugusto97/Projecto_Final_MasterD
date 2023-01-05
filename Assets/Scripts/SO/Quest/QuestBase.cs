using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New quest", menuName = "Quest/Creat new quest")]
public class QuestBase : ScriptableObject
{
    [Header("Basic Options")]
    [SerializeField] string questName;
    [TextArea]
    [SerializeField] string description;

    [Header("Dialog Options")]
    [SerializeField] Dialog startDialog;
    [SerializeField] Dialog inProgressDialog;
    [SerializeField] Dialog finishDialog;

    [Header("To Complete Options")]
    //Make a list if more items needed
    [SerializeField] ItemBase requiredItem;
    [SerializeField] int requiredItemCount = 1;


    [Header("Reward Options")]
    [SerializeField] ItemBase rewardItem;
    [SerializeField] int rewardItemCount = 1;

    [SerializeField] int rewardMoney;


    //Properties

    public string QuestName
    {
        get { return questName; }
    }
    public string Description
    {
        get { return description; }
    }
    public Dialog StartDialog
    {
        get { return startDialog; }
    }
    public Dialog InProgressDialog
    {
        get { return inProgressDialog; }
    }
    public Dialog FinishedDialog
    {
        get { return finishDialog; }
    }
    public ItemBase RequiredItem
    {
        get { return requiredItem; }
    }
    public ItemBase RewardItem
    {
        get { return rewardItem; }
    }
    public int RequiredItemCount
    {
        get { return requiredItemCount; }
    }
    public int RewardItemCount
    {
        get { return rewardItemCount; }
    }
    public int RewardMoneyAmount
    {
        get { return rewardMoney; }
    }

}
