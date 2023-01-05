using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName ="Units/Create New Unit ")]
public class UnitBase : ScriptableObject
{
    [SerializeField] string _name;
    [SerializeField] string _uniqueIdentifier;
    [SerializeField] bool isboss;

    [SerializeField] Alignment alignment;

    [TextArea]
    [SerializeField] string description;
    [Header("Image")]
    [SerializeField] Sprite unit_sprite;
    [SerializeField] bool isBig;

    [SerializeField] UnitType type1;
    [SerializeField] UnitType type2;

    [Header("Stats")]
    [SerializeField] int maxHP;
    [SerializeField] int maxMana;
    [SerializeField] int strenght;
    [SerializeField] int defence;
    [SerializeField] int magic;
    [SerializeField] int magicDef;
    [SerializeField] int manaregen;
    
    [Header("Resitances")]
    [SerializeField] int psnResist;
    [SerializeField] int brnResist;
    [SerializeField] int slpResist;
    [SerializeField] int parResist;
    [SerializeField] int frzResist;
    [SerializeField] int confusionResist;

    [Header("EXP/Money")]
    [SerializeField] int expYield;
    [SerializeField] float moneyYield;


    [SerializeField] List<LeveldAttackMoves> levelAttMove_list;
    public static int MaxNumberOfMoves { get; set; } = 3;


    public int GetExpForLevel(int lvl)
    {
        if (lvl < 50)
        {
            return (6 / 5) * (lvl ^ 3) - 15 * (lvl ^ 2) + 100 * lvl - 140;
        }
        else
        {
            if (lvl == 100)
                return 0;
            else
                return lvl ^ 3;
        }       
    }

    public string Name
    {
        get { return _name; }
    }
    public string UniqueIdentifier
    {
        get { return _uniqueIdentifier; }
    }
    public bool IsBoss
    {
        get { return isboss; }
    }
    public string Description
    {
        get { return description; }
    }
    public Sprite Unit_Sprite
    {
        get { return unit_sprite; }
    }
    public bool IsBig
    {
        get { return isBig; }
    }
    public UnitType Type1
    {
        get { return type1; }
    }
    public UnitType Type2
    {
        get { return type2; }
    }
    public Alignment UnitAlignment
    {
        get { return alignment; }
    }
    public int MaxHP
    {
        get { return maxHP; }
    }
    public int Strenght
    {
        get { return strenght; }
    }
    public int Defence
    {
        get { return defence; }
    }
    public int Magic
    {
        get { return magic; }
    }
    public int MagicDef
    {
        get { return magicDef; }
    }
    public int ManaRegen
    {
        get { return manaregen; }
    }
    public int MaxMana
    {
        get { return maxMana; }
    }
    public int PsnResist
    {
        get { return psnResist; }
    }
    public int BrnResist
    {
        get { return brnResist; }
    }
    public int SlpResist
    {
        get { return slpResist; }
    }
    public int ParResist
    {
        get { return parResist; }
    }
    public int FrzResist
    {
        get { return frzResist; }
    }
    public int ConfusionResist
    {
        get { return confusionResist; }
    }
    public int ExpYield
    {
        get { return expYield; }
    }
    public float MoneyYield
    {
        get { return moneyYield; }
    }
    public List<LeveldAttackMoves> LevelAttMov
    {
        get { return levelAttMove_list; }
    }
}

[System.Serializable]
public class LeveldAttackMoves
{
    [SerializeField] AttackMovesBase attmovBase;
    [SerializeField] int level;

    public AttackMovesBase AttMovBase
    {
        get { return attmovBase; }
    }
    public int Level
    {
        get { return level; }
    }
}

