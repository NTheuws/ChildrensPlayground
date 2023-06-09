using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlatformBehaviour : MonoBehaviour
{
    private int spawnCounter = 0;

    private Vector3 currentLocation;
    public GameObject enemy;
    public GameObject ticket;

    private float upperPlacement = 1.2f;
    private float lowerPlacement = 0.3f;

    // Chances for the spawns on the platforms.
    private const float spawnNothing = 70;
    private const float spawnEnemyUpperChance = 80;

    // Keep track of what kind of platform is being used.
    // 0. full, 1. half, 2 empty.
    private int platformType = 0;

    void Start()
    {
        currentLocation = transform.position;
        PlayerBehaviour.disableHalfBlock += HalfBlockCollisionJump;
    }

    private void HalfBlockCollisionJump(Collider2D playerCollider)
    {
        switch (platformType)
        {
            // Full platform.
            case 0:
                GetComponent<Collider2D>().enabled = true;
                break;
            // Wooden platform.
            case 1:
                Physics2D.IgnoreCollision(playerCollider, this.GetComponent<Collider2D>());
                if (playerCollider != null && GetComponent<Collider2D>() != null)
                {
                    StartCoroutine(WaitABit(playerCollider));
                }
                break;
            // Empty space.
            case 2:
                GetComponent<Collider2D>().enabled = false;
                break;
        }
    }


    // Attempt to spawn an enemy above.
    public void AttemptSpawn(bool below)
    {
        // Redo position.
        currentLocation = transform.position;
        // Slight difference in placement to have a slightly more playful element.
        float addedX = Random.Range(-0.75f,0.75f);

        if (spawnCounter > 0)
        {
            float ran = Random.Range(1, 100);
            if (ran <= spawnNothing)
            {
                // keep spot empty.
            }
            else if (ran > spawnNothing && ran <= spawnEnemyUpperChance)
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
        // Make sure the 1st floor doesnt count double.
        if (!below)
        {
            spawnCounter++;
        }
    }

    // Place the object on the upper or lower spot.
    private void PositionObject(float val, GameObject obj, float addedX, bool below)
    {
        float addedY = 0;
        if (below)
        {
            // Place below the 1st platform so the floor also gets populated.
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

    public void ChangePlatformType(int type)
    {
        if (type >= 0 && type < 3)
        {
            platformType = type;

            // 0: full block, all colliders
            // 1: half, no colliders only when crouching or jumping/ only collide when walking on top.
            // 2: empty, no colliders
            switch(type)
            {
                case 0:
                    GetComponent<Collider2D>().enabled = true;
                    break;
                case 1:
                    // collision is only on when the character is falling downwards, then will be turned off when crouching.
                    GetComponent<Collider2D>().enabled = true;
                    break;
                case 2:
                    GetComponent<Collider2D>().enabled = false;
                    break;
            }
        }
    }

    IEnumerator WaitABit(Collider2D playerCollider)
    {
        // Toggle collider between half platform and player off for 0.6s.
        if (platformType == 1)
        {
            yield return new WaitForSeconds(0.6f);
            if (playerCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, GetComponent<Collider2D>(), false);
            }
        }
    }
    private void OnDestroy()
    {
        PlayerBehaviour.disableHalfBlock -= HalfBlockCollisionJump;
    }
}
