using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    private float speed = 3f;
    private Vector3 startPos;
    public bool started = false;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (started)
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
            if (transform.position.x < -10f)
            {
                transform.position = startPos;
            }
        }
    }

    // Move object back to original position on contact.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        randomSpeed();
        transform.position = startPos;
    }
    // Randomize speed.
    private void randomSpeed()
    {
        speed = Random.Range(3f, 5f);
    }
}
