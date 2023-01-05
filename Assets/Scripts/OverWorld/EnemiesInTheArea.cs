using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesInTheArea : MonoBehaviour
{
    [SerializeField] List<Units> possibleEnemies;
    public int minLvl;
    public int maxLvl;

    public Sprite backGround;

    public Units Enemy1;
    public Units Enemy2;
    public Units Enemy3;

    public Units GetRandomEnemie()
    {
        var enemie = possibleEnemies[Random.Range(0, possibleEnemies.Count)];
        enemie.isdead = false;
        possibleEnemies.Remove(enemie);
        enemie.ULevel = Random.Range(minLvl, maxLvl+1);
        enemie.Init();
        return enemie;
    }

    public void Setup()
    {
        Enemy1 = GetRandomEnemie();
        Enemy2 = GetRandomEnemie();
        Enemy3 = GetRandomEnemie();
        possibleEnemies.Add(Enemy1);
        possibleEnemies.Add(Enemy2);
        possibleEnemies.Add(Enemy3);
    }
}
