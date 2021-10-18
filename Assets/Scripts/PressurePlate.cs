using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    PowerState power;
    GameObject platePad;
    SpringJoint2D spring;
    float springDist;
    
    // Start is called before the first frame update
    void Start()
    {
        power = PowerState.Off;
        spring = GetComponent<SpringJoint2D>();
        springDist = spring.distance;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
    //Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Plate")) {
            //spring.distance = 0.005f;
            //SpriteRenderer s = platePad.GetComponent<SpriteRenderer>();
            //s.color = Color.green;

            platePad = collision.gameObject;
            power = PowerState.On;
            Debug.Log(power);
        }
    }

    

    private void OnCollisionExit2D(Collision2D collision)
    {
        //Debug.Log(collision.gameObject.name);
        if (platePad != null)
        {
            //SpriteRenderer s = platePad.GetComponent<SpriteRenderer>();
            //s.color = Color.red;
            //spring.distance = springDist;

            power = PowerState.Off;
            Debug.Log(power);
            platePad = null;
        }
    }
}
