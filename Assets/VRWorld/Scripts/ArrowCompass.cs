using UnityEngine;
using System;

public class ArrowCompass : MonoBehaviour
{
    public LocationService locationService;
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(locationService.locationWebRequestDone)
        {
            transform.rotation = Quaternion.Euler(0, locationService.GetNorthTrueHeading(), 0);
        }
       
    }
}
