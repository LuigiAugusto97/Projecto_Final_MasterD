using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AdditivePortal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] Transform spawnPoint;

    [SerializeField] SpawnerID id;
    private Player_CTRL player;

    public bool TriggerManyTimes => false;
    public void OnPlayerTriggered(Player_CTRL player)
    {
        this.player = player;
        StartCoroutine(GoToAdditivePortal());
    }

    IEnumerator GoToAdditivePortal()
    {    
        GameCTRL.Instance.PauseGame(true);
        yield return Fader.i.FadeIN(.5f);
        var _destionation = FindObjectsOfType<AdditivePortal>().First(x => x != this && x.id == this.id);
        player.Char.SetToTile(_destionation.SpawnPoint.position);
        yield return Fader.i.FadeOUT(.5f);
        GameCTRL.Instance.PauseGame(false);

    }

    public Transform SpawnPoint
    {
        get { return spawnPoint; }
    }
}
