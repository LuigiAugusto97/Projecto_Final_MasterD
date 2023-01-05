using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryTrigger : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] Dialog dialog;

    public bool blocksMovement = true;
    public bool TriggerManyTimes => false;

    public void OnPlayerTriggered(Player_CTRL player)
    {
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog)) ;
    }
}
