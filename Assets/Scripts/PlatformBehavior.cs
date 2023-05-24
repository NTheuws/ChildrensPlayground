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

    // Chances for the spawns on the platforms.
    private const float spawnNothing = 70;
    private const float spawnEnemyUpperChance = 80;

    void Start()
    {
        currentLocation = transform.position;
    }

    // Attempt to spawn an enemy above.
    public void AttemptSpawn(bool below)
    {
        // Redo position.
        currentLocation = transform.position;
        // Slight difference in placement to have a slightly more playful element.
        float addedX = Random.Range(-0.75f,0.75f);

        float ran = Random.Range(1, 100);
        if (ran <= spawnNothing)
        {
            // keep spot empty.
        }
        else if(ran > spawnNothing && ran <= spawnEnemyUpperChance)
        // Spawn enemy.
        {
            GameObject c = Instantiate(enemy) as GameObject;
            PositionObject(ran, c, addedX, below);
        }
        // Spawn tickets.
        else
        {
            GameObject c = Instantiate(ticket) as GameObject;
            PositionObject(ran, c, addedX, below); 
        }
    }

    // Place the object on the upper or lower spot.
    private void PositionObject(float val, GameObject obj, float addedX, bool below)
    {
        float addedY = 0;
        if (below)
        {
            addedY = -1.7f;
        }
        if (val % 2 == 0)
        {
            // Upper half.
            obj.transform.position = new Vector3(currentLocation.x + addedX, currentLocation.y + upperPlacement + addedY, currentLocation.z);
        }
        else
        {
            //Lower half.
            obj.transform.position = new Vector3(currentLocation.x + addedX, currentLocation.y + lowerPlacement + addedY, currentLocation.z);
        }
    }
}
