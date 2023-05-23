using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroHandler : MonoBehaviour
{
    public TMP_Text CountDown;
    private float zoomSpeed = 30f;
    private bool startZoom = false;
    private bool CountingToStartGame = false;
    void Start()
    {
        StartCoroutine(WaitToStart(5));
    }

    void Update()
    {
        if (startZoom)
        {
            SlowZoomIn();
        }
        if (Camera.main.fieldOfView == 22 && !CountingToStartGame)
        {
            CountingToStartGame= true;
            StartCoroutine(CountingDown(1));
        }
    }

    private void SlowZoomIn()
    {
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 22, Time.deltaTime / zoomSpeed);
        zoomSpeed = zoomSpeed / 1.01f;
    }

    IEnumerator WaitToStart(int val)
    {
        yield return new WaitForSeconds(val);
        startZoom = true;
    }

    IEnumerator CountingDown(float val)
    {
        CountDown.text = "3";
        yield return new WaitForSeconds(val);
        CountDown.text = "2";
        yield return new WaitForSeconds(val);
        CountDown.text = "1";
        yield return new WaitForSeconds(val);
        CountDown.text = "";
        yield return new WaitForSeconds(val);
        // Switch scene.
        SceneManager.LoadScene("MainScene");
    }
}
