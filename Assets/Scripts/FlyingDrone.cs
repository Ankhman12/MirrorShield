using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingDrone : MonoBehaviour
{
    public enum MovementType { LeftRight, UpDown, }
    public MovementType movementType;
    public float maxCoord;
    public float minCoord;
    public LayerMask player;
    public float flySpeed;
    public float pauseWait;
    Transform cannon;
    Vector2 dir = Vector2.zero;
    bool playerSpotted;
    bool paused;
    float pauseTime;
    // Start is called before the first frame update
    void Start()
    {
        cannon = GetComponentsInChildren(typeof(Transform))[1] as Transform;
        cannon.gameObject.SetActive(false);
        if (movementType.Equals(MovementType.LeftRight))
            dir = new Vector2(1, 0);
        else if (movementType.Equals(MovementType.UpDown))
            dir = new Vector2(0, 1);
    }

    void Update()
    {
        if (!playerSpotted && !paused)
        {
            transform.position += new Vector3(dir.x * flySpeed * Time.deltaTime, dir.y * flySpeed * Time.deltaTime,0);
        }
        else if (paused)
        {
            pauseTime += Time.deltaTime;
            if (pauseTime >= pauseWait)
            {
                paused = false;
                pauseTime = 0;
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
        Vector2 fireDir = (cannon.position - playerObj.transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(cannon.position, fireDir);
        if (hit.collider == null || ((1 << hit.collider.gameObject.layer) & player) != 0)
        {
            if (cannon.gameObject.activeInHierarchy)
                DisableCannon();
            return;
        }
        else
        {
            playerSpotted = true;
            float angle = -Vector2.Angle(cannon.position, playerObj.transform.position);
            cannon.right = playerObj.transform.position - cannon.position;
            cannon.gameObject.SetActive(true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        dir *= -1;
        paused = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Attack(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Attack(collision);
    }

    void DisableCannon()
    {
        cannon.gameObject.SetActive(false);
        playerSpotted = false;
        paused = false;
    }
}
