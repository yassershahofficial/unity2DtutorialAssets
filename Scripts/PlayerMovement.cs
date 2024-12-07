using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private float movementInputDirection;
    public float movementSpeed = 10f;
    public float jumpForce = 5f;
    private bool isFacingRight = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        inputMovement();
    }
    private void FixedUpdate()
    {
        applyMovement();
    }
    private void inputMovement()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");
        if(Input.GetButtonDown("Jump")){
            applyJump();
        }
        spriteMovementDirection();
    }
    private void applyMovement()
    {
        rb.velocity = new Vector2(movementInputDirection*movementSpeed , rb.velocity.y);
    }
    private void applyJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
    private void spriteMovementDirection(){
        if(isFacingRight && movementInputDirection<0)
        {
            Flip();
        }
        else if(!isFacingRight && movementInputDirection>0)
        {
            Flip();
        }
    }
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}