public class TypeChart
{
    static float[][] chart =
    {
        //                         NOR    FIR    WAT    GRA    ELE    ICE    FHT    POI    GRD    FLY    PHY    GHO    DRA    DAR    STE           
        /*NOR*/     new float [] { 1.0f,  1.0f,  1.0f,  1.0F,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  0.0f,  1.0f,  1.0f,  0.5f },

        /*FIR*/     new float [] { 1.0f,  0.5f,  0.5f,  2.0F,  1.0f,  2.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  0.5f,  1.0f,  2.0f },

        /*WAT*/     new float [] { 1.0f,  2.0f,  0.5f,  0.5F,  1.0f,  1.0f,  1.0f,  1.0f,  2.0f,  1.0f,  1.0f,  1.0f,  0.5f,  1.0f,  1.0f },

        /*GRA*/     new float [] { 1.0f,  0.5f,  2.0f,  0.5F,  1.0f,  1.0f,  1.0f,  0.5f,  2.0f,  0.5f,  1.0f,  1.0f,  0.5f,  1.0f,  0.5f },

        /*ELE*/     new float [] { 1.0f,  1.0f,  2.0f,  0.5F,  0.5f,  1.0f,  1.0f,  1.0f,  0.0f,  2.0f,  1.0f,  1.0f,  0.5f,  1.0f,  1.0f },

        /*ICE*/     new float [] { 1.0f,  0.5f,  0.5f,  2.0F,  1.0f,  0.5f,  1.0f,  1.0f,  2.0f,  2.0f,  1.0f,  1.0f,  2.0f,  1.0f,  0.5f },

        /*FHT*/     new float [] { 2.0f,  1.0f,  1.0f,  1.0F,  1.0f,  2.0f,  1.0f,  0.5f,  1.0f,  0.5f,  0.5f,  0.0f,  1.0f,  2.0f,  2.0f },

        /*POI*/     new float [] { 1.0f,  1.0f,  1.0f,  2.0F,  1.0f,  1.0f,  1.0f,  0.5f,  0.5f,  1.0f,  1.0f,  0.5f,  1.0f,  1.0f,  0.0f },

        /*GRD*/     new float [] { 1.0f,  2.0f,  1.0f,  0.5F,  2.0f,  1.0f,  1.0f,  2.0f,  1.0f,  0.0f,  1.0f,  1.0f,  1.0f,  1.0f,  2.0f },

        /*FLY*/     new float [] { 1.0f,  1.0f,  1.0f,  2.0F,  0.5f,  1.0f,  2.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  0.5f },

        /*PHY*/     new float [] { 1.0f,  1.0f,  1.0f,  1.0F,  1.0f,  1.0f,  2.0f,  2.0f,  1.0f,  1.0f,  0.5f,  1.0f,  1.0f,  0.0f,  0.5f },

        /*GHO*/     new float [] { 0.0f,  1.0f,  1.0f,  1.0F,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  2.0f,  2.0f,  1.0f,  0.5f,  1.0f },

        /*DRA*/     new float [] { 1.0f,  1.0f,  1.0f,  1.0F,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  2.0f,  1.0f,  0.5f },

        /*DAR*/     new float [] { 1.0f,  1.0f,  1.0f,  1.0F,  1.0f,  1.0f,  1.0f,  0.5f,  1.0f,  1.0f,  2.0f,  2.0f,  1.0f,  0.5f,  1.0f },

        /*STE*/     new float [] { 1.0f,  0.5f,  0.5f,  1.0F,  0.5f,  2.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  0.5f }

    };

    public static float GetTypeEfective(UnitType attackType, UnitType defenceType)
    {
        if (attackType == UnitType.None || defenceType == UnitType.None)
        {
            return 1F;  
        }

        int row = (int)attackType - 1;
        int col = (int)defenceType - 1;

        return chart[row][col];
    }
}
public enum Stat
{
    Attack,
    Defence,
    Magic,
    MagicDef,
    ManaRegen,

    //Not effective stats, only to use for moves
    Accuracy,
    Evasion
}
public enum Alignment
{
    Teammate,
    Enemy
}
public enum UnitType
{
    None,
    Normal,
    Fire,
    Water,
    Grass,
    Eletric,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Phychic,
    Ghost,
    Dragon,
    Dark,
    Steel
}


