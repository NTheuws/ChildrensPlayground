using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private int tutorialStep = 1;
    // Keeps track of the amount of players.
    public int totalPlayers = 0;
    // Keeps track of the players that have done the action required to progress to the next step.
    private int[] succeededPlayers = { 0, 0, 0, 0 };

    // Obstacle used for the tutorial.
    public TutorialObstacle obstaclePrefab;
    private TutorialObstacle obstacle;
    private bool spawnedObstacle = false;

    private void Start()
    {
        obstacle = Instantiate(obstaclePrefab);
    }

    public void tutorialSteps(PlayerBehaviour player, int playerNumber ,Actions.PlayerActions action)
    {
        bool containsPlayer = false;
        if (succeededPlayers[playerNumber-1] == 1)
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
                    if (action == Actions.PlayerActions.Left)
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
                    tutorialStep++;
                    break;
                // Getting hit by obstacle
                case 6:
                    obstacle.started = true;
                    /*
                    if (!spawnedObstacle)
                    {
                        spawnedObstacle = true;
                        obstacle = Instantiate(obstaclePrefab);
                    }*/
                    if (obstacle.hitPlayer)
                    {
                        tutorialStep++;
                    }
                    break;
                // Dodge obstacle
                case 7:
                    if (action == Actions.PlayerActions.Jump)
                    {
                        player.PlayerJump();
                        obstacle.slowedSpeed = 3f;
                        // Toggle player in array.
                        succeededPlayers[playerNumber - 1] = 1;
                    }
                    if (action == Actions.PlayerActions.Crouch)
                    {
                        player.PlayerCrouch();
                    }
                    break;
            }
            // See if the step has been completed by all players.
            CheckStepSucceeded();

            //Check if the tutorial has ended.
            if (obstacle.ObstacleDone)
            {
                Destroy(obstacle);
                tutorialOngoing = false;
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
            tutorialStep++;
        }
        // reset array
        succeededPlayers = new int[] { 0, 0, 0, 0 };
    }
}
