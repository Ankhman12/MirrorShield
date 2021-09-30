using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Mirror : MonoBehaviour
{
    public float rotationRadius;
    public float maxAngle;
    public float minAngle;
    Vector2 unitVector;
    public Transform parentPos;
    bool updateRotation;
    bool rotateMirrorLocally = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        

        CalculatePosition();
        if (updateRotation)
        {
            transform.localPosition = rotationRadius * unitVector;
            transform.up = parentPos.position - transform.position;
            //transform.eulerAngles = transform.eulerAngles + new Vector3(0, 0, 0);
        }
    }
    void CalculatePosition()
    {
        updateRotation = false;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 worldPos2 = new Vector2(worldPos.x, worldPos.y);

        Vector2 parentPos2;
        if (rotateMirrorLocally) {
            parentPos2 = new Vector2(parentPos.position.x, parentPos.position.y);
        }
        else {
            parentPos2 = new Vector2(transform.position.x, transform.position.y);
        }
        if (Vector2.Angle(parentPos2, worldPos2) > minAngle)
        {
            unitVector = (worldPos2 - parentPos2).normalized;
            updateRotation = true;
        }
    }
}
