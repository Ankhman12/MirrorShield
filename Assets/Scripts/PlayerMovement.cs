using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour
{
    //Integer values for the player's speed and jumpSpeed
    public float speed;
    public float jumpSpeed;
    //Tells the isGrounded function which layers to check if the player is grounded on.
    //Prevents the player from jumping off colliders without restriction (including the player's own collider)
    public LayerMask groundedLayers;

    PlayerControls controls;
    Rigidbody2D player;
    int dir = 0;
    float distanceToGround;
    
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
        controls.Movement.Jump.Enable();
        #endregion
    }
    void SideMovement(CallbackContext ctx)
    {
        dir = (int)ctx.ReadValue <float>();
    }
    //Causes the player to jump
    void Jump(CallbackContext ctx)
    {
        if (isGrounded())
            player.AddForce(new Vector2(0, jumpSpeed * Time.fixedDeltaTime * 50), ForceMode2D.Impulse); ;
    }
    private void FixedUpdate()
    {
        player.velocity = new Vector2(dir * speed * Time.fixedDeltaTime * 50, player.velocity.y);
    }
    //Checks if the players distance to a collider on the grounded layer is below a certain threshold
    bool isGrounded()
    {
        return Physics2D.Raycast(transform.position, -Vector3.up, distanceToGround + .1f, groundedLayers);
    }
}
