using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer_CRTL : MonoBehaviour
{
    [SerializeField] List<string> dialogChoices;
    public IEnumerator HealPlayerParty(Transform player, Dialog dialog)
    {
        int selected = 0;
        yield return DialogManager.Instance.ShowDialog(dialog, dialogChoices, (choiceIntex) => selected = choiceIntex);
        if (selected == 0)
        {
            yield return Fader.i.FadeIN(0.5f);
            var playerParty = player.GetComponent<PartySystem>();

            foreach (var unit in playerParty.Party)
            {
                unit.FullHeal();
                unit.isdead = false;
            }
            yield return Fader.i.FadeOUT(0.5f);

            yield return DialogManager.Instance.ShowDialogText("Your party has been fully healed");
        }
        else if (selected == 1)
        {
            yield return DialogManager.Instance.ShowDialogText("Okay maybe next time...");
        }

    }
}
