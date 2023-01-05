using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Units
{
    [SerializeField] UnitBase _base;
    [SerializeField] int _lvl;

    public bool isdead;
    public bool isRevived;
    public UnitBase UBase
    {
        get
        {
            return _base;
        }
        set { }
    }
    public int ULevel
    {
        get
        {
            return _lvl;
        }
        set 
        { 
            _lvl = value; 
        }
    }
    public int EXP { get; set; }
    public int CorruptPoints { get; set; }
    public int HP { get; set; }
    public int Mana { get; set; }
    public List<Unit_Moves> Unit_Physical_Moves { get; set; }
    public List<Unit_Moves> Unit_Magical_Moves { get; set; }
    public Dictionary<Stat,int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBost { get; private set; }
    public Conditions Status { get; private set; }
    public int StatusTime { get; set; }
    public Conditions VolatileStatus { get; private set; }
    public int VolatileStatusTime { get; set; }
    public Dictionary<ConditionID, int> StatusResist { get; private set; }
    public Queue<string> StatusChange { get; private set; } = new Queue<string>();
    public event System.Action OnStatusChanged;

    private float attack;
    private float defence;

    // set all things ofr units
    public void Init()
    {
        //Adicionar ataques à personagem dependendo do nivel;
        Unit_Physical_Moves = new List<Unit_Moves>();
        Unit_Magical_Moves = new List<Unit_Moves>();
        foreach (var attmove in _base.LevelAttMov)
        {
            if (CheckMovePhysical(attmove))
            {
                if (attmove.Level <= ULevel)
                {
                    Unit_Physical_Moves.Add(new Unit_Moves(attmove.AttMovBase));
                }
                if (Unit_Physical_Moves.Count > UnitBase.MaxNumberOfMoves)
                {
                    break;
                }
            }
            else if(!CheckMovePhysical(attmove))
            {
                if (attmove.Level <= ULevel)
                {
                    Unit_Magical_Moves.Add(new Unit_Moves(attmove.AttMovBase));
                }
                if (Unit_Physical_Moves.Count > UnitBase.MaxNumberOfMoves)
                {
                    break;
                }
            }

        }


        CalculateStats();
        HP = MaxHP;
        Mana = MaxMana;

        EXP = _base.GetExpForLevel(ULevel); 

        ResetStatsBots();
        StatusChange = new Queue<string>();
        Status = null;
        VolatileStatus = null;

        //Apply a Status Resist 
        StatusResist = new Dictionary<ConditionID, int>()
        {
            {ConditionID.Psn, _base.PsnResist },
            {ConditionID.Brn, _base.BrnResist },
            {ConditionID.Frz, _base.FrzResist },
            {ConditionID.Par, _base.ParResist },
            {ConditionID.Slp, _base.SlpResist },
            {ConditionID.Confusion, _base.ConfusionResist }
        };
    }

    //save a unit
    public Units(UnitsSaveData saveData)
    {
        _base = UnitsBaseDB.GetUnitByUniqueID(saveData.uniqueId);
        HP = saveData.hp;
        _lvl = saveData.level;
        Mana = saveData.mana;
        EXP = saveData.exp;
        if (saveData.statusID != null)
        {
            Status = ConditionsDB.StatusConditions[saveData.statusID.Value];
        }
        else
        {
            Status = null;
        }

        CalculateStats();
        StatusChange = new Queue<string>();
        ResetStatsBots();
        VolatileStatus = null;

        Unit_Physical_Moves = saveData.physicalMoves.Select(x => new Unit_Moves(x)).ToList(); 
        Unit_Magical_Moves = saveData.magicalMoves.Select(x => new Unit_Moves(x)).ToList(); 
         
        //Apply a Status Resist 
        StatusResist = new Dictionary<ConditionID, int>()
        {
            {ConditionID.Psn, _base.PsnResist },
            {ConditionID.Brn, _base.BrnResist },
            {ConditionID.Frz, _base.FrzResist },
            {ConditionID.Par, _base.ParResist },
            {ConditionID.Slp, _base.SlpResist },
            {ConditionID.Confusion, _base.ConfusionResist }
        };
    }
    //load a unit
    public UnitsSaveData GetSavedData()
    {
        var saveData = new UnitsSaveData()
        {
            uniqueId = _base.UniqueIdentifier,
            hp = HP,
            mana = Mana,
            level = ULevel,
            exp = EXP,
            statusID = Status?.Id,
            physicalMoves = Unit_Physical_Moves.Select(x => x.GetSavedData()).ToList(),
            magicalMoves = Unit_Magical_Moves.Select(x => x.GetSavedData()).ToList()
        };

        return saveData;
    }

    //On battle Over
    public void OnBattleOver()
    {
        CureVolatileStatus();
        ResetStatsBots();
    }
    public bool CheckForLvlUP()
    {
        if ( EXP >= _base.GetExpForLevel(ULevel + 1))
        {
            ++_lvl;
            return true;
        }
        return false;
    }
    public void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((_base.Strenght * ULevel) / 100f + 5));
        Stats.Add(Stat.Defence, Mathf.FloorToInt((_base.Defence * ULevel) / 100f + 5));
        Stats.Add(Stat.Magic, Mathf.FloorToInt((_base.Magic * ULevel) / 100f + 5));
        Stats.Add(Stat.MagicDef, Mathf.FloorToInt((_base.MagicDef * ULevel) / 100f + 5));
        Stats.Add(Stat.ManaRegen, Mathf.FloorToInt((_base.ManaRegen * ULevel) / 100f + 5));

        MaxHP = Mathf.FloorToInt((_base.MaxHP * ULevel) / 100f + 10 + ULevel);
        MaxMana = Mathf.FloorToInt((_base.MaxMana * ULevel) / 100f + 10 + ULevel);

    }
    public LeveldAttackMoves GetMoveAtLvl()
    {
       return _base.LevelAttMov.Where(x => x.Level == _lvl).FirstOrDefault();
    }
    public void LearnMove(LeveldAttackMoves lernableMove, List<Unit_Moves> listToAdd)
    {
        if (listToAdd.Count > UnitBase.MaxNumberOfMoves)
            return;

        listToAdd.Add(new Unit_Moves(lernableMove.AttMovBase));
    }
    public void ResetStatsBots()
    {
        StatBost = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defence, 0},
            {Stat.Magic, 0},
            {Stat.MagicDef, 0},
            {Stat.ManaRegen, 0},
            {Stat.Accuracy, 0 },
            {Stat.Evasion, 0 }
        };
    }
    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }
    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    //Status Changes

    private int GetStat(Stat stat)
    {
        int statValue = Stats[stat];


        // Apply Stat Value;
        int bost = StatBost[stat];
        var bostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (bost >= 0)
        {
            statValue = Mathf.FloorToInt(statValue * bostValues[bost]) ;
        }
        else if (bost < 0)
        {
            statValue = Mathf.FloorToInt(statValue / bostValues[-bost]);
        }

        return statValue;
    }
   
    public void SetStatus(ConditionID conditionID)
    {
        if (Status != null) 
        {
            return;
        }               
        foreach (var KV in StatusResist)
        {
            if (conditionID == KV.Key)
            {
                float x = UnityEngine.Random.Range(0, 100);
                if (x > KV.Value)
                {
                    Status = ConditionsDB.StatusConditions[conditionID];
                    Status?.OnStart?.Invoke(this);
                    StatusChange.Enqueue($"{_base.Name} {Status.StartMessage}");
                    OnStatusChanged?.Invoke();
                }
                else
                {
                    if (KV.Value == 100)
                    {
                        StatusChange.Enqueue($"{_base.Name} is imune to that");
                    }
                    else
                    { 
                        StatusChange.Enqueue($"{_base.Name} resisted");
                    }
                    
                }
            }
        }
       
    }

    public void SetVolatileStatus(ConditionID conditionID)
    {
        if (VolatileStatus != null) return;

        VolatileStatus = ConditionsDB.StatusConditions[conditionID];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChange.Enqueue($"{_base.Name} {VolatileStatus.StartMessage}");        
    }

    public void ApplyBoosts(List<StatChanges> statChanges)
    {
        foreach (var statbost in statChanges)
        {
            var stat = statbost.stat;
            var bost = statbost.boost;

            StatBost[stat] =Mathf.Clamp(StatBost[stat] + bost, -6, 6);
            if (bost > 0)
            {
                StatusChange.Enqueue($"{_base.Name}'s {stat} was increased");
            }
            else if (bost < 0)
            {
                StatusChange.Enqueue($"{_base.Name}'s {stat} was decreased");
            }
        }
    } 

    //Actions to Show
    public HealDetails HealOther(Unit_Moves move, Units target)
    {
        float modifiers = Random.Range(0.85f, 1f);
        float a = (2 * ULevel + 10) / 250f;
        float d = a * move.AMBase.Power * ((float)MagicAttack / 10f) + 2;
        int healAmount = Mathf.FloorToInt(d * modifiers);

        target.HP += healAmount;

        if (target.HP >= target.MaxHP)
        {
            target.HP = target.MaxHP;
        }

        var healDetails = new HealDetails()
        {
            HealAmount = healAmount,
            TargetHP = target.HP
        };
        return healDetails;
    }

    public DamageDetails DealDamage(Unit_Moves move, Units defender)
    {
        float criticalDamage = 1f;
        if (Random.value * 100f <= 6.25f)
        {
            criticalDamage = 2f;
        }



        if (move.AMBase.MovCatg == MoveCategory.Physical)
        {
            attack = Attack;
            defence = defender.Defence;
        }
        else if (move.AMBase.MovCatg == MoveCategory.Magical)
        {
            attack = MagicAttack;
            defence = defender.MagicDefence;
        }

        float type = TypeChart.GetTypeEfective(move.AMBase.Type, defender._base.Type1) * TypeChart.GetTypeEfective(move.AMBase.Type, defender._base.Type2);
        float modifiers = Random.Range(0.85f, 1f) * type * criticalDamage;
        float a = (2 * ULevel + 10) / 250f;
        float d = a * move.AMBase.Power * ((float)attack / defence) + 2;
        int dmg = Mathf.FloorToInt(d * modifiers);

        var damageDetails = new DamageDetails()
        {
            TypeEfect = type,
            CriticalDamage = criticalDamage,
            Dmg = dmg
        };

        defender.TakeDamage(dmg);
        if (defender.HP == 0)
        {
            defender.isdead = true;           
        }

        isdead = false;
        return damageDetails;
    }


    //enemy attack pattern
    public Unit_Moves GetRandomMove()
    {
        var TotalMoves = new List<Unit_Moves>();
        TotalMoves.AddRange(Unit_Physical_Moves);
        TotalMoves.AddRange(Unit_Magical_Moves);      
        
        int r = Random.Range(0, TotalMoves.Count);
        return TotalMoves[r];
    }

    //properties
    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }
    public int Defence
    {
        get { return GetStat(Stat.Defence);; }
    }
    public int MagicAttack
    {
        get { return GetStat(Stat.Magic);}
    }
    public int MagicDefence
    {
        get { return GetStat(Stat.MagicDef); }
    }
    public int ManaRegen
    {
        get { return GetStat(Stat.ManaRegen);}
    }
    public int MaxHP { get; private set; }
 
    public int MaxMana {get; private set; }

    //Moves status handle
    public void OnAfterTurn()
    {
        var regen = Mathf.FloorToInt((_base.ManaRegen * ULevel) / 100f + 5);
        Mana = Mathf.Clamp(Mana + regen, 0, MaxMana);
        Status?.OnAfterTurn?.Invoke(this); 
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }
    public bool OnBeforeMove()
    {
        bool canUseMove = true;
        if (Status?.OnBeforeMove != null)
        {
            if (!Status.OnBeforeMove(this))
            {
                canUseMove = false;
            }
        }
        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (!VolatileStatus.OnBeforeMove(this))
            {
                canUseMove = false;
            }
        }
        return canUseMove;                                                                   
    }

    //For Item usage
    public void TakeDamage(int dmg)
    {
        HP = Mathf.Clamp(HP - dmg, 0, MaxHP);
        if (HP == 0)
        {
            isdead = true;
        }
    }
    public void HealDamage(int healAmount)
    {
        HP = Mathf.Clamp((HP + healAmount), 0, MaxHP);
    }
    public void RegainMana(int manaAmount)
    {
        Mana = Mathf.Clamp((Mana + manaAmount), 0, MaxMana);
    }
    public void FullHeal()
    {
        HP = MaxHP;
        Mana = MaxMana;
        CureStatus();
        CureVolatileStatus();
    }
    public bool CheckMovePhysical(LeveldAttackMoves move)
    {
        if (move.AttMovBase.MovCatg == MoveCategory.Physical)
        {
            return true;
        }

        return false;
    }
}
//Save properties
public class DamageDetails
{
    public float CriticalDamage { get; set; }
    public float TypeEfect { get; set; }
    public int Dmg { get; set; }

}
public class HealDetails
{
    public float HealAmount { get; set; }
    public int TargetHP { get; set; }

}

[System.Serializable]
public class UnitsSaveData
{
    public string uniqueId;
    public int hp;
    public int mana;
    public int level;
    public int exp;
    public ConditionID? statusID;
    public List<Unit_Moves_Data> physicalMoves;
    public List<Unit_Moves_Data> magicalMoves;
}