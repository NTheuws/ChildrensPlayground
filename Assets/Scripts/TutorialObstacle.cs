using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialObstacle : MonoBehaviour
{
    private float speed = 3f;
    public float slowedSpeed = 0.7f;
    private Vector3 startPos;
    public bool started = false;
    public bool hitPlayer = false;
    private float pausePointX = -2.4f;
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
                if (hitPlayer)
                {
                    speed = 0;
                    ObstacleDone = true;
                }
            }
            if (hitPlayer && transform.position.x < pausePointX)
            {
                speed = slowedSpeed;
            }
        }
    }

    // Move object back to original position on contact.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        speed = 3f;
        transform.position = startPos;
        hitPlayer = true;
    }
}
