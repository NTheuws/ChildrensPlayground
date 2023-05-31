using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialObstacle : MonoBehaviour
{
    private float speed = 3f;
    private Vector3 startPos;
    public bool started = false;
    public bool hitPlayer = false;
    private float pausePointX = 1.6f;
    public bool ObstacleDone = false;

    void Start()
    {
        // Remember the starting position of the gameobject.
        startPos = transform.position;
    }

    void Update()
    {
        if (started)
        {
            // Move the obstacle.
            transform.position += Vector3.left * speed * Time.deltaTime;
            // When OB move it back to the start.
            if (transform.position.x < -14f)
            {
                transform.position = startPos;
                ObstacleDone = true;
                Destroy(gameObject);

                // Get rid of the current skeleton tracker.
                GameObject[] skeletonTracker = GameObject.FindGameObjectsWithTag("SkeletonTracker");
                foreach (GameObject obj in skeletonTracker)
                {
                    Destroy(obj);
                }
                // Start the game.
                SceneManager.LoadScene("MainGame");
            }
        }
    }

    // Move object back to original position on contact.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        transform.position = startPos;
        hitPlayer = true;
    }
}
