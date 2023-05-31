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

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitForSecondsCoroutine()
    {

        yield return new WaitForSeconds(5);
        // Create the skeleton detector.
        Instantiate(jumpDetection);
        Instantiate(bodyManager);
    }
}
