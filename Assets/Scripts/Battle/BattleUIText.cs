using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIText : MonoBehaviour
{
    public int LettersperSecound = 50;
    [SerializeField] Text dialogText;
    [SerializeField] Text InformationText;

    public GameObject InitialBox;
    public List<Text> InitialBoxTexts;

    public GameObject FightBox;
    public List<Text> FightBoxTexts;

    public GameObject MagicBox;
    public List<Text> MagicBoxTexts;
    
    public GameObject ItemBox;
    public List<Text> ItemBoxTexts;

    public GameObject EnemyBox;
    public List<Text> EnemyBoxTexts;

    public GameObject FriendlyBox;
    public List<Text> FriendlyBoxTexts;

    public GameObject EXPDisplay;

    public List<Text> AttackMoves;
    public List<Text> MagicAttackMoves;
    public List<Text> Items;

    public MoveSelectionUI moveSelector;

    public List<AttackMovesBase> attmoves;
    public List<AttackMovesBase> magicmoves;




    public void SetDialog(string dialog = "", bool showinformation = true)
    {
        if (showinformation)
        {
            InformationText.gameObject.SetActive(true);
            InformationText.text = dialog;
            dialogText.gameObject.SetActive(false);
        }
        else
        {
            InformationText.text = dialog;
            InformationText.gameObject.SetActive(false);
            dialogText.gameObject.SetActive(true);
        }

    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / LettersperSecound);            
        }
        yield return new WaitForSeconds(1f);
    }

    public void EnableInitalBox(bool enable)
    {
        InitialBox.SetActive(enable);
    }

    public void SetMoves(List<Unit_Moves> moves)
    {
       for (int i = 0; i < moves.Count; i++)
       {
           if (moves[i].AMBase.MovCatg == MoveCategory.Physical)
           {
                attmoves.Add(moves[i].AMBase);
           }
           if (moves[i].AMBase.MovCatg == MoveCategory.Magical)
           {
                magicmoves.Add(moves[i].AMBase);
           }
       }

        for (int i = 0; i < AttackMoves.Count; i++)
        {
            if (i < attmoves.Count)
            {
                AttackMoves[i].text = attmoves[i].Name;
            }
        }
        for (int i = 0; i < MagicAttackMoves.Count; i++)
        {
            if (i < magicmoves.Count)
            {
                MagicAttackMoves[i].text = magicmoves[i].Name;
            }
        }
    }

    public void ResetMoves()
    {
        for (int i = 0; i < AttackMoves.Count; i++)
        {
            AttackMoves[i].text = "-";
        }
        for (int i = 0; i < MagicAttackMoves.Count; i++)
        {
            MagicAttackMoves[i].text = "-";
        }

        attmoves.Clear();
        magicmoves.Clear();
    }

    public void UpdateHandleAction(int selected, List<Text> UItexts)
    {
        for (int i = 0; i < UItexts.Count; i++)
        {
            if (i == selected)
            {
                UItexts[i].color = Color.blue;
            }
            else
            {
                UItexts[i].color = Color.white;
            }
        }
    }

   // void Start()
   // {
   //     if (ButtonFace != null)
   //     {
   //         if (Shadow != null)
   //         {
   //             MaxShadowOpacity = Shadow.color.a;
   //             Shadow.color = new Color(Shadow.color.r, Shadow.color.g, Shadow.color.b, 0f);
   //         }
   //
   //         body = ButtonFace.GetComponent<Rigidbody>();
   //         SpringJoint = ButtonFace.GetComponent<SpringJoint>();
   //         InitialLocalPosition = ButtonFace.localPosition;
   //
   //         pointerEvent = new PointerEventData(EventSystem.current);
   //         pointerEvent.button = PointerEventData.InputButton.Left;
   //         RaycastResult result = new RaycastResult();
   //         result.gameObject = gameObject;
   //         pointerEvent.pointerCurrentRaycast = result;
   //         pointerEvent.pointerPress = gameObject;
   //         pointerEvent.rawPointerPress = gameObject;
   //         ButtonFace.localPosition = new Vector3(InitialLocalPosition.x, InitialLocalPosition.y, SpringJoint.connectedAnchor.z);
   //         PhysicsPosition = transform.TransformPoint(new Vector3(InitialLocalPosition.x, InitialLocalPosition.y, SpringJoint.connectedAnchor.z));
   //         body.position = PhysicsPosition;
   //     }
   //     else
   //     {
   //         Debug.LogWarning("Ensure that you have a UI Element allotted in the Layer Transform!");
   //     }
   // }
}
