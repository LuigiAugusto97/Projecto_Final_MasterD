using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIEnemy : MonoBehaviour
{
    [Header("EnemyConfigs")]

    public Image BigEnemySprite;
    public Image SmallEnemySprite;
    public Text EnemyName;
    public Text TargetName;
    public Text Statustxt;
    public Button TargetButton;
    public Text EnemyLvl;
    public Text EnemyHP;

    public GameObject Fullenemy;

    Dictionary<ConditionID, Color> StatusColors;

    Units _unit;
    public void SetEnemy(Units Unit)
    {
        _unit = Unit;
        Fullenemy.SetActive(true);
        TargetName.text = Unit.UBase.Name;
        TargetButton.enabled = true;
        if (_unit.UBase.IsBig)
        {
            BigEnemySprite.sprite = Unit.UBase.Unit_Sprite;
            BigEnemySprite.gameObject.SetActive(true);
            SmallEnemySprite.gameObject.SetActive(false);
        }
        else
        {
            SmallEnemySprite.sprite = Unit.UBase.Unit_Sprite;
            SmallEnemySprite.gameObject.SetActive(true);
            BigEnemySprite.gameObject.SetActive(false);
        }
        SmallEnemySprite.sprite = Unit.UBase.Unit_Sprite;
        EnemyName.text = Unit.UBase.Name;
        EnemyHP.text = "HP:" + Unit.HP + "/" + Unit.MaxHP;
        EnemyLvl.text = "Lvl:" + Unit.ULevel;

        StatusColors = new Dictionary<ConditionID, Color>()
        {
            {ConditionID.Par, Color.yellow },
            {ConditionID.Psn, new Color(148,0,211,1) },
            {ConditionID.Slp, Color.grey },
            {ConditionID.Brn, Color.red },
            {ConditionID.Frz, Color.cyan },
            {ConditionID.None,Color.clear }
        };
        SetStatusText();
        _unit.OnStatusChanged += SetStatusText;
        if (_unit.isdead)
        {
            ZeroEnemy();
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

    public void UpdateEnemyHP()
    {
        EnemyHP.text = "HP:" + _unit.HP + "/" + _unit.MaxHP;
    }

    public void SetEnemyHP(int hp, Units Unit)
    {
        EnemyHP.text = "HP:" + hp + "/" + Unit.MaxHP;
    }

    public void ZeroEnemy()
    {
        Fullenemy.SetActive(false);
        BigEnemySprite.gameObject.SetActive(true);
        SmallEnemySprite.gameObject.SetActive(true);
        TargetName.text = "-";
        TargetButton.enabled = false;
    }
}
