using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [SerializeField] GameObject dialogBox;
    [SerializeField] ChoiceTextBox_CTRL choiceTextBox;  
    [SerializeField] Text dialogText;
    [SerializeField] int LettersperSecound = 50;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;


    public bool isTalking { get; private set; } 

    private void Awake()
    {
        Instance = this;   
    }
    //For Single use without dialog
    public IEnumerator ShowDialogText(String text, bool waitForInput= true, bool autoClose = true, List<string> Choices = null, Action<int> OnChoiceSelected = null)
    {
        OnShowDialog?.Invoke();
        isTalking = true;
        dialogBox.SetActive(true);

        AudioManager.i.PlaySFX(AudioId.UIManage);
        yield return StartCoroutine(TypeDialog(text));
        if (waitForInput)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }
        if (Choices != null && Choices.Count > 1)
        {
            yield return choiceTextBox.ShowChoices(Choices, OnChoiceSelected);
        }
        if (autoClose)
        {
            isTalking = false;
            dialogBox.SetActive(false);
            OnCloseDialog?.Invoke();
        }
    }

    //For multiple line of dialog
    public IEnumerator ShowDialog(Dialog dialog, List<string> Choices = null, Action<int> OnChoiceSelected = null)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();
        isTalking = true;

        dialogBox.SetActive(true);

        foreach (var line in dialog.Lines)
        {
            AudioManager.i.PlaySFX(AudioId.UIManage);
            yield return StartCoroutine(TypeDialog(line));
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }
        if (Choices != null && Choices.Count > 1)
        {
            yield return choiceTextBox.ShowChoices(Choices, OnChoiceSelected);
        }
        dialogBox.SetActive(false);
        isTalking = false;
        OnCloseDialog?.Invoke();

    }
    public void HandleUpdate()
    {

    }
 
    public IEnumerator TypeDialog(string line)
    {
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / LettersperSecound);
        }
    }
    public void CloseDialog()
    {
        isTalking = false;
        dialogBox.SetActive(false);
        OnCloseDialog?.Invoke();
    }
}
