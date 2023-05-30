using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScoring : MonoBehaviour
{
    private int gameScore = 0;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void PointScored(int val)
    {
        gameScore += val;
    }
}
