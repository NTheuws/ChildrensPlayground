using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    private bool timerStarted = false;
    public bool gameOver = false;
    private float totalTime = 30;

    // To display the current time left in seconds.
    public TMP_Text displayTimeLeft;
    private string timeTxt = "Tijd: ";

    public void StartTimer()
    {
        timerStarted = true;
    }

    void Update()
    {
        // Only when the game is going.
        if (timerStarted)
        {
            // Time hasn't ran out, keep ticking down.
            if (totalTime > 0)
            {
                totalTime -= Time.deltaTime;
                displayTimeLeft.text = timeTxt + (int)Math.Ceiling(totalTime);
            }
            // When the time ran out go to the scoring scene.
            else
            {
                gameOver = true;
                totalTime = 0;
                displayTimeLeft.text = timeTxt + "0";
                timerStarted = false;

                SceneManager.LoadScene("FinalScores");
            }
        }
    }
}
