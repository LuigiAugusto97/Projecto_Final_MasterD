using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Moves
{
    public AttackMovesBase AMBase { get; set; }
    public int ResourceCost { get; set; }

    public Unit_Moves(AttackMovesBase unit_move)
    {
        AMBase = unit_move;
        ResourceCost = unit_move.ResourceCost;
    }
    public Unit_Moves(Unit_Moves_Data saveData)
    {
        AMBase = Unit_MovesDB.GetMoveByName(saveData.moveName);
        ResourceCost = saveData.resourceCost;
    }
    public Unit_Moves_Data GetSavedData()
    {
        var saveData = new Unit_Moves_Data
        {
            moveName = AMBase.Name,
            resourceCost = ResourceCost
        };

        return saveData;
    }
}

[System.Serializable]
public class Unit_Moves_Data
{
    public string moveName;
    public int resourceCost;
}
