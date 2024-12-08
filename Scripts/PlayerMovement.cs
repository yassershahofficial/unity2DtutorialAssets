using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private RigidbodyConstraints2D defaultConstraints;

    private float movementInputDirection;
    public float movementSpeed = 10f;
    public float jumpForce = 5f;
    public float groundRadius;
    public float wallRadius;
    public float WallSlideSpeed;
    public float dragWallSlideMultiplier;
    public float currentDragWallSlideMultiplier;
    public float jumpHeightVariable;

    public int maxAmoutOfJump = 1; 
    public int amoutOfJump = 0;

    private bool isFacingRight = true;
    private bool isWalking = false;
    public bool isGrounded;
    public bool isOnWall;
    public bool isWallSlide;

    public Transform groundChecker;
    public LayerMask whatIsGround;
    public Transform wallChecker;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        amoutOfJump = maxAmoutOfJump;
        currentDragWallSlideMultiplier = dragWallSlideMultiplier;
        defaultConstraints = rb.constraints;
    }

    // Update is called once per frame
    private void Update()
    {
        inputMovement();
        spriteMovementDirection();
        changeAnimation();
    }
    private void FixedUpdate()
    {
        applyMovement();
        checkSurrounding();
    }

    //everything inside Update()
    private void inputMovement()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");
        
        //Jump
        if(Input.GetButtonDown("Jump") && (amoutOfJump>0 || (!isGrounded && isOnWall)))
        {
            applyJump();
        }

        if(Input.GetButtonUp("Jump"))
        {
            applyJump(jumpHeightVariable);     
        }

        if(isGrounded)
        {
            amoutOfJump = maxAmoutOfJump;
        }

        //Wall Slide
        isWallSlide = isOnWall && !isGrounded && rb.velocity.y < 0.001f;
        if(isWallSlide)
        {
            applyWallSliding();
            amoutOfJump = maxAmoutOfJump;
        }
        else{
            currentDragWallSlideMultiplier = dragWallSlideMultiplier;
            rb.constraints = defaultConstraints;
        }

    }
    private void applyWallSliding()
    {
        if(currentDragWallSlideMultiplier > 0){
            rb.velocity = new Vector2(rb.velocity.x, -WallSlideSpeed*dragWallSlideMultiplier);
            currentDragWallSlideMultiplier-=0.002f;
        }
        else{
            rb.constraints = RigidbodyConstraints2D.FreezePositionY;
        }
    }
    private void applyJump(float jumpHeightVariable=1)
    {
        if(jumpHeightVariable == 1){
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else{
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y*jumpHeightVariable);
        }
        amoutOfJump--;
        
    }
    private void spriteMovementDirection(){
        //Input jumping opposite direction, flip
        if((isFacingRight && movementInputDirection<0)||(!isFacingRight && movementInputDirection>0))
        {
            Flip();
        }
        //velocity x determine idle or walking
        isWalking = Math.Abs(rb.velocity.x) > 0.001f;
    }
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }
    private void changeAnimation()
    {
        anim.SetBool("isWallSlide",isWallSlide);
        anim.SetBool("isWalking",isWalking);
        anim.SetBool("isGrounded",isGrounded);
        anim.SetFloat("Yvelocity",rb.velocity.y);
        
    }

    //Everything inside FixedUpdate()
    private void applyMovement()
    {
        rb.velocity = new Vector2(movementInputDirection*movementSpeed , rb.velocity.y);
    }
    private void checkSurrounding()
    {
        isGrounded = Physics2D.OverlapCircle(groundChecker.position, groundRadius, whatIsGround);
        isOnWall = Physics2D.Raycast(wallChecker.position, transform.right, wallRadius, whatIsGround);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundChecker.position, groundRadius);
        Gizmos.DrawLine(wallChecker.position, new Vector3(wallChecker.position.x+wallRadius, wallChecker.position.y,wallChecker.position.z));
    }
}
