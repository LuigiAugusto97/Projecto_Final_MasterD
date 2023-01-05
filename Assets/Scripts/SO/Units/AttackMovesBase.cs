using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Move", menuName = "Units/Create New Attack Move ")]
public class AttackMovesBase : ScriptableObject
{
    [SerializeField] string _name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] UnitType type;
    [SerializeField] MoveCategory movecategory;
    [SerializeField] MoveDescriver movedescription;
    [SerializeField] MoveEffects effects;
    [SerializeField] List<SecondaryEffects> secondaryEffects;
    [SerializeField] MoveTarget target;

    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] bool alwaysHit;
    [SerializeField] int resourceCost;

    [SerializeField] AudioClip moveSound;
    public string Name
    {
        get { return _name; }
    }
    public string Description
    {
        get { return description; }
    }
    public UnitType Type
    {
        get { return type; }
    }
    public MoveCategory MovCatg
    {
        get { return movecategory; }
    }
    public MoveDescriver MovDescriv
    {
        get { return movedescription; }
    }
    public MoveEffects Effects
    {
        get { return effects; }
    }public List<SecondaryEffects> SecondaryEffects
    {
        get { return secondaryEffects; }
    }
    public MoveTarget Target
    {
        get { return target; }
    }
    public int Power
    {
        get { return power; }
    }
    public int Accuracy
    {
        get { return accuracy; }
    }
    public bool AlwaysHit
    {
        get { return alwaysHit; }
    }
    public int ResourceCost
    {
        get { return resourceCost; }
    }
    public AudioClip MoveSound
    {
        get { return moveSound;}
    }
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatChanges> boosts;
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volatilestatus;

    public List<StatChanges> Boosts
    {
        get { return boosts; }
    }

    public ConditionID Status
    {
        get { return status; }
    }
    public ConditionID VolatileStatus
    {
        get { return volatilestatus; ; }
    }
}
[System.Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;
    
    public int Chance
    {
        get { return chance; }
    }

    public MoveTarget Target
    {
        get { return target; }
    }
} 

[System.Serializable]
public class StatChanges
{
    public Stat stat;
    public int boost;
}

public enum MoveCategory
{
    Physical,
    Magical
}
public enum MoveDescriver
{
    Normal,
    Status,
    Heal,
    Revive,
    Cure
}

public enum MoveTarget
{
    Foe,
    Friendly,
    Self
}