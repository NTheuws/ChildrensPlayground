using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreTracker : MonoBehaviour
{
    public TMP_Text HighScoreText;
    public TMP_Text PositiveRemarkText;
    public TMP_Text countdownText;
    public ParticleSystem confetti;

    // Contains scores from last game and the highest score.
    GameScoring scores;
    string countdown = "Tijd tot volgende scene: ";

    private float totalTime = 30;

    void Start()
    {
        scores = this.GetComponent<GameScoring>();

        // Update the current score.
        scores.UpdateText();

        // See if the high score has been beaten.
        CheckHighestScoreBeaten();

        // Add a random positive remark.
        RandomPositiveRemark();

        // Set score back to 0 to prepare for the next game. 
        scores.ResetScore();
    }

    private void CheckHighestScoreBeaten()
    {
        // When the high score has been beaten change it and display it properly.
        if (GameScoring.gameScore > GameScoring.highScore)
        {
            GameScoring.highScore = GameScoring.gameScore;
            HighScoreText.text = "High score: " + GameScoring.highScore + "(Nieuw!)";

            // Confetti when highest score was beaten.
            confetti.Play();
        }
        // If the high score wasn't beaten.
        else
        {
            HighScoreText.text = "High score: " + GameScoring.highScore;
        }
    }

    // Randomize which one you'll get.
    private void RandomPositiveRemark()
    {
        string remark = "";
        int ran = UnityEngine.Random.Range(1, 8);
        switch (ran)
        {
            case 1:
                remark = "Goed zo!";
                break;
            case 2:
                remark = "Uitstekend!";
                break;
            case 3:
                remark = "Geweldig!";
                break;
            case 4:
                remark = "Fantastisch!";
                break;
            case 5:
                remark = "Schitterend!";
                break;
            case 6:
                remark = "Klasse!";
                break;
            case 7:
                remark = "Prachtig!";
                break;
        }

        // Set text to remark.
        PositiveRemarkText.text = remark;
    }

    void Update()
    {
        // Tick down the timer.
        if (totalTime > 0)
        {
            totalTime -= Time.deltaTime;
            countdownText.text = countdown + Math.Round(totalTime);
        }
        else
        {
            countdownText.text = countdown + totalTime;
            SceneManager.LoadScene("MainGame");
        }
    }
}
