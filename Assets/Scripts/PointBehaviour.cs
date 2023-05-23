using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointBehaviour : MonoBehaviour
{
    private Vector3 startPos;
    private float speed = 3f;
    public bool started = false;
    public bool hitPlayer = false;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (started)
        {
            // Move the obstacle.
            transform.position += Vector3.left * speed * Time.deltaTime;
        }
    }

    // Move object back to original position on contact.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        speed = 0f;
        hitPlayer = true;
        started = false;
        transform.position = startPos;
    }
}
