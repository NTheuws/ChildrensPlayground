using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class TicketGame : MonoBehaviour
{
    int TicketValue = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // On contact remove object and add 1 to the score.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Add point to score.
        GameScoring scoring = (GameScoring)FindObjectOfType(typeof(GameScoring));
        if (scoring != null && scoring.tag == "Score") 
        {
            scoring.PointScored(TicketValue);
        }
        // Get rid of object.
        Destroy(this.gameObject);
    }
}
