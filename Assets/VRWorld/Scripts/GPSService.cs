using UnityEngine;
using System.Collections;
using TMPro;
public class GPSService : MonoBehaviour
{
    public TextMeshPro logtext;
    public bool locationWebRequestDone,compassRunning = false;
    public int waitTime = 20;
    public float compassvalue = 0f;
    public int count = 0;
    private void Start()
    {
    }

    public IEnumerator CheckLocationServiceStatus()
    {
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.CoarseLocation))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.CoarseLocation);
        }

        // First, check if user has location service enabled
        if (!UnityEngine.Input.location.isEnabledByUser)
        {
            // TODO Failure
           Debug.Log("Android and Location not enabled");
            yield break;
        }
        Input.location.Start(600);
        waitTime = 20;
        /*
        StartCoroutine(StartCompass());
        if (!compassRunning)
        {
            // TODO Failure
            Debug.Log("Compass not started");
            yield break;
        }
        */
        Debug.Log("Function called " + " compass enabled " + Input.compass.enabled + " location service status " + Input.location.status);
        waitTime = 20;
        while(Input.location.status == LocationServiceStatus.Initializing && waitTime > 0)
        {
            yield return new WaitForSeconds(1);
            waitTime--;
            Debug.Log(waitTime + " compass enabled " + Input.compass.enabled + " location service status " + Input.location.status);
        }
        if(waitTime < 1)
        {
            Debug.Log("Time out" + " compass enabled " + Input.compass.enabled + " location service status " + Input.location.status);
            //yield break;
        }
        if(Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("failed" + " compass enabled " + Input.compass.enabled + " location service status " + Input.location.status);
            //yield break;
        }
        else if(Input.location.status == LocationServiceStatus.Running)
        {
            Debug.Log("Running");
            UpdateLocationData();
        }
        Input.location.Stop();
        StopCoroutine(CheckLocationServiceStatus());
        Debug.Log("Function call end " + " compass enabled " + Input.compass.enabled + " location service status " + Input.location.status);
    }
    private void UpdateLocationData()
    {
        Debug.Log("location " +Input.location.lastData.latitude + " " + Input.location.lastData.longitude + "true heading " + Input.compass.trueHeading);
            //compassvalue = Input.compass.trueHeading;
            locationWebRequestDone = true;
    }
    public float GetNorthTrueHeading()
    {
        if (locationWebRequestDone)
        {
            return compassvalue;
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
    public IEnumerator StartCompass()
    {
        if (Input.compass.enabled)
        {
            compassRunning = true;
            Debug.Log("Compass Started");
            yield return null;
        }
        else if (waitTime < 0)
        {
            Debug.Log("Compass Timeout");
            yield return null;
        }
        else
        {
            waitTime--;
            Input.compass.enabled = true;
            Debug.Log("Waiting for compass to start " + waitTime);
            yield return new WaitForSeconds(1);
            StartCoroutine(StartCompass());
        }
    }
    private void Update()
    {
        logtext.text = "Compass enabled : " + Input.compass.enabled + " Location Service status " + Input.location.status;
    }
}