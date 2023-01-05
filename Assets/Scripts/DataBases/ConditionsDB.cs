using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static void GetStatusConditions()
    {
        foreach (var KV in StatusConditions)
        {
            var StatusConId = KV.Key;
            var StatusCon = KV.Value;

            StatusCon.Id = StatusConId;
        }
    }
    static void PoisonEffect(Units unit)
    {
        unit.TakeDamage(unit.MaxHP / 8);
        unit.StatusChange.Enqueue($"{unit.UBase.Name} is hurt due to poison");
    }
    static void BurnEffect(Units unit)
    {
        unit.TakeDamage(unit.MaxHP / 16);
        unit.StatusChange.Enqueue($"{unit.UBase.Name} is hurt due to burn");
    }
    static bool ParalizeEffect(Units unit)
    {
       if(Random.Range(1, 5) == 1)
       {
            unit.StatusChange.Enqueue($"{unit.UBase.Name} is paralized and can't attack");
            return  false;           
       }
        return true;
    }
    static bool FreezeEffect(Units unit)
    {
       if(Random.Range(1, 5) == 1)
       {
            unit.CureStatus();
            unit.StatusChange.Enqueue($"{unit.UBase.Name} is not frozen and can attack");
            return  false;           
       }
        return true;
    }
    static bool SleepEffect(Units unit)
    {
        if (unit.StatusTime <= 0)
        {
            unit.CureStatus();
            unit.StatusChange.Enqueue($"{unit.UBase.Name} woke up!");
            return true;
        }
        unit.StatusTime--;
        unit.StatusChange.Enqueue($"{unit.UBase.Name} is asleep");
        return false;     
    }
    static void SleepAmount(Units unit)
    {
        //Sleep for 1-3 turns
        unit.StatusTime = Random.Range(1, 4);
        Debug.Log($"Will be a sleep for {unit.StatusTime} rounds");
    }
    static bool BleedEffect(Units unit)
    {
        if (unit.StatusTime <= 0)
        {
            unit.CureStatus();
            unit.StatusChange.Enqueue($"{unit.UBase.Name} stopped bleeding!");
            return true;
        }
        unit.StatusTime--;
        unit.TakeDamage(unit.MaxHP / 12);
        unit.StatusChange.Enqueue($"{unit.UBase.Name} is bleeding.");
        unit.StatusChange.Enqueue($"{unit.UBase.Name} is hurt due to bleeding");
        return false;
    }

    static void BleedAmount(Units unit)
    {
        //Sleep for 1-6 turns
        unit.StatusTime = Random.Range(1, 6);
        Debug.Log($"Will bleed for {unit.StatusTime} rounds");
    }


    //Volatile Status Conditions



    static bool Confusionffect(Units unit)
    {
        if (unit.VolatileStatusTime <= 0)
        {
            unit.CureVolatileStatus();
            unit.StatusChange.Enqueue($"{unit.UBase.Name} as snapped out of confusion!");
            return true;
        }
        unit.StatusTime--;
        //50% of doing a move
        if (Random.Range(1,3) == 1)
        {
            return true;
        }
        //Hurt by Confusion
        unit.StatusChange.Enqueue($"{unit.UBase.Name} is confused\r\nIs hurt by confusion");
        unit.TakeDamage(unit.MaxHP / 8);
        return false;     
    }
    static void ConfusionAmount(Units unit)
    {
        //Confused for 1-3 turns
        unit.StatusTime = Random.Range(1, 5);
    }

    static bool FlinchEffect(Units unit)
    {
        if (unit.VolatileStatusTime <= 0)
        {
            unit.CureVolatileStatus();
            return true;
        }
        unit.StatusTime--;
        unit.StatusChange.Enqueue($"{unit.UBase.Name} cant attack because it flinched");
        return false;
    }
    static void FlinchAmount(Units unit)
    {
        //flinched for 1 turn
        unit.StatusTime = 1;
        Debug.Log($"Will be a confused for {unit.VolatileStatusTime} rounds");
    }



    public static Dictionary<ConditionID, Conditions> StatusConditions { get; set; } = new Dictionary<ConditionID, Conditions>()
    {
        {
            ConditionID.Psn, new Conditions
            {
                Name = "Poison",
                StartMessage = "has been poisoned",
                OnAfterTurn = PoisonEffect
                
            } 
        }, 
        {
            ConditionID.Brn, new Conditions
            {
                Name = "Burn",
                StartMessage = "has been burned",
                OnAfterTurn = BurnEffect
                
            } 
        },
        {
            ConditionID.Par, new Conditions
            {
                Name = "Paralized",
                StartMessage = "has been paralized",
                OnBeforeMove = ParalizeEffect
            } 
        },
        {
            ConditionID.Frz, new Conditions
            {
                Name = "Freeze",
                StartMessage = "has been frozen",
                OnBeforeMove = FreezeEffect
            } 
        },
        {
            ConditionID.Slp, new Conditions
            {
                Name = "Sleep",
                StartMessage = "has fallen asleep",
                OnStart = SleepAmount,
                OnBeforeMove = SleepEffect
            } 
        },
        {
            ConditionID.Confusion, new Conditions
            {
                Name = "Confusion",
                StartMessage = "is confused",
                OnStart = ConfusionAmount,
                OnBeforeMove = Confusionffect
            } 
        },
        {
            ConditionID.Flinch, new Conditions
            {
                Name = "Flinch",
                StartMessage = "Flinched",
                OnStart = FlinchAmount,
                OnBeforeMove = FlinchEffect
            } 
        },
        {
            ConditionID.Bld, new Conditions
            {
                Name = "Bleed",
                StartMessage = "is bleeding",
                OnStart = BleedAmount,
                OnBeforeMove = BleedEffect
            } 
        }
    };
}
public enum ConditionID
{
    None,
    Psn,
    Brn,
    Slp,
    Par,
    Frz,
    Bld,
    Confusion,
    Flinch
}
