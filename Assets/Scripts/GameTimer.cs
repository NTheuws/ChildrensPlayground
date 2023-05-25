using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public bool gameOver = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartTimer()
    {
        StartCoroutine(WaitForSecondsCoroutine(300));
    }
    private IEnumerator WaitForSecondsCoroutine(int val)
    {
        yield return new WaitForSeconds(val);
        // 5min have passed, game's over
        gameOver= true;
    }
}
