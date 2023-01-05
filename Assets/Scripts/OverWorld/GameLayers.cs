using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectsMask;
    [SerializeField] LayerMask interactableObjectMask;
    [SerializeField] LayerMask encounterObjectMask;
    [SerializeField] LayerMask playerObjectMask;
    [SerializeField] LayerMask portalObjectMask;
    [SerializeField] LayerMask storyTriggersObjectMask;

    public static GameLayers i { get; set; }
    private void Awake()
    {
        i = this;
    }

    public LayerMask SolidLayer
    {
        get { return solidObjectsMask; }
    }
    public LayerMask InteractableLayer
    {
        get { return interactableObjectMask; }
    }
    public LayerMask EncounterLayer
    {
        get { return encounterObjectMask; }
    }
    public LayerMask PlayerLayer
    {
        get { return playerObjectMask; }
    }
    public LayerMask PortalLayer
    {
        get { return portalObjectMask; }
    }
    public LayerMask StoryTriggersLayer
    {
        get { return storyTriggersObjectMask; }
    }
    public LayerMask TriggerLayers
    {
        get { return portalObjectMask | encounterObjectMask | storyTriggersObjectMask; }
    }
}
