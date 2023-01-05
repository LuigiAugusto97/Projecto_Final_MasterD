using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conditions
{
    public ConditionID Id { get; set; }
    public string Name { get; set; }
    public string Descriptions { get; set; }
    public string StartMessage { get; set; }

    public Action<Units> OnStart { get; set; }
    public Action<Units> OnAfterTurn { get; set; }
    public Func<Units,bool> OnBeforeMove { get; set; }
}
