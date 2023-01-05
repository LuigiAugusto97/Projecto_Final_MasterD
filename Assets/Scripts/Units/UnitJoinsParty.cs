using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitJoinsParty : MonoBehaviour, ISavable
{
    [SerializeField] Units UnitToJoin;
    public bool UnitHasJoined = false;

    [Header("Dialog Options")]
    [SerializeField] Dialog dialog;

    //To give Units to player
    public IEnumerator AddUnitToParty(Player_CTRL player)
    {
        yield return DialogManager.Instance.ShowDialog(dialog);
        player.GetComponent<PartySystem>().AddUnit(UnitToJoin);
        UnitHasJoined = true;

        string dialogtext = $"{UnitToJoin.UBase.Name} has joined {player.Name}'s party!";

        yield return DialogManager.Instance.ShowDialogText(dialogtext);
    }

    public bool CanGive()
    {
        if (UnitToJoin != null && !UnitHasJoined)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    //SAVE SYSTEM
    public object SaveData()
    {
        return UnitHasJoined;
    }

    public void LoadData(object data)
    {
        UnitHasJoined = (bool)data;
    }
}
