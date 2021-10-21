using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ConduitManager : MonoBehaviour
{
    public List<GameObject> poweredObjects;
    public List<GameObject> powerSources;
    public List<TilemapRenderer> conduits;

    //public List<Sprite> tlCorner;
    //public List<Sprite> trCorner;
    //public List<Sprite> blCorner;
    //public List<Sprite> brCorner;
    //public List<Sprite> horizontal;
    //public List<Sprite> vertical;
    //public List<Sprite> recieverUp;
    //public List<Sprite> recieverDown;
    //public List<Sprite> recieverRight;
    //public List<Sprite> recieverLeft;


    // Start is called before the first frame update
    void Start()
    {
        if (poweredObjects.Count != powerSources.Count && poweredObjects.Count != conduits.Count)
        {
            Debug.LogError("power source and target object lists must have same the same length, with each shared index representing a group");
        }

        //for (int i = 0; i < powerSources.Count; i++) {
        //    //set up connections
        //    ScriptableObject conduit = ScriptableObject.CreateInstance("Conduit");
        //    Conduit c = (Conduit)conduit;
        //    c.powerSource = powerSources[i];
        //    c.powerTarget = poweredObjects[i];
        //    c.powered = PowerState.Off;
        //    
        //
        //
        //  conduits.Add(c);
        //}

        for (int i = 0; i < conduits.Count; i++)
        {
            conduits[i].enabled = false;
            //Debug.Log("lalalouie");
        }
    }
    // Update is called once per frame
    void Update()
    {
       // Debug.Log("kklol");
        for (int i = 0; i < conduits.Count; i++)
        {
            PressurePlate pp;
            if (powerSources[i].TryGetComponent<PressurePlate>(out pp))
            {
                //Debug.Log("xxxxx");
                if (pp.isPowered() == PowerState.On)
                {
                    conduits[i].enabled = true;
                    MovingPlatform mp;
                    if (poweredObjects[i].TryGetComponent<MovingPlatform>(out mp))
                    {
                        mp.powered = PowerState.On;
                    }
                    BoxDispenser bd;
                    if (poweredObjects[i].TryGetComponent<BoxDispenser>(out bd))
                    {
                        bd.powered = PowerState.On;
                    }
                }
                if (pp.isPowered() == PowerState.Off)
                {
                    conduits[i].enabled = false;
                    MovingPlatform mp;
                    if (poweredObjects[i].TryGetComponent<MovingPlatform>(out mp))
                    {
                        mp.powered = PowerState.Off;
                    }
                    BoxDispenser bd;
                    if (poweredObjects[i].TryGetComponent<BoxDispenser>(out bd))
                    {
                        bd.powered = PowerState.Off;
                    }
                }
            }
            
            LaserReciever lr;
            if (powerSources[i].TryGetComponent<LaserReciever>(out lr))
            {
                //Debug.Log("bitbop");
                if (lr.isPowered() == PowerState.On)
                {
                    conduits[i].enabled = true;
                    MovingPlatform mp;
                    if (poweredObjects[i].TryGetComponent<MovingPlatform>(out mp))
                    {
                        mp.powered = PowerState.On;
                    }
                    BoxDispenser bd;
                    if (poweredObjects[i].TryGetComponent<BoxDispenser>(out bd))
                    {
                        bd.powered = PowerState.On;
                    }
                }
                if (lr.isPowered() == PowerState.Off)
                {
                    conduits[i].enabled = false;
                    MovingPlatform mp;
                    if (poweredObjects[i].TryGetComponent<MovingPlatform>(out mp))
                    {
                        mp.powered = PowerState.Off;
                    }
                    BoxDispenser bd;
                    if (poweredObjects[i].TryGetComponent<BoxDispenser>(out bd))
                    {
                        bd.powered = PowerState.Off;
                    }
                }
            }
        }


    }
}
