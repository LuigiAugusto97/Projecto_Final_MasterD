using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavingSystem : MonoBehaviour
{
    public static SavingSystem i { get; private set; }
    private void Awake()
    {
        i = this;
    }

    //Place to save data of all objects that have been changed
    Dictionary<string, object> gameData = new Dictionary<string, object>();

    public void CaptureEntityData(List<SavableEntity> savableEntities)
    {
        foreach (SavableEntity savable in savableEntities)
        {
            gameData[savable.UniqueId] = savable.CaptureData();
        }
    }

    public void RestoreEntityData(List<SavableEntity> savableEntities)
    {
        foreach (SavableEntity savable in savableEntities)
        {
            string id = savable.UniqueId;
            if (gameData.ContainsKey(id))
                savable.RestoreData(gameData[id]);
        }
    }

    public void Save(string saveFile)
    {
        CaptureDataAll(gameData);
        SaveFile(saveFile, gameData);
    }

    public void Load(string saveFile)
    {
        gameData = LoadFile(saveFile);
        RestoreDataAll(gameData);
    }

    public void Delete(string saveFile)
    {
        File.Delete(GetPath(saveFile));
    }

    // To capture data of all savable objects in the game
    private void CaptureDataAll(Dictionary<string, object> state)
    {
        foreach (SavableEntity savable in FindObjectsOfType<SavableEntity>())
        {
            state[savable.UniqueId] = savable.CaptureData();
        }
    }

    // To restore data of all savable objects in the game
    private void RestoreDataAll(Dictionary<string, object> state)
    {
        foreach (SavableEntity savable in FindObjectsOfType<SavableEntity>())
        {
            string id = savable.UniqueId;
            if (state.ContainsKey(id))
                savable.RestoreData(state[id]);
        }
    }

    void SaveFile(string saveFile, Dictionary<string, object> state)
    {
        string path = GetPath(saveFile);
        print($"saving to {path}");

        using (FileStream fs = File.Open(path, FileMode.Create))
        {
            // Serialize object
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(fs, state);
        }
    }

    Dictionary<string, object> LoadFile(string saveFile)
    {
        string path = GetPath(saveFile);
        if (!File.Exists(path))
            return new Dictionary<string, object>();

        using (FileStream fs = File.Open(path, FileMode.Open))
        {
            // Deserialize object
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            return (Dictionary<string, object>)binaryFormatter.Deserialize(fs);
        }
    }

    private string GetPath(string saveFile)
    {
        return Path.Combine(Application.persistentDataPath, saveFile);
    }
}
