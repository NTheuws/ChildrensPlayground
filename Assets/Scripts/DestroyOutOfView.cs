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

    // Remove the objects which go out of the camer's view on the left side to prevent the game from taking more and more resources.
    void Update()
    {
        Vector2 screenPosition = mainCamera.WorldToScreenPoint(transform.position);
        if (screenPosition.x < widthThresold.x)
        { 
            Destroy(gameObject); 
        }
    }
}
