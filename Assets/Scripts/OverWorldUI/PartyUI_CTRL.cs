using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyUI_CTRL : MonoBehaviour
{
    [Header("MenuOptions")]
    [SerializeField] GameObject menu;
    [SerializeField] List<PlayerTeamUI_Member> partyMembers;

    private List<Units> playersParty;


    private Units selectedUnit;
    public Units SelectedUnit
    {
        get { return playersParty[selected]; }
    }
    private int selectedUnitIndex;

    private int selected;

    public event Action onMenuBack;
    private void Awake()
    {
        playersParty = PartySystem.GetPlayerParty();
    }
    public void HandleMenu()
    {
        selected = 0;
        if (menu.activeSelf == false)
        {
            menu.SetActive(true);
        }
        else if (menu.activeSelf)
        {
            menu.SetActive(false);
            onMenuBack?.Invoke();
        }
    }
    public void HandleUpdate(Action onSelected, Action onBack)
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ++selected;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            --selected;
        }
        selected = Mathf.Clamp(selected, 0, (playersParty.Count - 1));
        HandleUIUpdate();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            onSelected?.Invoke();       
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            onBack?.Invoke();
        }
    }
    private void HandleUIUpdate()
    {
        SetParty();
        for (int i = 0; i < playersParty.Count; i++)
        {
            if (i == selected)
            {
                partyMembers[i].Indecate(true);
            }
            else
            {
                partyMembers[i].Indecate(false);
            }
        }
    }
    public void ChangePartyMember()
    {
        if (!partyMembers[selected].isSelected && selectedUnit == null)
        {
            partyMembers[selected].isSelected = true;
            selectedUnitIndex = selected;
            selectedUnit = playersParty[selected];
        }
        else if (partyMembers[selected].isSelected)
        {
            partyMembers[selected].isSelected = false;
            selectedUnit = null;
        }
        else if (!partyMembers[selected].isSelected && selectedUnit != null)
        {
            playersParty[selectedUnitIndex] = playersParty[selected];
            playersParty[selected] = selectedUnit;

            partyMembers[selectedUnitIndex].isSelected = false;
            selectedUnit = null;
            SetParty();
        }
    }
    public void SetParty()
    {
        for (int i = 0; i < playersParty.Count; i++)
        {
            partyMembers[i].SetPlayer(playersParty[i]);
        }
    }
}
