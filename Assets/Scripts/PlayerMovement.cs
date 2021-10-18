using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour
{
    public Image[] hearts;
    int health = 3;

    //Integer values for the player's speed and jumpSpeed
    public float speed;
    public float jumpSpeed;
    public float jumpTime;
    // Values for the timed duration of the dash in seconds, along with the speed of the dash.
    public float dashDuration;
    public float groundDashSpeed;
    public float airDashSpeed;
    //Tells the isGrounded function which layers to check if the player is grounded on.
    //Prevents the player from jumping off colliders without restriction (including the player's own collider)
    public LayerMask groundedLayers;

    PlayerControls controls;
    Rigidbody2D player;
    int dir = 0;
    int dashdir = 0;
    float distanceToGround;

    bool canJump = false;
    bool isJumping = false;
    bool grounded = false;
    float airTimer = 0f;
    bool isGroundDash = false; // Check whether the player is currently dashing on the ground
    bool isAirDash = false;    //                                                 or aerially
    float dashTimer = 0;   // Used to check how much time is left in dash.

    public Sprite heart;
    public Sprite emptyHeart;

    //Character sprite controllers
    public SpriteRenderer spriteRenderer;
    public Animator anim;
    
    void Start()
    {
        distanceToGround = GetComponent<Collider2D>().bounds.extents.y;
        controls = new PlayerControls();
        player = GetComponent<Rigidbody2D>();

        #region ControlMapping
        controls.Movement.SideMovement.performed += SideMovement;
        controls.Movement.SideMovement.canceled += ctx => dir = 0;
        controls.Movement.SideMovement.Enable();
        controls.Movement.Jump.performed += Jump;
        controls.Movement.Jump.canceled += Jump;
        controls.Movement.Jump.Enable();
        controls.Movement.Dash.performed += Dash;
        controls.Movement.Dash.Enable();
        #endregion
    }
    void SideMovement(CallbackContext ctx)
    {
        dir = (int)ctx.ReadValue <float>();
        if (!isGroundDash && !isAirDash && dir != 0) dashdir = dir; // Get last direction to use in Dash
    }
    //Causes the player to jump
    void Jump(CallbackContext ctx)
    {
        //if (canJump)
        //    player.AddForce(new Vector2(0, jumpSpeed * Time.fixedDeltaTime * 50), ForceMode2D.Impulse);
               
        //Debug.Log(ctx.performed);
        //Debug.Log(ctx.canceled);
        if (ctx.canceled)
        {
            //Debug.Log("boop");
            isJumping = false;
        }
        if (ctx.performed) {
            //Debug.Log("beep");
            isJumping = true;
        }
        
        
    
    }
    // Causes the player to dash left or right. If in air, change to an aerial dash.
    void Dash(CallbackContext ctx) 
    {
        //dashdir = (int)ctx.ReadValue<float>();
        if (grounded) { // Grounded Dash
            isGroundDash = true;
            
        }
        else // Aerial Jetpack Dash
        {
            isAirDash = true;
        }
    }
    private void FixedUpdate()
    {

        grounded = isGrounded();
        anim.SetBool("onGround", grounded);
        anim.SetBool("inAir", !grounded);
        if (!grounded)
        {
            airTimer += Time.fixedDeltaTime;
        }
        else {
            airTimer = 0f;
            canJump = true;
        }
        if (airTimer > jumpTime) {
            canJump = false;
            anim.SetBool("Jumping", false);
        }
        if (canJump && isJumping)
        {
            anim.SetBool("Jumping", true);
            player.AddForce(new Vector2(0, jumpSpeed * Time.fixedDeltaTime * 50), ForceMode2D.Impulse);
        }

        if (dir == 0)
        {
            anim.SetBool("isMoving", false);
        }
        else {
            anim.SetBool("isMoving", true);
        }
        if (dir < 0)
        {
            Flip(true);
        }
        else {
            Flip(false);
        }
        if (!isGroundDash && !isAirDash) // Walking
        {

            player.velocity = new Vector2(dir * speed * Time.fixedDeltaTime * 50, player.velocity.y);
        }
        else if (isGroundDash && !isAirDash) // Ground dash
        {
            isGroundDash = dash(groundDashSpeed);
            anim.SetBool("isDashing", isGroundDash);
        }
        else if (isAirDash) // Aerial dash
        {
            // Use jetpack fuel / fuel check here.
            isAirDash = dash(airDashSpeed);
            anim.SetBool("isDashing", isAirDash);
        }
        if (isAirDash || isGroundDash) {
            anim.SetBool("isDashing", true);
        }
    }
    
    //Checks if the players distance to a collider on the grounded layer is below a certain threshold
    bool isGrounded()
    {
        return Physics2D.Raycast(transform.position, -Vector3.up, distanceToGround + .1f, groundedLayers);
    }

    // Controls dashing. Returns true if dash in progress, returns false if the dash is complete.
    bool dash(float dashSpeed)
    {
        if (dashTimer >= dashDuration)
        {
            dashTimer = 0;
            dashdir = 0;
            return false;
        }
        else
        {
            dashTimer += Time.deltaTime;
            player.velocity = new Vector2(dashdir * dashSpeed * Time.fixedDeltaTime * 50, player.velocity.y);
            return true;
        }
    }

    public void Damage()
    {
        if (health == 0)
            return;
        health--;
        hearts[health].sprite = emptyHeart;
        if (health <= 0)
        {
            Debug.Log("Die");
            SceneManager.LoadScene(0);
        }
    }

    public void AddHealth()
    {
        if (health < hearts.Length-1)
        {
            health++;
            hearts[health].sprite = heart;
        }
    }

    private void Flip(bool flipped) {
        spriteRenderer.flipX = flipped;
    }
}
