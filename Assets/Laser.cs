using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private LineRenderer lr;

    int maxReflectionCount;
    int minReflectionCount = 1;
    public float maxStepDistance = 200f;

    Vector3[] reflectPoints;


    void Start()
    {
        lr = GetComponent<LineRenderer>();
        maxReflectionCount = lr.positionCount;
        reflectPoints = new Vector3[maxReflectionCount];

    }


    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right);
        if (hit.collider != null)
        {
            reflectPoints[0] = this.transform.position;
            Reflect(this.transform.position + this.transform.right * 0.75f, this.transform.right, minReflectionCount, null);
            
            lr.SetPositions(reflectPoints);

        }
        else
        {
            lr.SetPosition(1, new Vector2(2000, 0));
        }

    }


    private void Reflect(Vector2 position, Vector2 direction, int reflectionCount, Collider2D prevCollider)
    {

        if (reflectionCount == maxReflectionCount)
        {
            return;
        }

        Vector2 startingPosition = position;

        RaycastHit2D hit2 = Physics2D.Raycast(position + (direction * .1f), direction, maxStepDistance);
        if (hit2.collider != null)
        {
            if (hit2.collider.Equals(prevCollider)) { 
                
            }
            
            direction = Vector2.Reflect(direction, hit2.normal);
            position = hit2.point;
        }
        else
        {
            position += direction * maxStepDistance;
        }

        reflectPoints[reflectionCount] = new Vector3(position.x, position.y, 0);

        Debug.DrawLine(startingPosition, position, Color.blue);
        

        if (reflectionCount == 2) {
            Debug.Log(hit2.collider);
        }

        Reflect(position, direction, reflectionCount + 1, hit2.collider);


    }
}
