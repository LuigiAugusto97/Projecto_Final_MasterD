using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsBaseDB 
{
    static Dictionary<string, UnitBase> units_Data;

    //Get all SO units
    public static void GetUnits()
    {
        units_Data = new Dictionary<string, UnitBase>();

        var AllUnitsData = Resources.LoadAll<UnitBase>("");
        foreach (var unitData in AllUnitsData)
        {
            if(units_Data.ContainsKey(unitData.UniqueIdentifier))
            {
                Debug.LogError($"That unit,{unitData.UniqueIdentifier}, already exist"); 
                continue;
            }
            units_Data[unitData.UniqueIdentifier] = unitData;
        }
    }

    //If need to check on single unit
    public static UnitBase GetUnitByUniqueID(string uniqueId)
    {
        if (!units_Data.ContainsKey(uniqueId))
        {
            Debug.LogError($"There is no unit with id:{uniqueId}");
        }

        return units_Data[uniqueId];
    }
}
