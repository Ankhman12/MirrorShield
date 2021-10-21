using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserReciever : MonoBehaviour
{
    PowerState powered;
    bool recievingLaser;

    public float cooldownTime;
    public float cooldownTimer;

    public SpriteRenderer spr;
    public Sprite onSprite;
    public Sprite offSprite;

    void Start()
    {
        powered = PowerState.Off;
        recievingLaser = false;
        cooldownTimer = 0f;
        //spr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        //Debug.Log(powered);
        if (recievingLaser) {
            powered = PowerState.On;
            //spr.color = Color.green;
            spr.sprite = onSprite;
        }
        else {
            powered = PowerState.Off;
            //spr.color = Color.red;
            spr.sprite = offSprite;
        }
        cooldownTimer += Time.deltaTime;
        if (cooldownTimer >= cooldownTime) {
            //Debug.Log("holadea");
            recievingLaser = false;
        }
    }
    public void recieveLaser() {
        recievingLaser = true;
        cooldownTimer = 0f;
    }
    public PowerState isPowered()
    {
        return this.powered;
    }


}
