using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCGivesItem : MonoBehaviour, ISavable
{
    [SerializeField] ItemBase ItemToGive;
    [SerializeField] int count = 1;
    private bool itemWasGiven = false;

    [Header("Dialog Options")]
    [SerializeField] Dialog dialog;

    //To give item to player
    public IEnumerator GiveItem(Player_CTRL player)
    {
        yield return DialogManager.Instance.ShowDialog(dialog);
        player.GetComponent<Inventory>().AddItem(ItemToGive, count);
        itemWasGiven = true;

        //PlayPickup Music
        AudioManager.i.PlaySFX(AudioId.ItemGained, true);

        string dialogtext = $"{player.Name} aquired a {ItemToGive.Name}!";
        if (count > 1)
        {
            dialogtext = $"{player.Name} aquired x{count} {ItemToGive.Name}!";
        }
        yield return DialogManager.Instance.ShowDialogText(dialogtext);
    }

    public bool CanGive()
    {
        return ItemToGive != null && !itemWasGiven && count > 0;
    }


    //SAVE SYSTEM
    public object SaveData()
    {
        return itemWasGiven;
    }

    public void LoadData(object data)
    {
        itemWasGiven = (bool)data;
    }
}
