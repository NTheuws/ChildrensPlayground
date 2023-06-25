using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

public class BackGroundLoop : MonoBehaviour
{
    // Different sprites that can be used for the platforms.
    public Sprite[] platforms;
    // All different platforms that need to be repeated.
    public GameObject[] levels;

    private Camera mainCamera;
    private Vector2 screenBounds;
    public float choke;
    public GameTimer gameTimer;

    public GameObject background;
    public GameObject spawnPoint;

    private float movingSpeed = 2f;
    private bool startMoving = false;

    // Amount of platforms that are to remain empty at the beginning.
    private int clearStartArea = 32;
    void Start()
    {
        // Get camera component.
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

    // Only start moving the screen when the game has been started.
    public void GameStarted()
    {
        // Start the timer.
        gameTimer.StartTimer();

        // Allow the game to move.
        startMoving = true;
    }

    private void Update()
    {
        // Move the neccesary objects only when the game started.
        if (startMoving)
        {
            transform.position += Vector3.right * Time.deltaTime * movingSpeed;
            background.transform.position += Vector3.right * Time.deltaTime * movingSpeed;
            spawnPoint.transform.position += Vector3.right * Time.deltaTime * movingSpeed;
        }
    }

    private void LateUpdate()
    {
        // Make sure there are no gaps in betweeen sprites.
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
        bool spawnSomething = true;
        int platformType = -1;
        float ran = Random.Range(1, 100);

        // If the platform is supposed to remain empty move the random value to said range.
        if (clearStartArea != 0)
        {
            clearStartArea--;
            //Dont spawn anything.
            spawnSomething = false;
            ran = 90;
        }

        // Random chances of being 1 of the 3 options.
        if (ran <= 65)
        {
            obj.GetComponent<SpriteRenderer>().sprite = platforms[0];
            platformType = 0;
        }
        else if(ran > 65 && ran <= 85)
        {
            obj.GetComponent<SpriteRenderer>().sprite = platforms[1];
            platformType = 1;
        }
        else
        {
            obj.GetComponent<SpriteRenderer>().sprite = platforms[2];
            platformType = 2;
        }

        // Check if something spawns on top.
        PlatformBehaviour platform = obj.GetComponent<PlatformBehaviour>();
        if (platform != null)
        {
            // Set platformtype for collisions.
            platform.ChangePlatformType(platformType);

            if (spawnSomething)
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
}
