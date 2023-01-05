using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIPlayer : MonoBehaviour
{
    [Header("PlayerConfigs")]
    public Image PlayerSprite;
    public Text PHealth;
    public Text PMana;  
    public Text Statustxt;

    [Header("EXPConfigs")]
    public GameObject playerEXP;
    public GameObject EXPbar;
    public Text EXPtxt;
    public Text LvlUPtext;
    private int _lvlNumber;

    [Header("UITargetConfigs")]
    public Text TargetName;
    public Button TargetButton;

    [Header("UIColorConfigs")]
    public Image Selected1;
    public Image Selected2;

    Dictionary<ConditionID, Color> StatusColors;

    private Units _unit;
    public Units Unit
    {
        get { return _unit; }
    }
    //Set the UI for the player
    public void SetPlayer(Units Unit)
    {
        _unit = Unit;
        if (_unit.isdead == false)
        {
            Selected1.color = Color.blue;
            Selected2.color = Color.blue;
            PlayerSprite.sprite = Unit.UBase.Unit_Sprite;
            TargetName.text = Unit.UBase.Name;
            TargetButton.enabled = true;
            PHealth.text = "HP:" + Unit.HP + "/" + Unit.MaxHP;
            PMana.text = "MP:" + Unit.Mana + "/" + Unit.MaxMana;

            //EXPDisplay
            playerEXP.SetActive(true);
            EXPtxt.text = _unit.UBase.Name;
            _lvlNumber = 0;
            LvlUPtext.enabled = false;
            SetEXP();

            StatusColors = new Dictionary<ConditionID, Color>()
            {
                {ConditionID.Par, Color.yellow },
                {ConditionID.Psn, new Color(148,0,211,1) },
                {ConditionID.Slp, Color.grey },
                {ConditionID.Brn, Color.red },
                {ConditionID.Frz, Color.cyan },
                {ConditionID.Bld, Color.black },
                {ConditionID.None,Color.clear }
            };
            SetStatusText();
            _unit.OnStatusChanged += SetStatusText;
        }
        else if (_unit.isdead)
        {
            TargetName.text = Unit.UBase.Name;
            PlayerSprite.sprite = Unit.UBase.Unit_Sprite;
            ZeroPlayer();
        }  
    }
    public void SetStatusText()
    {
        if (_unit.Status == null)
        {
            Statustxt.text = "";
        }
        else
        {
            Statustxt.text = _unit.Status.Id.ToString().ToUpper();
            Statustxt.color = StatusColors[_unit.Status.Id];
        }
    }

    public void ChangeSelectedColor(Color color)
    {
        Selected1.color = color;
        Selected2.color = color;
    }

    public void UpdatePlayerHPandMP()
    {
        PHealth.text = "HP:" + _unit.HP + "/" + _unit.MaxHP;
        PMana.text = "MP:" + _unit.Mana + "/" + _unit.MaxMana;
    }

    public void SetPlayerHP(int hp, Units Unit)
    {
        Unit.HP = hp;
        PHealth.text = "HP:" + Unit.HP + "/" + Unit.MaxHP;
    }

    public void SetPlayerMana(int mana, Units Unit)
    {
        Unit.Mana = mana;
        PMana.text = "MP" + Unit.Mana + "/" + Unit.MaxMana;
    }

    // To leave him Dead
    public void ZeroPlayer()
    {
        ChangeSelectedColor(Color.gray);
        SetPlayerHP(0, _unit);
        SetPlayerMana(0, _unit);
        playerEXP.SetActive(false);
    }
    public void TakeExpAway()
    {
        playerEXP.SetActive(false);
    }
 
    //Handle EXP UI
    public void SetEXP()
    {
        float normalizedEXP = GetNormalizedEXP();
        EXPbar.transform.localScale = new Vector3(normalizedEXP, 1, 1);

    }
    public IEnumerator GrowEXP(bool leveled = false)
    {
        if (leveled)
        {
            EXPbar.transform.localScale = new Vector3(0f, 1f, 1f);
            LvlUPtext.enabled = true;
            _lvlNumber++;
            LvlUPtext.text = $"+{_lvlNumber}";
        }
        float newEXP = GetNormalizedEXP();
        float currentEXP = EXPbar.transform.localScale.x;
        float changeAmout = newEXP - currentEXP;
        while (newEXP - currentEXP > Mathf.Epsilon)
        {
            currentEXP += changeAmout * Time.deltaTime;
            EXPbar.transform.localScale = new Vector3(currentEXP, 1f, 1f);
            yield return null;
        }
        EXPbar.transform.localScale = new Vector3(newEXP, 1f, 1f);
    }
    private float GetNormalizedEXP()
    {
        //Current and Max EXP
        int currentLvlExp = _unit.UBase.GetExpForLevel(_unit.ULevel);
        int nextLvlExp = _unit.UBase.GetExpForLevel(_unit.ULevel + 1);
        //Normalize it  and return 
        float normalizedEXP = (float)(_unit.EXP - currentLvlExp) / (nextLvlExp - currentLvlExp);
        return Mathf.Clamp01(normalizedEXP);
    }
}
