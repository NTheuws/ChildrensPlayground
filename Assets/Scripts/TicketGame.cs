using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class TicketGame : MonoBehaviour
{
    private int ticketValue = 1;

    // On contact remove object and add 1 to the score.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Add point to score.
        GameScoring scoring = (GameScoring)FindObjectOfType(typeof(GameScoring));
        if (scoring != null && scoring.tag == "Score") 
        {
            scoring.PointScored(ticketValue);
        }
        // Get rid of object.
        Destroy(this.gameObject);
    }
}
