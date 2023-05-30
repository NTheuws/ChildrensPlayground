using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovieclipGame : MonoBehaviour
{
    private int clipValue = -3;

    // On contact remove object and add 1 to the score.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Add point to score.
        GameScoring scoring = (GameScoring)FindObjectOfType(typeof(GameScoring));
        if (scoring != null && scoring.tag == "Score")
        {
            scoring.PointScored(clipValue);
        }
        // Get rid of object.
        Destroy(this.gameObject);
    }
}
