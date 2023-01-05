using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounters : MonoBehaviour, IPlayerTriggerable
{
    public bool TriggerManyTimes => true;

    public void OnPlayerTriggered(Player_CTRL player)
    {
        //Check for Random Encounter
        if (UnityEngine.Random.Range(1, 101f) <= 10)
        {
            GameCTRL.Instance.StartBattle();
        }
    }
}
