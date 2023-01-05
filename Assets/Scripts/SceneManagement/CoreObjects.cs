using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreObjects : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
