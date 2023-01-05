using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player_CTRL : MonoBehaviour, ISavable
{
    [Header("Name Options")]
    [SerializeField] string _name;
    public String Name
    {
        get { return _name; }
        set { _name = value; }
    }

    //Check if trigger can be reapeted
    private IPlayerTriggerable currentTrigger;

    //For Managing movement
    private Vector2 _input;

    private Character_CTRL _char;
    public Character_CTRL Char
    {
        get{ return _char; }
    }

    private void Awake()
    {
        _char = GetComponent<Character_CTRL>();
        _char.SetToTile(transform.position);
    }

    public void HandleUpdate()
    {  
        if (!_char.IsMoving)
        {
            _input.x = Input.GetAxisRaw("Horizontal");
            _input.y = Input.GetAxisRaw("Vertical");

            //Remove diagonal 
            if (_input.x != 0)
            {
                _input.y = 0;
            }
            if (_input != Vector2.zero)
            {
               StartCoroutine(_char.MoveTo(_input,OnMoveOver));
            }
        }
        _char.HandleUpdate();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Interact());
        }
    }
    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, 0.18f, GameLayers.i.TriggerLayers);

        IPlayerTriggerable trigger = null;
        foreach (var collider in colliders)
        {
             trigger = collider.GetComponent<IPlayerTriggerable>();
            //Check what type of collinder and handle action
            if (trigger != null)
            {
                StoryTrigger storyTrigger = collider.gameObject.GetComponent<StoryTrigger>();
                if (storyTrigger != null && storyTrigger.blocksMovement)
                {
                    StartCoroutine(_char.MoveTo(_char.PreviousTile, OnMoveOver));
                }
                if (trigger == currentTrigger && !trigger.TriggerManyTimes)
                {
                    break;
                }

                _char.NPCANIM.isMoving = false;
                trigger.OnPlayerTriggered(this);
                currentTrigger = trigger;
                break;
            }
        }

        if (colliders.Count() == 0 || trigger != currentTrigger)
        {
            currentTrigger = null;
        }
    }
    //Function to check for a interaction
    private IEnumerator Interact()
    {
        //Calculate the direction
        var direction = new Vector3(_char.NPCANIM.Move_x, _char.NPCANIM.Move_y);
        var interactPos = transform.position + direction;
        //Debug.DrawLine(transform.position, interactPos, Color.red, 0.5f);
        //Check for collider with the mask to interact
        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);
        if (collider != null)
        {
            //Apply the interact function for the object
            yield return collider.GetComponent<IInteractables>()?.Interact(transform);
        }
    }


    //SAVE SYSTEM 
    public object SaveData()
    {
        var saveData = new PlayerSaveData()
        {
            position = new float[] { transform.position.x, transform.position.y },
            partyUnits = GetComponent<PartySystem>().Party.Select(x => x.GetSavedData()).ToList()
        };

        return saveData;
    }

    public void LoadData(object data)
    {
        var saveData = (PlayerSaveData)data;
        
        //Load position 
        var pos = saveData.position;
        transform.position = new Vector3(pos[0], pos[1]);
        _char.SetToTile(transform.position);

        //load party
        GetComponent<PartySystem>().Party = saveData.partyUnits.Select(x => new Units(x)).ToList();
    }
}
//Class to hold all the data to save from this class
[Serializable]
public class PlayerSaveData
{
    public float[] position;
    public List<UnitsSaveData> partyUnits;
}