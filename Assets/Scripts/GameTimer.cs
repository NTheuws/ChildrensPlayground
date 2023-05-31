using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameTimer : MonoBehaviour
{
    private bool timerStarted = false;
    public bool gameOver = false;
    private float totalTime = 300;

    // To display the current time left in seconds.
    public TMP_Text displayTimeLeft;
    private string timeTxt = "Tijd: ";

    public void StartTimer()
    {
        timerStarted = true;
    }

    void Update()
    {
        if (timerStarted)
        {
            if (totalTime > 0)
            {
                totalTime -= Time.deltaTime;
                displayTimeLeft.text = timeTxt + (int)Math.Ceiling(totalTime);
            }
            else
            {
                gameOver = true;
                totalTime = 0;
                displayTimeLeft.text = timeTxt + "0";
                timerStarted = false;
            }
        }
    }
}
