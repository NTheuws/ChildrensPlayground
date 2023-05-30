using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameTimer : MonoBehaviour
{
    public bool gameOver = false;

    public void StartTimer()
    {
        StartCoroutine(WaitForSecondsCoroutine(300));
    }

    private IEnumerator WaitForSecondsCoroutine(int val)
    {
        yield return new WaitForSeconds(val);
        // 5min have passed, game's over
        gameOver = true;
    }
}
