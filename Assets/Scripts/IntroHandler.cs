using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroHandler : MonoBehaviour
{
    public TMP_Text countDown;
    public TMP_Text scenarioText;
    private float zoomSpeed = 30f;
    private bool startZoom = false;
    private bool countingToStartGame = false;
    void Start()
    {
        // Sketch the scenario first.
        StartCoroutine(WaitToStart());
    }

    void Update()
    {
        if (startZoom)
        {
            // Zoom into the screen.
            SlowZoomIn();
        }
        if (Camera.main.fieldOfView == 22 && !countingToStartGame)
        {
            countingToStartGame= true;
            StartCoroutine(CountingDown(1));
        }
    }

    private void SlowZoomIn()
    {
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 22, Time.deltaTime / zoomSpeed);
        zoomSpeed = zoomSpeed / 1.01f;
    }

    IEnumerator WaitToStart()
    {
        yield return new WaitForSeconds(3);

        // Sketch the scenario for the theme.
        scenarioText.text = "De kaartjes voor de film zijn het scherm in gezogen?! We moeten ze terughalen voordat de film begint, anders zijn de mensen voor niets gekomen. Snel! voordat we de kaartjes kwijt zijn in de jungle..";
        yield return new WaitForSeconds(15);
        scenarioText.text = "";
        startZoom = true;
    }

    IEnumerator CountingDown(float val)
    {        
        // Start counting down.
        countDown.text = "3";
        yield return new WaitForSeconds(val);
        countDown.text = "2";
        yield return new WaitForSeconds(val);
        countDown.text = "1";
        yield return new WaitForSeconds(val);
        countDown.text = "Actie!";
        yield return new WaitForSeconds(val);
        // Switch scene.
        SceneManager.LoadScene("Tutorial");
    }
}
