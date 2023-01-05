using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_MovesDB 
{
    static Dictionary<string, AttackMovesBase> moves_Data;
    public static void GetMoves()
    {
        moves_Data = new Dictionary<string, AttackMovesBase>();

        var AllMovessData = Resources.LoadAll<AttackMovesBase>("");
        foreach (var moveData in AllMovessData)
        {
            if (moves_Data.ContainsKey(moveData.Name))
            {
                Debug.LogError($"That move,{moveData.Name}, already exist");
                continue;
            }
            moves_Data[moveData.Name] = moveData;
        }
    }

    public static AttackMovesBase GetMoveByName(string moveName)
    {
        if (!moves_Data.ContainsKey(moveName))
        {
            Debug.LogError($"There is no move with the name:{moveName}");
            return null;
        }

        return moves_Data[moveName];
    }
}
