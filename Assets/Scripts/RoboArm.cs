using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboArm : MonoBehaviour
{
    public Transform[] patrolPoints;
    public GameObject baseJoint;
    public GameObject armJoint;
    public GameObject cannon;
    public float pauseTime;
    public float moveSpeed;
    public float armMoveSpeed;
    public LayerMask player;
    float pauseCounter = 0;
    bool playerSpotted = false;
    bool waiting = false;
    Vector2 nextPoint;
    int patrolPoint;
    // Start is called before the first frame update
    void Start()
    {
        cannon.SetActive(false);
        nextPoint = patrolPoints[0].position;
        patrolPoint = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerSpotted && !waiting)
        {
            float moveDist = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, nextPoint, moveDist);
            if (Vector2.Distance(transform.position, nextPoint) <= .01f)
            {
                if (patrolPoint == 0)
                {
                    patrolPoint++;
                    nextPoint = patrolPoints[patrolPoint].position;
                }
                else
                {
                    patrolPoint = 0;
                    nextPoint = patrolPoints[patrolPoint].position;
                }
                waiting = true;
            }
        } 
        else if (waiting)
        {
            pauseCounter += Time.deltaTime;
            if (pauseCounter >= pauseTime)
            {
                waiting = false;
                pauseCounter = 0;
            }
        }
    }
    void Attack(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & player) == 0)
            return;
        if (collision.CompareTag("Mirror"))
            return;
        GameObject playerObj = collision.gameObject;
        if (playerObj.transform.position.y >= transform.position.y - .5)
        {
            if (cannon.gameObject.activeInHierarchy)
                DisableCannon();
            return;
        }
        Vector2 fireDir = (playerObj.transform.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, fireDir);
        if (hit.collider == null || ((1 << hit.collider.gameObject.layer) & player) == 0)
        {
            if (cannon.gameObject.activeInHierarchy)
                DisableCannon();
            return;
        }
        else
        {
            playerSpotted = true;
            baseJoint.transform.up = -(playerObj.transform.position - baseJoint.transform.position);
            float zRot = baseJoint.transform.eulerAngles.z;
            zRot = zRot % 360;
            baseJoint.transform.eulerAngles = new Vector3(0, 0, 360 - zRot);
            armJoint.transform.up = armJoint.transform.position - playerObj.transform.position;
            cannon.SetActive(true);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Attack(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Attack(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        DisableCannon();
    }
    void DisableCannon()
    {
        cannon.SetActive(false);
        playerSpotted = false;
    }
}
