using UnityEngine;
using System.Collections;
using TMPro;
public class GPSService : MonoBehaviour
{
    public TextMeshPro logtext;
    public bool locationWebRequestDone;
    public int waitTime = 20;
    private void Start()
    {
        StartCoroutine(CheckLocationServiceStatus());
    }

    public IEnumerator CheckLocationServiceStatus()
    {
        logtext.text = "Started";
        if (!Input.location.isEnabledByUser)
        {
            yield break;
        }
        Input.compass.enabled = true;
        Input.location.Start();

        int waitTime = 20;
        while(Input.location.status == LocationServiceStatus.Initializing && waitTime > 0)
        {
            yield return new WaitForSeconds(1);
            waitTime--;
            logtext.text = waitTime.ToString();
        }
        if(waitTime < 1)
        {
            logtext.text = "Time out";
            yield break;
        }
        if(Input.location.status == LocationServiceStatus.Failed)
        {
            logtext.text = "failed";
            yield break;
        }
        else
        {
            logtext.text = "Running";
            InvokeRepeating("UpdateLocationData", 0.5f, 1f);
        }

    }
    private void UpdateLocationData()
    {
        if(Input.location.status == LocationServiceStatus.Running)
        {
            logtext.text = "location "+Input.location.lastData.latitude + " " + Input.location.lastData.longitude + "true heading " + Input.compass.trueHeading;
            locationWebRequestDone = true;
        }
        else
        {
            logtext.text = "stopped";
        }
    }
    public float GetNorthTrueHeading()
    {
        if (locationWebRequestDone)
        {
            return -Input.compass.trueHeading;
        }
        return 0f;
    }
    public (float, float) GetLatitudeAndLongitude()
    {
        if (locationWebRequestDone)
        {
            return (Input.location.lastData.latitude, Input.location.lastData.longitude);
        }
        return (0f, 0f);
    }
    public void StartLocationService()
    {
        StartCoroutine(CheckLocationServiceStatus());
    }
}