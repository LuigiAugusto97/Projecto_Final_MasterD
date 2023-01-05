using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recovery Item",menuName = "Items/Create new Recovery Item")]
public class RecoveryItem : ItemBase
{
    [Header("HP Options")]
    [SerializeField] float hpAmountPercentage;
    [SerializeField] bool giveMaxHP;

    [Header("MP Options")]
    [SerializeField] float manaAmountPercentage;
    [SerializeField] bool giveMaxMP;

    [Header("Stats Options")]
    [SerializeField] List<StatChanges> boost;

    [Header("Status Options")]
    [SerializeField] ConditionID condition;
    [SerializeField] bool cureAllStatus;

    [Header("Revive Options")]
    [SerializeField] bool revive;
    [SerializeField] bool reviveMaxHP;

    public override bool Use(Units unit)
    {
        //Revive
        if (revive || reviveMaxHP)
        {
            if (!unit.isdead)
                return false;

            if (revive)
            {
                unit.isdead = false;
                unit.isRevived = true;
                unit.HealDamage(unit.MaxHP / 2);
            }
            else if (reviveMaxHP)
            {
                unit.isdead = false;
                unit.isRevived = true;
                unit.HealDamage(unit.MaxHP);
            }
            unit.CureStatus();
            return true;
        }

        //No item can be use on dead units
        if (unit.isdead) return false;               

        //Heal
        if (giveMaxHP || hpAmountPercentage > 0)
        {
            if (unit.HP == unit.MaxHP)
                return false;

            if (giveMaxHP)
                unit.HealDamage(unit.MaxHP);
            else
            {
                var healAmount = hpAmountPercentage / 100;
                unit.HealDamage((int)(unit.MaxHP * healAmount));
            }
        }
        //Give Mana

        if (giveMaxMP || manaAmountPercentage > 0)
        {
            if (unit.Mana == unit.MaxMana)
                return false;

            if (giveMaxMP)
                unit.RegainMana(unit.MaxMana);
            else
            {
                var manaAmount = manaAmountPercentage / 100;
                unit.RegainMana((int)(unit.MaxMana * manaAmount));
            }
        }

        //Apply boostChanges
        if (boost != null)
        {
            unit.ApplyBoosts(boost);
        }

        //Cure Status
        if (cureAllStatus || condition != ConditionID.None)
        {
            if (unit.Status == null && unit.VolatileStatus == null)
            {
                return false;
            }

            if (cureAllStatus)
            {
                unit.CureStatus();
                unit.CureVolatileStatus();
            }
            else
            {
                if (unit.Status.Id == condition)
                {
                    unit.CureStatus();
                }
                else if (unit.VolatileStatus.Id == condition)
                {
                    unit.CureVolatileStatus();
                }
                else
                {
                    return false;
                }
            }
        }
        return true;
    }
}
