using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Mirror : MonoBehaviour
{
    public float rotationRadius;
    Vector2 unitVector;
    //Position around which the mirror rotates
    public Transform parentPos;
    bool rotateMirrorLocally = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        CalculatePosition();
        transform.localPosition = rotationRadius * unitVector;
        transform.up = parentPos.position - transform.position;
    }
    void CalculatePosition()
    {
        //Finds the world position of the mouse cursor
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        //Translates the cursor's 3D position into 2D space
        Vector2 mousePos = new Vector2(worldPos.x, worldPos.y);

        Vector2 parentPos2;
        if (rotateMirrorLocally) {
            parentPos2 = new Vector2(parentPos.position.x, parentPos.position.y);
        }
        else {
            parentPos2 = new Vector2(parentPos.position.x, parentPos.position.y);
        }
        unitVector = (mousePos - parentPos2).normalized;
    }
}
