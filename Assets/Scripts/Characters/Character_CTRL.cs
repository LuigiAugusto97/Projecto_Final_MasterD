using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_CTRL : MonoBehaviour
{
    public float moveSpeed;
    public bool IsMoving { get; private set; }
    public Vector2 PreviousTile { get; private set; }

    private CharacterAnimator _npcAnim;


    private void Awake()
    {
        _npcAnim = GetComponent<CharacterAnimator>();
        SetToTile(transform.position);
    }

    public IEnumerator MoveTo(Vector2 TargerVector, Action OnMoveOver = null)
    {
        PreviousTile = TargerVector * new Vector2(-1, -1);

        // Calculate Target Position with a vector instead of a input
        _npcAnim.Move_x = Mathf.Clamp(TargerVector.x, -1f, 1f);
        _npcAnim.Move_y = Mathf.Clamp(TargerVector.y, -1f, 1f);
        Vector3 targetPos = transform.position;
        targetPos.x += TargerVector.x;
        targetPos.y += TargerVector.y;

        if (!isTargetClear(targetPos))
        {
            yield break;
        }

        //Start moving
        IsMoving = true;


        //Verificar se existe difereça entre a posição de destino e a atual
        while (Vector3.Distance(transform.position, targetPos) > Mathf.Epsilon)
        {
            //Enquanto verdado movimentar o Player para o seu posição de destino
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime); ;

            yield return null;
        }
        //Igualar a posição para não criar pequenas divergências
        transform.position = targetPos;

        //Finalizar o Movimento
        IsMoving = false;

        //Do a possibleAction
        OnMoveOver?.Invoke();
    }
    //Make the character always on center of tile
    public void SetToTile(Vector2 position)
    {
        position.x = Mathf.Floor(position.x) + 0.5f;
        position.y = Mathf.Floor(position.y) + 0.8f;
        transform.position = position;
    }

    public void HandleUpdate()
    {
        _npcAnim.isMoving = IsMoving;
    }
    private bool CanWalk(Vector3 TargetPos)
    {
        if (Physics2D.OverlapCircle(TargetPos, 0.18f, GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer) != null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    private bool isTargetClear(Vector3 TargetPos)
    {
        var targetDiff = TargetPos - transform.position;
        var dir = targetDiff.normalized;

        if (Physics2D.BoxCast(transform.position + dir, new Vector2(0.18f, 0.18f), 0f, dir, targetDiff.magnitude - 1, GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer | GameLayers.i.PlayerLayer) == true)
        {
            //Path not Clear
            return false;
        }
        //Path true
        return true;
    }
    public void LookAt(Vector3 target)
    {
        //Calculate the distance of tiles between target
        var xDiff = Mathf.Floor(target.x) - Mathf.Floor(transform.position.x);
        var yDiff = Mathf.Floor(target.y) - Mathf.Floor(transform.position.y);
        //Check if target is in a horizontal or vertical position in relation to the NPC
        if (xDiff == 0 || yDiff == 0)
        {
            _npcAnim.Move_x = Mathf.Clamp(xDiff, -1f, 1f);
            _npcAnim.Move_y = Mathf.Clamp(yDiff, -1f, 1f);
        }
    }


    public CharacterAnimator NPCANIM
    {
        get { return _npcAnim; }
    }
}
public enum NPCState { Idle, Walk, Dialog }
