using UnityEngine;
using System.Collections;

public class LocationService : MonoBehaviour
{
    public bool locationWebRequestDone;
    IEnumerator Start()
    {
        locationWebRequestDone = false;
        // Check if the user has location service enabled.
        if (!Input.location.isEnabledByUser)
            yield break;

        // Starts the location service.
        Input.location.Start();
        Input.compass.enabled = true;

        // Waits until the location service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // If the service didn't initialize in 20 seconds this cancels location service use.
        if (maxWait < 1)
        {
            print("Timed out");
            yield break;
        }

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            locationWebRequestDone = true;
            // If the connection succeeded, this retrieves the device's current location and displays it in the Console window.
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }

        // Stops the location service if there is no need to query location updates continuously.
        Input.location.Stop();
    }
    public float GetNorthTrueHeading()
    {
        if(locationWebRequestDone)
        {
            return -Input.compass.trueHeading;
        }
        return 0f;
    }
    public (float,float) GetLatitudeAndLongitude()
    {
        if(locationWebRequestDone)
        {
            return (Input.location.lastData.latitude, Input.location.lastData.longitude);
        }
        return (0f, 0f);
    }
}