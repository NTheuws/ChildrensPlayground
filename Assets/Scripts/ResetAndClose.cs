using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetAndClose : MonoBehaviour
{
    void Update()
    {
        // Close game.
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
        // Go to main game/Skip intro and or tutorial
        if (Input.GetKey("space"))
        {
            SceneManager.LoadScene("MainGame");
        }
    }
}
