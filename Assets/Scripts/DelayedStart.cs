using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedStart : MonoBehaviour
{
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
