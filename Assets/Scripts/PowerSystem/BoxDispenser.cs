using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDispenser : MonoBehaviour
{
    public GameObject spawnedBox;
    public GameObject boxPrefab;
    public PowerState powered;

    // Start is called before the first frame update
    void Start()
    {
        powered = PowerState.Off;
    }

    // Update is called once per frame
    void Update()
    {
        if (powered == PowerState.On) {
            if (spawnedBox != null)
            {//&& (Mathf.Abs(spawnedBox.transform.position.x - transform.position.x) >= 0.5f || Mathf.Abs(spawnedBox.transform.position.y - transform.position.y) >= 0.5f)) {
                //Debug.Log(powered);
                Destroy(spawnedBox);
                
                //play break particles
            }
            spawnedBox = Instantiate(boxPrefab, transform.position + (new Vector3(0, .1f, 0)), Quaternion.identity);
            powered = PowerState.Off;
        }
    }
}
