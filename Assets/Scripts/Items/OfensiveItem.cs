using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ofensive Item", menuName = "Items/Create new Ofensive Item")]
public class OfensiveItem : ItemBase
{
    [Header("Dmg Options")]
    [SerializeField] int flatDmgAmount;
    [SerializeField] float percentageDmgAmount;

    [Header("Stats Options")]
    [SerializeField] List<StatChanges> boost;

    [Header("Status Options")]
    [SerializeField] int statusChance;
    [SerializeField] ConditionID status;
    [SerializeField] int volatileStatusChance;
    [SerializeField] ConditionID volatileStatus;

    public override bool Use(Units unit)
    {
        //Check if dead
        if (unit.isdead) return false;

        //Deal normal or percentage damage
        if (flatDmgAmount > 0 || percentageDmgAmount > 0)
        {
            if (flatDmgAmount > 0)
            {
                unit.TakeDamage(flatDmgAmount);
            }
            else if (percentageDmgAmount > 0)
            {
                if (!unit.UBase.IsBoss)
                {
                    var damage = percentageDmgAmount / 100;
                    unit.TakeDamage((int)(unit.HP*damage));
                }
                else
                {
                    return false;
                }
            }
        }

        //Only apply if unit is not dead
        if (unit.HP > 0)
        {
            //Apply boostChanges
            if (boost != null)
            {
                unit.ApplyBoosts(boost);
            }

            //Apply status
            if (status != ConditionID.None)
            {
                if (unit.Status != null)
                {
                    return false;
                }
                else
                {
                    var rndchance = Random.Range(1, 101);
                    if (rndchance <= statusChance)
                        unit.SetStatus(status);
                }
            }

            //Apply volatile status
            if (volatileStatus != ConditionID.None)
            {
                if (unit.VolatileStatus != null)
                {
                    return false;
                }
                else
                {
                    var rndchance = Random.Range(1, 101);
                    if (rndchance <= volatileStatusChance)
                        unit.SetVolatileStatus(volatileStatus);
                }
            }
        }

        return true;
    }
}
