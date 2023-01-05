using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceTextBox_CTRL : MonoBehaviour
{
    //HandleUI Updates
    private List<ChoiceText_Member> totalChoices;
    private int currentChoice;

    [SerializeField] ChoiceText_Member ChoiceText_prefab;
    private bool choiceSelected;
    public IEnumerator ShowChoices(List<string> possibleChoices, Action<int> onChoiceSelected)
    {
        choiceSelected = false;
        currentChoice = 0;
        gameObject.SetActive(true);

        //Delete all choices
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        totalChoices = new List<ChoiceText_Member>();
        //Spawn all new choices
        foreach ( var choice in possibleChoices)
        {
            var childObject = Instantiate(ChoiceText_prefab, transform);
            childObject.ChoiceText.text = choice;
            totalChoices.Add(childObject);
        }
        yield return new WaitUntil(() => choiceSelected == true);
        onChoiceSelected?.Invoke(currentChoice);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            currentChoice++;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            currentChoice--;
        }
        currentChoice = Mathf.Clamp(currentChoice, 0, totalChoices.Count - 1);

        for (int i = 0; i < totalChoices.Count; i++)
        {
            totalChoices[i].Indicate(i == currentChoice);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            choiceSelected = true;
        }
    }
}
