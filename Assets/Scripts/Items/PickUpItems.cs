using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItems : MonoBehaviour, IInteractables, ISavable
{
    [SerializeField] ItemBase itemToPickUP;
    [SerializeField] int count = 1;
    public bool PickedUp { get; set; } = false;
    public IEnumerator Interact(Transform Character)
    {
        //Check if it was previously picked up
        if (!PickedUp)
        {
            Character.GetComponent<Inventory>().AddItem(itemToPickUP, count);
            PickedUp = true;
            //disable components
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;

            string playerName = Character.GetComponent<Player_CTRL>().Name;
            string dialogtext = $"{playerName} aquired a {itemToPickUP.Name}!";

            //PlayPickup Music
            AudioManager.i.PlaySFX(AudioId.ItemGained, true);

            if (count > 1)
            {
                dialogtext = $"{playerName} aquired x{count} {itemToPickUP.Name}!";
            }
            yield return DialogManager.Instance.ShowDialogText(dialogtext);
        }
    }

    //SAVE SYSTEM
    public object SaveData()
    {
        return PickedUp;
    }
    public void LoadData(object data)
    {
        PickedUp = (bool)data;

        if (PickedUp)
        {
            //disable components
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
