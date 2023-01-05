using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] List<SceneLoader> adjacentScenes;
    [SerializeField] AudioClip musicOfScene;
    [SerializeField] AudioClip musicOfBattle;
    public AudioClip MusicOfBattle
    {
        get { return musicOfBattle; }
    }
    public AudioClip MusicOfScene
    {
        get { return musicOfScene; }
    }

    public bool DontStopMusic;
    private List<SavableEntity> saveData;

    public bool isLoaded { get; private set; }


    //handle Scene Enter
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LoadSceneAdditive();
            GameCTRL.Instance.SetCurrentScene(this);            

            if(musicOfScene != null && GameCTRL.Instance.StopPlayingMusic())
                AudioManager.i.PlayMusic(musicOfScene, fadeMusic: true);

            //Load All adjacentScenes
            foreach (var scene in adjacentScenes)
            {
                scene.LoadSceneAdditive();
            }
            //Unload Not adjecenteScenes
            if (GameCTRL.Instance.PreviousScene != null)
            {
                var prevAdjacentScenes = GameCTRL.Instance.PreviousScene.adjacentScenes;
                foreach (var scene in prevAdjacentScenes)
                {
                    if (!adjacentScenes.Contains(scene) && scene != this)
                    {
                        scene.UnLoadSceneAdditive();
                    }
                }
                if (!adjacentScenes.Contains(GameCTRL.Instance.PreviousScene))
                {
                    GameCTRL.Instance.PreviousScene.UnLoadSceneAdditive();
                }
            }
        }
    }

    //Load and unload Scene additivatly
    public void LoadSceneAdditive()
    {
        if (!isLoaded)
        {
            var operation = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            isLoaded = true;

            operation.completed += (AsyncOperation op) =>
            {
                saveData = GetSavableEntitiesInScene();

                //Restore all the changed data into the scene
                SavingSystem.i.RestoreEntityData(saveData);
            };

        }
    }
    public void UnLoadSceneAdditive()
    {
        if (isLoaded)
        {
            //Obtain all the data changed in the scene
            SavingSystem.i.CaptureEntityData(saveData);

            SceneManager.UnloadSceneAsync(gameObject.name);
            isLoaded = false;
        }
    }

    //Funttion to retrive all savable entites in the scene before unloading
    private List<SavableEntity> GetSavableEntitiesInScene()
    {
        var currentScene = SceneManager.GetSceneByName(gameObject.name);
        var savableSceneObjects = FindObjectsOfType<SavableEntity>().Where(x => x.gameObject.scene == currentScene).ToList();

        return savableSceneObjects;
    }

}
