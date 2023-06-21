using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameScoring : MonoBehaviour
{
    public static int highScore = 0;
    public static int gameScore = 0;
    public TMP_Text displayTotalScore;
    private string scoreTxt = "Punten: ";

    public void UpdateText()
    {
        displayTotalScore.text = scoreTxt + gameScore.ToString();
    }

    public void PointScored(int val)
    {
        // Add point to the total score.
        gameScore += val;
        if(gameScore < 0)
        {
            gameScore= 0;
        }    
        displayTotalScore.text = scoreTxt + gameScore.ToString();
    }

    // Reset the score back to 0;
    public void ResetScore()
    {
        gameScore = 0;
    }
}
