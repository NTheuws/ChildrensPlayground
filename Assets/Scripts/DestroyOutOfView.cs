using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOutOfView : MonoBehaviour
{
    private Camera mainCamera;
    public Vector2 widthThresold;
    public Vector2 heightThresold;

    void Start()
    {
        mainCamera = GameObject.Find("Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 screenPosition = mainCamera.WorldToScreenPoint(transform.position);
        if (screenPosition.x < widthThresold.x)
        { 
            Destroy(gameObject); 
        }
    }
}
