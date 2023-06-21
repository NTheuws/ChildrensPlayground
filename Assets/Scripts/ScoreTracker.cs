using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreTracker : MonoBehaviour
{
    public TMP_Text HighScoreText;
    public TMP_Text PositiveRemarkText;

    // Contains scores from last game and the highest score.
    GameScoring scores;

    private float totalTime = 15;

    void Start()
    {
        scores = this.GetComponent<GameScoring>();
        //SceneManager.UnloadScene("MainGame");

        // Update the current score.
        scores.UpdateText();

        // See if the high score has been beaten.
        CheckHighestScoreBeaten();

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

        }
        // If the high score wasn't beaten.
        else
        {
            HighScoreText.text = "High score: " + GameScoring.highScore;
        }
    }

    void Update()
    {
        if (totalTime > 0)
        {
            totalTime -= Time.deltaTime;
        }
        else
        {
            SceneManager.LoadScene("MainGame");
        }
    }
}
