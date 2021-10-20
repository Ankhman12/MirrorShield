using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Tooltip("The Nodes making up the path")]
    public List<Transform> platformPath = new List<Transform>();

    [Tooltip("The distance at which the enemy considers that it has reached its current path destination point")]
    public float PathReachingRadius = 2f;

    int m_platformIndex;
    public float platformSpeed;
    public PowerState powered;

    [Tooltip("Time platform waits at each end of its track")]
    public float platformWaitTime;
    float waitTimer;

    Vector2 dir = Vector2.zero;


    GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        m_platformIndex = 0;
        SetPathDestinationToClosestNode();
    }

    // Update is called once per frame
    void Update()
    {
        if (powered == PowerState.On)
        {
            transform.position += new Vector3(dir.x * platformSpeed * Time.deltaTime, dir.y * platformSpeed * Time.deltaTime, 0);
        }

        UpdatePathDestination();
        Vector3 currentDest = GetDestinationOnPath();
        dir = (currentDest - transform.position).normalized;
    }

    bool IsPathValid()
    {
        return (this != null) && platformPath.Count > 0;
    }

    public void ResetPathDestination()
    {
        m_platformIndex = 0;
    }

    public void SetPathDestinationToClosestNode()
    {
        if (IsPathValid())
        {
            int closestPathNodeIndex = 0;
            for (int i = 0; i < platformPath.Count; i++)
            {
                float distanceToPathNode = GetDistanceToNode(transform.position, i);
                if (distanceToPathNode < GetDistanceToNode(transform.position, closestPathNodeIndex))
                {
                    closestPathNodeIndex = i;
                }
            }

            m_platformIndex = closestPathNodeIndex;
        }
        else
        {
            m_platformIndex = 0;
        }
    }

    public Vector3 GetDestinationOnPath()
    {
        if (IsPathValid())
        {
            return GetPositionOfPathNode(m_platformIndex);
        }
        else
        {
            return transform.position;
        }
    }

    public void UpdatePathDestination()
    {
        if (IsPathValid())
        {
            // Check if reached the path destination
            if (new Vector3(transform.position.x - GetDestinationOnPath().x, transform.position.y - GetDestinationOnPath().y, 0).magnitude <= PathReachingRadius)
            {
                //Debug.Log("yeet");
                //Allows guard to wait at each patrol point for a predefined amount of time
                if (waitTimer < platformWaitTime)
                {
                    waitTimer += Time.deltaTime;
                }
                else
                {
                    //reset wait timer
                    waitTimer = 0f;

                    // increment path destination index
                    m_platformIndex = m_platformIndex == 1 ? 0 : 1;
                    if (m_platformIndex < 0)
                    {
                        m_platformIndex += platformPath.Count;
                    }

                    if (m_platformIndex >= platformPath.Count)
                    {
                        m_platformIndex -= platformPath.Count;
                        //ResetPathDestination();
                    }

                }
            }
        }
    }

    public float GetDistanceToNode(Vector3 origin, int destinationNodeIndex)
    {
        if (destinationNodeIndex < 0 || destinationNodeIndex >= platformPath.Count ||
            platformPath[destinationNodeIndex] == null)
        {
            return -1f;
        }

        return (platformPath[destinationNodeIndex].position - origin).magnitude;
    }

    public Vector3 GetPositionOfPathNode(int nodeIndex)
    {
        if (nodeIndex < 0 || nodeIndex >= platformPath.Count || platformPath[nodeIndex] == null)
        {
            return Vector3.zero;
        }

        return platformPath[nodeIndex].position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        for (int i = 0; i < platformPath.Count; i++)
        {
            int nextIndex = i + 1;
            if (nextIndex >= platformPath.Count)
            {
                nextIndex -= platformPath.Count;
            }

            Gizmos.DrawLine(platformPath[i].position, platformPath[nextIndex].position);
            Gizmos.DrawSphere(platformPath[i].position, 0.1f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player = collision.gameObject;
            Player.transform.SetParent(this.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (Player != null)//collision.gameObject.CompareTag("Player"))
        {
            Player.transform.SetParent(null);
            Player = null;
        }
    }
}
