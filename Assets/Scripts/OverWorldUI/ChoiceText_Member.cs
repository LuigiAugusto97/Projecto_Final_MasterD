using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceText_Member : MonoBehaviour
{
    [SerializeField] Text choiceText;
    public Text ChoiceText
    {
        get { return choiceText; }
        set { choiceText = value; } 
    }

    public void Indicate(bool isSelected)
    {
        if (isSelected)
        {
        choiceText.color = Color.blue;
        }
        else
        {
            choiceText.color = Color.black;
        }
    }
}
