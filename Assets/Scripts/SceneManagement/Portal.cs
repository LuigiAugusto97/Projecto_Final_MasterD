using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] int sceneToLoad = -1;

    [SerializeField] SpawnerID id;
    private Player_CTRL player;

    public bool TriggerManyTimes => false;
    public void OnPlayerTriggered(Player_CTRL player)
    {
        this.player = player;
        StartCoroutine(SwitchScene());
    }

    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);
        GameCTRL.Instance.PauseGame(true);
        yield return Fader.i.FadeIN(.5f);
        yield return SceneManager.LoadSceneAsync(sceneToLoad);
        var _destination = FindObjectsOfType<Portal>().First(x => x != this && x.id == this.id);
        player.Char.SetToTile(_destination.SpawnPoint.position);
        yield return Fader.i.FadeOUT(.5f);
        GameCTRL.Instance.PauseGame(false);
        Destroy(gameObject);
    }

    public Transform SpawnPoint
    {
        get { return spawnPoint; }
    }
}
public enum SpawnerID { A, B, C, D, E }
