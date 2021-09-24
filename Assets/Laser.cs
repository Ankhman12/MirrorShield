using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private LineRenderer lr;
    public GameObject contactFX;

    public int maxReflectionCount;
    int minReflectionCount = 0;
    public float maxStepDistance = 200f;

    string mirrorTag = "Mirror";

    //Vector3[] reflectPoints;

    LinkedList<Vector3> reflectPoints;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = maxReflectionCount + 1;
        //reflectPoints = new Vector3[maxReflectionCount];
        reflectPoints = new LinkedList<Vector3>();
        Instantiate(contactFX);
        enableLaser();
    }


    void Update()
    {
        shootLaser();
    }

    private void renderLaser() 
    {
        contactFX.transform.position = reflectPoints.Last.Value;
        Debug.Log(contactFX.transform.position);

        Vector3[] arr = new Vector3[maxReflectionCount + 1];
        int size = reflectPoints.Count;
        for (int i = 0; i < size; i++) 
        {
            arr[i] = reflectPoints.First.Value;
            reflectPoints.RemoveFirst();
        }
        if (size < maxReflectionCount) {
            for (int j = size; j < arr.Length; j++) 
            {
                arr[j] = arr[size - 1];
            }
        }
        lr.SetPositions(arr);
    }

    public void shootLaser() 
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right);
        if (hit.collider != null)
        {
            reflectPoints.AddFirst(this.transform.position);
            if (hit.collider.gameObject.CompareTag(mirrorTag))
            {
                Reflect(this.transform.position + this.transform.right * 0.75f, this.transform.right, minReflectionCount);
            }
            else {
                reflectPoints.AddLast(hit.point);
            }
            //lr.SetPositions(reflectPoints);
            renderLaser();
            //Debug.Log("boop");
        }
        else
        {
            //contactFX.Pause();
            lr.SetPosition(1, new Vector2(2000, 0));
        }
    }


    private void Reflect(Vector2 position, Vector2 direction, int reflectionCount)
    {

        if (reflectionCount == maxReflectionCount)
        {
            return;
        }

        Vector2 startingPosition = position;

        RaycastHit2D hit2 = Physics2D.Raycast(position + (direction * .1f), direction, maxStepDistance);
        if (hit2.collider != null)
        {   
            direction = Vector2.Reflect(direction, hit2.normal);
            position = hit2.point;
        }
        else
        {
            position += direction * maxStepDistance;
        }

        reflectPoints.AddLast(new Vector3(position.x, position.y, 0));

        Debug.DrawLine(startingPosition, position, Color.blue);
        

        if (reflectionCount == 2) 
        {
            Debug.Log(hit2.collider);
        }

        if (hit2.collider.gameObject.CompareTag(mirrorTag))
        {
            Reflect(position, direction, reflectionCount + 1);
        }

    }

    void enableLaser()
    {
        lr.enabled = true;
        contactFX.GetComponentInChildren<ParticleSystem>().Play();
    }

    void disableLaser()
    {
        lr.enabled = false;
        contactFX.GetComponentInChildren<ParticleSystem>().Stop();
    }
}
