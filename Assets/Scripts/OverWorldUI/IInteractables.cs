using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractables
{
    IEnumerator Interact(Transform Character);
}
