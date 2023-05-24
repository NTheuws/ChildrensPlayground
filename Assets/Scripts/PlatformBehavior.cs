using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBehavior : MonoBehaviour
{
    //public GameObject topPlacement;
    //public GameObject bottomPlacement;

    private Vector3 currentLocation;
    public GameObject enemy;
    public GameObject ticket;

    private float upperPlacement = 1.2f;
    private float lowerPlacement = 0.3f;
    void Start()
    {
        currentLocation= transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Attempt to spawn an enemy above.
    public void AttemptSpawn()
    {
        // Redo position.
        currentLocation = transform.position;

        float ran = Random.Range(1, 100);
        if (ran <= 70)
        {
            // keep spot empty.
        }
        else if(ran > 70 && ran <= 78)
        // Spawn enemy.
        {
            GameObject c = Instantiate(enemy) as GameObject;
            PositionObject(ran, c);
        }
        // Spawn tickets.
        else
        {
            GameObject c = Instantiate(ticket) as GameObject;
            PositionObject(ran, c); 
        }
    }

    private void PositionObject(float val, GameObject obj)
    {
        if (val % 2 == 0)
        {
            // Upper half.
            obj.transform.position = new Vector3(currentLocation.x, currentLocation.y + upperPlacement, currentLocation.z);
        }
        else
        {
            //Lower half.
            obj.transform.position = new Vector3(currentLocation.x, currentLocation.y + lowerPlacement, currentLocation.z);
        }
    }
}
