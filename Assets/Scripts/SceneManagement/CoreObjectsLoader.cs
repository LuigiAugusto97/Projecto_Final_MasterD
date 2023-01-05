using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreObjectsLoader : MonoBehaviour
{
    [SerializeField] GameObject coreObjectsPrefab;
    [SerializeField] bool fromMainMenu = false;
    private void Awake()
    {
        var existingObjects = FindObjectsOfType<CoreObjects>();
        if (existingObjects.Length == 0)
        {
            var spawnPos = new Vector3(0, 0, 0);
            //if There is a grid Spawn there
            if (fromMainMenu)
            {
                //If loaded from main menu
                spawnPos = new Vector3(81, -65, 0);
            }


            var grid = FindObjectOfType<Grid>();
            if (grid != null)
                spawnPos = grid.transform.position;

            Instantiate(coreObjectsPrefab, spawnPos, Quaternion.identity);
        }            
    }
}
