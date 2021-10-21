using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingDrone : MonoBehaviour
{
    public enum MovementType { LeftRight, UpDown, FollowPath};
    public MovementType movementType;
    public PatrolPath patrol { get; set; }
    public float PathReachingRadius = 2f;
    int m_PathDestinationNodeIndex;
    bool patrolOrder = true;
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

    List<GameObject> bodies = new List<GameObject>();

    [Header("Patrol Settings")]
    [Tooltip("Patrol Time")]
    [SerializeField] float patrolWaitTime;
    float waitTimer;

    // Start is called before the first frame update
    void Start()
    {
        cannon = GetComponentsInChildren(typeof(Transform))[1] as Transform;
        cannon.gameObject.SetActive(false);
        if (movementType == MovementType.LeftRight) {
            dir = new Vector2(1, 0);
        }
        else if (movementType == MovementType.UpDown) {
            dir = new Vector2(0, 1);
        }
        else if (movementType == MovementType.FollowPath) {
            SetPathDestinationToClosestNode();
        }
            
    }

    void Update()
    {
        if (!playerSpotted && !paused)
        {
            transform.position += new Vector3(dir.x * flySpeed * Time.deltaTime, dir.y * flySpeed * Time.deltaTime, 0);
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

        if (movementType == MovementType.FollowPath) {
            UpdatePathDestination(patrolOrder);
            Vector3 currentDest = GetDestinationOnPath();
            dir = (currentDest - transform.position).normalized; 
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
        Vector2 fireDir = (playerObj.transform.position-cannon.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(cannon.position, fireDir);
        if (hit.collider == null || ((1 << hit.collider.gameObject.layer) & player) == 0)
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
        if (movementType != MovementType.FollowPath)
        {
            dir *= -1;
        }
        else if (!collision.gameObject.CompareTag("Player")) {
            patrolOrder = !patrolOrder;
            UpdatePathDestination(patrolOrder);
        }
        //paused = true;
        Rigidbody2D rb;
        if (collision.gameObject.TryGetComponent<Rigidbody2D>(out rb))
        {
            bodies.Add(collision.gameObject);
            rb.transform.SetParent(this.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null && bodies.Contains(rb.gameObject))//collision.gameObject.CompareTag("Player"))
        {
            rb.transform.SetParent(null);
            bodies.Remove(rb.gameObject);
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
        cannon.gameObject.SetActive(false);
        playerSpotted = false;
        paused = false;
    }

    bool IsPathValid()
    {
        return patrol && patrol.PathNodes.Count > 0;
    }

    public void ResetPathDestination()
    {
        m_PathDestinationNodeIndex = 0;
    }

    public void SetPathDestinationToClosestNode()
    {
        if (IsPathValid())
        {
            int closestPathNodeIndex = 0;
            for (int i = 0; i < patrol.PathNodes.Count; i++)
            {
                float distanceToPathNode = patrol.GetDistanceToNode(transform.position, i);
                if (distanceToPathNode < patrol.GetDistanceToNode(transform.position, closestPathNodeIndex))
                {
                    closestPathNodeIndex = i;
                }
            }

            m_PathDestinationNodeIndex = closestPathNodeIndex;
        }
        else
        {
            m_PathDestinationNodeIndex = 0;
        }
    }

    public Vector3 GetDestinationOnPath()
    {
        if (IsPathValid())
        {
            return patrol.GetPositionOfPathNode(m_PathDestinationNodeIndex);
        }
        else
        {
            return transform.position;
        }
    }

    public void UpdatePathDestination(bool inverseOrder = false)
    {
        if (IsPathValid())
        {
            // Check if reached the path destination
            if (new Vector3(transform.position.x - GetDestinationOnPath().x, transform.position.y - GetDestinationOnPath().y, 0).magnitude <= PathReachingRadius)
            {
                //Allows guard to wait at each patrol point for a predefined amount of time
                if (waitTimer < patrolWaitTime)
                {
                    waitTimer += Time.deltaTime;
                }
                else
                {
                    //reset wait timer
                    waitTimer = 0f;

                    // increment path destination index
                    m_PathDestinationNodeIndex =
                        inverseOrder ? (m_PathDestinationNodeIndex - 1) : (m_PathDestinationNodeIndex + 1);
                    if (m_PathDestinationNodeIndex < 0)
                    {
                        m_PathDestinationNodeIndex += patrol.PathNodes.Count;
                    }

                    if (m_PathDestinationNodeIndex >= patrol.PathNodes.Count)
                    {
                        m_PathDestinationNodeIndex -= patrol.PathNodes.Count;
                        //ResetPathDestination();
                    }

                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Path reaching range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, PathReachingRadius);

        //if (DetectionModule != null)
        //{
        //    // Detection range
        //    Gizmos.color = DetectionRangeColor;
        //    Gizmos.DrawWireSphere(transform.position, DetectionModule.DetectionRange);

        //    // Attack range
        //    Gizmos.color = AttackRangeColor;
        //    Gizmos.DrawWireSphere(transform.position, DetectionModule.AttackRange);
        //}
    }
}
