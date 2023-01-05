using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    //PARAMETERS
    public float Move_x { get; set; }
    public float Move_y { get; set; }
    public bool isMoving { get; set; }

    //FRAMES
    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUPSprites;
    [SerializeField] List<Sprite> walkLeftSprites;
    [SerializeField] List<Sprite> walkRightSprites;

    //STATES
    private SpriteAnimator WalkDownAnim;
    private SpriteAnimator WalkUpAnim;
    private SpriteAnimator WalkLeftAnim;
    private SpriteAnimator WalkRightAnim;

    //STATE CHANGES VARIABLES
    private SpriteAnimator currentAnim;
    private bool wasPrevMoving;

    //REFERENCES
     private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        //Set animations
        WalkDownAnim = new SpriteAnimator(walkDownSprites,spriteRenderer);
        WalkUpAnim = new SpriteAnimator(walkUPSprites, spriteRenderer);
        WalkLeftAnim = new SpriteAnimator(walkLeftSprites, spriteRenderer);
        WalkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer);

        //Set Default animation

        currentAnim = WalkDownAnim;
    }    
    private void Update()
    {
        //Parameter to check if animation changed
        var prevAnim = currentAnim;

        //Change animations acording to parameters
        if (Move_x == -1)
        {             
            currentAnim = WalkLeftAnim;
        }
        else if (Move_x == 1)
        {
            currentAnim = WalkRightAnim;
        }
        else if (Move_y == 1)
        {
            currentAnim = WalkUpAnim;
        }
        else if(Move_y == -1)
        {
            currentAnim = WalkDownAnim;
        }

        //Check if the animation changed
        if (currentAnim != prevAnim || isMoving != wasPrevMoving)
        {
            currentAnim.Start();
        }

        //Check if NPC is Moving
        if (isMoving)
        {
            currentAnim.HandleUpdate();
        }
        else
        {
            spriteRenderer.sprite = currentAnim.Frames[0];
        }
        //Parameter to check if NPC was moving
        wasPrevMoving = isMoving;
    }
}
