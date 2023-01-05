using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartySystem : MonoBehaviour
{
    [SerializeField] List<Units> party;

    public List<Units> Party
    {
        get { return party; }
        set { party = value; }
    }

    private void Start()
    {
        foreach (var unit in party)
        {
            unit.Init();
        }
    }

    public Units GetPlayer(int i)
    {
        if (i < party.Count)
        {
            return party[i];
        }
        else
        {
            return null;
        }
    }
    //Add a new party member
    public void AddUnit(Units unit)
    {
        var unitTobeAdd = party.FirstOrDefault(partyMemb => partyMemb == unit);
        if (unitTobeAdd == null)
        {
            party.Add(unit);
            unit.Init();
        }
        else
        {
            Debug.LogError("This member already exists");
        }
    }
    public void RemoveUnit(Units unit)
    {
        var unitTobeAdd = party.First(partyMemb => partyMemb == unit);
        if (unitTobeAdd == null)
        {
            Debug.LogError("This member does not exists");
        }
        else
        {
            party.Remove(unit);
        }
    }

    //Always find players Party
    public static List<Units> GetPlayerParty()
    {
        return FindObjectOfType<Player_CTRL>().GetComponent<PartySystem>().Party;
    }
}
