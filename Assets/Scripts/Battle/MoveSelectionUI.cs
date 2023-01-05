using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveSelectionUI : MonoBehaviour
{
    public List<Text> movesText;

    public void SetMoveData(AttackMovesBase newMove, List<Unit_Moves> moveGroup)
    {
        for (int i = 0; i < moveGroup.Count; i++)
        {
            movesText[i].text = moveGroup[i].AMBase.Name;
        }

        movesText[moveGroup.Count].text = newMove.Name;
    }

}