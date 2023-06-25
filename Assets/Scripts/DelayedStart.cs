using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedStart : MonoBehaviour
{
    // After the tutorial it's neccesary to reset the Kinect.
    // This is needed because the skeleton ID will be reused without it being visible.

    public GameObject jumpDetection;
    public GameObject bodyManager;

    void Start()
    {
        StartCoroutine(WaitForSecondsCoroutine());        
    }

    IEnumerator WaitForSecondsCoroutine()
    {
        yield return new WaitForSeconds(3);
        // Create the skeleton detector.
        Instantiate(jumpDetection);
        Instantiate(bodyManager);
    }
}
