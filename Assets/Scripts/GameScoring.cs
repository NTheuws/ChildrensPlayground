using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameScoring : MonoBehaviour
{
    private int gameScore = 0;
    public TMP_Text displayTotalScore;
    private string scoreTxt = "Punten: ";

    void Start()
    {
        
    }

    void Update()
    {
        
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
}
