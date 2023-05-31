using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Tutorial : MonoBehaviour
{
    /* The tutorial is meant to show the different kind of interactions and controls.
     * The controls that currently exist in the game are:
     * 1. Jumping
     * 2. Moving left
     * 3. Moving right
     * 4. Moving down a platform
     * 
     * The interaction that exist are:
     * 5. Collecting points
     * 6. Getting hit by an obsticle
     * 7. Dodge obstacle
     */

    // See if the tutorial has been completed or not.
    public bool tutorialOngoing = true;
    // Tracks which step the tutorial is at.
    private int tutorialStep = 0;
    // Keeps track of the amount of players.
    public int totalPlayers = 0;
    // Keeps track of the players that have done the action required to progress to the next step.
    private int[] succeededPlayers = { 0, 0, 0, 0 };
    // Instructions for the controls will be shown on a text.
    public TMP_Text instructionsText;
    // Obstacle used for the tutorial.
    public TutorialObstacle obstaclePrefab;
    private TutorialObstacle obstacle;
    // Points to be collected.
    public PointBehaviour pointObject;
    // Score count.
    public TMP_Text currentScore;

    private bool alreadyJumped = false;
    private bool firstMessage = true;

    private void Start()
    {
        obstacle = Instantiate(obstaclePrefab);
        instructionsText.enabled = false;
    }

    private void Update()
    {
        switch (tutorialStep)
        {
            case 1:
                StartCoroutine(WaitForSecondsCoroutine(5));
                break;
            case 2:
                instructionsText.text = "Bukt..";
                break;
            case 3:
                instructionsText.text = "Naar links loopt..";
                break;
            case 4:
                instructionsText.text = "en naar rechts..";
                break;
            case 5:
                currentScore.text = "Punten: 0";
                instructionsText.text = "Tickets geven je punten.";
                pointObject.started = true;
                if (pointObject.hitPlayer)
                {
                    currentScore.text = "Punten: 1";
                    tutorialStep++;
                }
                break;
            case 6:
                if (!alreadyJumped)
                {
                    instructionsText.text = "Als je geraakt wordt door iets anders \ngaan je punten omlaag.";
                }
                obstacle.started = true;

                if (obstacle.hitPlayer)
                {
                    currentScore.text = "Punten: 0";
                    tutorialStep++;
                    instructionsText.text = "Spring eroverheen!";
                }
                break;
            default:
                if (!obstacle.hitPlayer)
                {
                    instructionsText.text = "";
                }
                break;
        }
    }

    public void tutorialSteps(PlayerBehaviour player, int playerNumber, Actions.PlayerActions action)
    {
        if (tutorialOngoing)
        {
            bool containsPlayer = false;
            if (succeededPlayers[playerNumber - 1] == 1)
            {
                containsPlayer = true;
            }

            if (!containsPlayer)
            {
                switch (tutorialStep)
                {

                    // Jump
                    case 1:
                        if (action == Actions.PlayerActions.Jump)
                        {
                            player.PlayerJump();
                            // Toggle player in array.
                            succeededPlayers[playerNumber - 1] = 1;
                        }
                        break;
                    // Move down
                    case 2:
                        if (action == Actions.PlayerActions.Crouch)
                        {
                            player.PlayerCrouch();
                            // Toggle player in array.
                            succeededPlayers[playerNumber - 1] = 1;
                        }
                        break;
                    // Move left
                    case 3:
                        if (action == Actions.PlayerActions.Left && player.isGrounded)
                        {
                            player.MoveLeft();
                            // Toggle player in array.
                            succeededPlayers[playerNumber - 1] = 1;
                        }
                        break;
                    // Move right
                    case 4:
                        if (action == Actions.PlayerActions.Right)
                        {
                            player.MoveRight();
                            // Toggle player in array.
                            succeededPlayers[playerNumber - 1] = 1;
                        }
                        break;
                    // Collect coin
                    case 5:
                    // Getting hit by obstacle
                    case 6:
                        // Option to dodge, when dodged skip step 7.
                        if (action == Actions.PlayerActions.Jump)
                        {
                            alreadyJumped = true;
                            instructionsText.text = "";
                            player.PlayerJump();
                        }
                        break;
                    // Dodge obstacle
                    case 7:
                        if (action == Actions.PlayerActions.Jump)
                        {
                            alreadyJumped = true;
                            instructionsText.text = "";
                            player.PlayerJump();
                        }
                        break;
                }
                // See if the step has been completed by all players.
                CheckStepSucceeded();

                //Check if the tutorial has ended.
                if (obstacle.ObstacleDone)
                {
                    instructionsText.enabled = false;
                    tutorialOngoing = false;
                }
            }
        }
    }

    // Checks if the amount of players that have done the step equals the total player count.
    private void CheckStepSucceeded()
    {
        int val = 0;
        for (int i = 0; i < totalPlayers; i++)
        {
            val += succeededPlayers[i];
        }

        if (val == totalPlayers)
        {
            // reset array
            succeededPlayers = new int[] { 0, 0, 0, 0 };
            tutorialStep++;
        }

        if (tutorialStep == 8 || tutorialStep == 7)
        {
            instructionsText.text = "";
        }
    }

    IEnumerator WaitForSecondsCoroutine(int val)
    {
        if (firstMessage)
        {
            firstMessage = false;
            instructionsText.text = "Door zelf te bewegen, \nbeweeg je het karakter..";
            yield return new WaitForSeconds(val);
            instructionsText.text = "Dus als je springt..";
        }
        else
        {
            yield return new WaitForSeconds(val);
        }
    }

    public void EnableText()
    {
        instructionsText.enabled = true;
        tutorialStep = 1;
    }
}
