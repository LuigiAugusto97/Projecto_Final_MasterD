using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTeamUI_Member : MonoBehaviour
{
    [SerializeField] Units _unit;

    [SerializeField] GameObject member;
    [Header("Unit Description")]
    [SerializeField] Text nameTxt;
    [SerializeField] Text lvlTxt;
    [SerializeField] Text hpTxt;
    [SerializeField] Text mpTxt;
    [Header("Selection Indicators")]
    [SerializeField] Image indecator= null;
    public bool isSelected;
    private void Awake()
    {
        member.SetActive(false);
    }
    public void SetPlayer(Units unit)
    {
        isSelected = false;
        this._unit = unit;
        if (_unit.isdead)
        {
            nameTxt.color = Color.red;
            lvlTxt.color = Color.red;
            hpTxt.color = Color.red;
            mpTxt.color = Color.red;
        }
        else
        {
            nameTxt.color = Color.black;
            lvlTxt.color = Color.black;
            hpTxt.color = Color.black;
            mpTxt.color = Color.black;
        }
        member.SetActive(true);
        nameTxt.text = _unit.UBase.Name;
        lvlTxt.text = ($"Lvl: {_unit.ULevel}");
        hpTxt.text = ($"HP: {_unit.HP}/{_unit.MaxHP}");
        mpTxt.text = ($"MP: {_unit.Mana}/{_unit.MaxMana}");
    }

    public void Indecate(bool selected)
    {
        if (isSelected)
        {
            indecator.color = Color.green;
            indecator.gameObject.SetActive(true);
        }
        else
        {
            indecator.color = Color.black;
            if (selected)
            {
                indecator.gameObject.SetActive(true);
            }
            else
            {
                indecator.gameObject.SetActive(false);
            }
        }

    }
}
