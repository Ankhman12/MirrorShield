using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public int health;

    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if (collision.gameObject.CompareTag("Player")) {
            Debug.Log("yeetd");
            bool gainedHealth = collision.gameObject.GetComponent<PlayerMovement>().AddHealth(health);
            if (gainedHealth)
                 Destroy(gameObject);
        }
    }
}
