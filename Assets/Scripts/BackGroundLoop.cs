using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

public class BackGroundLoop : MonoBehaviour
{
    public Sprite[] platforms;
    public GameObject[] levels;
    private Camera mainCamera;
    private Vector2 screenBounds;
    public float choke;

    private float movingSpeed = 2.2F;


    void Start()
    {
        mainCamera = gameObject.GetComponent<Camera>();
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));

        foreach(GameObject obj in levels)
        {
            LoadObjects(obj);
        }
    }

    private void LoadObjects(GameObject obj)
    {
        float objectWidth = obj.GetComponent<SpriteRenderer>().bounds.size.x - choke;
        int childsNeeded = (int)Mathf.Ceil(screenBounds.x * 2 / objectWidth);
        GameObject clone = Instantiate(obj) as GameObject;

        for (int i = 0; i <= childsNeeded; i++)
        {
            GameObject c = Instantiate(clone) as GameObject;
            c.transform.SetParent(obj.transform);
            c.transform.position = new Vector3(objectWidth * i, obj.transform.position.y, obj.transform.position.z);  
            c.name = obj.name + i;

            if (c.tag != "Ground")
            {
                RandomizePlatform(c);
            }
        }

        Destroy(clone);
        Destroy(obj.GetComponent<SpriteRenderer>());
    }

    private void Update()
    {
        transform.position += Vector3.right * Time.deltaTime * movingSpeed;
    }

    private void LateUpdate()
    {
        foreach (GameObject obj in levels)
        {
            RepositionChildren(obj);
        }
    }

    private void RepositionChildren(GameObject obj)
    {
        Transform[] children = obj.GetComponentsInChildren<Transform>();
        if (children.Length > 1)
        {
            GameObject firstChild = children[1].gameObject;
            GameObject lastChild = children[children.Length - 1].gameObject;
            float halfObjectWidth = lastChild.GetComponent<SpriteRenderer>().bounds.extents.x - choke;
            if (transform.position.x + screenBounds.x > lastChild.transform.position.x + halfObjectWidth)
            {
                firstChild.transform.SetAsLastSibling();
                firstChild.transform.position = new Vector3(lastChild.transform.position.x + halfObjectWidth * 2, lastChild.transform.position.y, lastChild.transform.position.z);

                if (firstChild.tag != "Ground")
                {
                    RandomizePlatform(firstChild);
                }
            }
        }
    }

    private void RandomizePlatform(GameObject obj)
    {
        /* platforms[] is a collection of the possible types of platforms
         * 0: Normal path
         * 1: wooden platform
         * 2: gap
         */

        float ran = Random.Range(1, 100);
        if (ran <= 65)
        {
            obj.GetComponent<SpriteRenderer>().sprite = platforms[0];
        }
        else if(ran > 65 && ran <= 85)
        {
            obj.GetComponent<SpriteRenderer>().sprite = platforms[1];
        }
        else
        {
            obj.GetComponent<SpriteRenderer>().sprite = platforms[2];
        }

        // Check if something spawns on top.
        PlatformBehavior platform = obj.GetComponent<PlatformBehavior>();
        if (platform != null)
        {
            platform.AttemptSpawn(false);

            // If its the first floor itll also generate for the bottom floor.
            if (obj.tag == "Platform1")
            {
                platform.AttemptSpawn(true);
            }
        }
    }
}
