using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actions : MonoBehaviour
{
    // These are the different options the players have to control their character.


    // Pass the action through.
    [Flags]
    public enum PlayerActions
    {
        Jump = 0,
        Crouch = 1,
        Left = 2, 
        Right = 3,
    }
}
