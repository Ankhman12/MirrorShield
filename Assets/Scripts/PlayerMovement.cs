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
    public float speed;
    public float jumpSpeed;
    PlayerControls controls;
    Rigidbody2D player;
    int dir = 0;
    float distanceToGround;
    void Start()
    {
        distanceToGround = GetComponent<Collider2D>().bounds.extents.y;
        controls = new PlayerControls();
        player = GetComponent<Rigidbody2D>();
        controls.Movement.SideMovement.performed += SideMovement;
        controls.Movement.SideMovement.canceled += ctx => dir = 0;
        controls.Movement.SideMovement.Enable();
        controls.Movement.Jump.performed += Jump;
        controls.Movement.Jump.Enable();
    }
    void SideMovement(CallbackContext ctx)
    {
        dir = (int)ctx.ReadValue <float>();
    }
    void Jump(CallbackContext ctx)
    {
        if (isGrounded())
            player.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
    }
    private void FixedUpdate()
    {
        player.velocity = new Vector2(dir * speed, player.velocity.y);
    }
    bool isGrounded()
    {
        return Physics2D.Raycast(transform.position, -Vector3.up, distanceToGround + .1f);
    }
}
