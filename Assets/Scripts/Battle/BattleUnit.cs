using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    public Units Unit { get; set; }
    public BattleUIPlayer playerUI = null;
    public BattleUIEnemy enemyUI = null;
    public void Setup(Units unit)
    {
        Unit =  unit;
    }
}
