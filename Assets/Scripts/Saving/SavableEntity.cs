using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class SavableEntity : MonoBehaviour
{
    [SerializeField] string uniqueId = "";
    static Dictionary<string, SavableEntity> globalLookup = new Dictionary<string, SavableEntity>();

    public string UniqueId
    {
        get { return uniqueId; }
    }

    //To gather the data of savableEntity gameObject
    public object CaptureData()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        foreach (ISavable savable in GetComponents<ISavable>())
        {
            data[savable.GetType().ToString()] = savable.SaveData();
        }
        return data;
    }

    // To return the data of the savableEntity gameObject
    public void RestoreData(object data)
    {
        Dictionary<string, object> dataDict = (Dictionary<string, object>)data;
        foreach (ISavable savable in GetComponents<ISavable>())
        {
            string id = savable.GetType().ToString();

            if (dataDict.ContainsKey(id))
                savable.LoadData(dataDict[id]);
        }
    }

#if UNITY_EDITOR
    // Creating uniqueID for SavableEntity
    private void Update()
    {
        // don't execute in playmode
        if (Application.IsPlaying(gameObject)) return;

        // don't generate Id for prefabs
        if (String.IsNullOrEmpty(gameObject.scene.path)) return;

        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty property = serializedObject.FindProperty("uniqueId");

        if (String.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
        {
            property.stringValue = Guid.NewGuid().ToString();
            serializedObject.ApplyModifiedProperties();
        }

        globalLookup[property.stringValue] = this;
    }
#endif
    //Functio to check SavableEntity is unique
    private bool IsUnique(string candidate)
    {
        if (!globalLookup.ContainsKey(candidate)) return true;

        if (globalLookup[candidate] == this) return true;

        // Handle scene unloading cases
        if (globalLookup[candidate] == null)
        {
            globalLookup.Remove(candidate);
            return true;
        }

        // Handle edge cases like designer manually changing the UUID
        if (globalLookup[candidate].UniqueId != candidate)
        {
            globalLookup.Remove(candidate);
            return true;
        }

        return false;
    }
}