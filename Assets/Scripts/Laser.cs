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
    string playerTag = "Player";
    string recieverTag = "LaserReciever";
    string breakableTag = "Breakable";

    //Vector3[] reflectPoints;

    LinkedList<Vector3> reflectPoints;

    //amount of time between heart removals
    float damageWaitTime = 3f;
    //current wait time
    float damageWait = 0f;
    bool damageable = true;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = maxReflectionCount + 1;
        //reflectPoints = new Vector3[maxReflectionCount];
        reflectPoints = new LinkedList<Vector3>();
    }


    void Update()
    {
        shootLaser();
        if (!damageable)
        {
            damageWait += Time.deltaTime;
            if (damageWait >= damageWaitTime)
            {
                damageable = true;
                damageWait = 0;
            }
        }
    }

    private void renderLaser() 
    {

        Vector3[] arr = new Vector3[maxReflectionCount + 1];
        int size = reflectPoints.Count;
        if (size > maxReflectionCount) {
            size = maxReflectionCount;
        }
        for (int i = 0; i < size; i++) 
        {
            //Debug.Log(i);
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
            //Debug.Log(hit.collider.transform.gameObject);
            if (hit.collider.transform.gameObject.CompareTag(mirrorTag))
            {
                Reflect(this.transform.position + this.transform.right * 0.75f, this.transform.right, minReflectionCount);
            }
            else {
                reflectPoints.AddLast(hit.point);
                if (hit.collider.gameObject.CompareTag(playerTag) && damageable)
                {
                    hit.collider.gameObject.GetComponent<PlayerMovement>().Damage();
                    damageable = false;
                }
                else if (hit.collider.transform.gameObject.CompareTag(recieverTag)) 
                {
                    hit.collider.gameObject.GetComponent<LaserReciever>().recieveLaser();
                }
                else if (hit.collider.gameObject.CompareTag(breakableTag))
                {
                    Destroy(hit.collider.gameObject);
                    shootLaser();
                }
            }
            renderLaser();
        }
        else
        {
            //contactFX.Pause();
            lr.SetPosition(0, this.transform.position);
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

        //Debug.DrawLine(startingPosition, position, Color.blue);
        if (hit2.collider != null)
        {
            if (hit2.collider.transform.gameObject.CompareTag(mirrorTag))
            {
                Reflect(position, direction, reflectionCount + 1);
            }
            else if (hit2.collider.gameObject.CompareTag(playerTag) && damageable)
            {
                hit2.collider.gameObject.GetComponent<PlayerMovement>().Damage();
                damageable = false;
            }
            else if (hit2.collider.gameObject.CompareTag(breakableTag))
            {
                Destroy(hit2.collider.gameObject);
                shootLaser();
            }
        }
    }
}
