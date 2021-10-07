using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningLaser : MonoBehaviour
{
    public GameObject topLaser;
    public GameObject bottomLaser;
    public int spinWait;
    public float rotateSpeed;
    Transform spinner;
    bool rotate;
    Vector3 rotateStop = Vector3.zero;
    private void Start()
    {
        spinner = GetComponent<Transform>();
        StartCoroutine(wait());
    }
    private void FixedUpdate()
    {
        if (rotate)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(rotateStop), rotateSpeed * Time.deltaTime);
            if (transform.rotation == Quaternion.Euler(rotateStop))
            {
                topLaser.SetActive(true);
                bottomLaser.SetActive(true);
                rotate = false;
                StartCoroutine(wait());
            }
        }
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(spinWait);
        rotateStop = spinner.eulerAngles + new Vector3(0, 0, -90);
        topLaser.SetActive(false);
        bottomLaser.SetActive(false);
        rotate = true;
    }
}
