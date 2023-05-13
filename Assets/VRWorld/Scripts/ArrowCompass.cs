using UnityEngine;
using System;


public class ArrowCompass : MonoBehaviour
{
    public GPSService gPSService;
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(gPSService.locationWebRequestDone)
        {
            transform.rotation = Quaternion.Euler(90, 0, gPSService.GetNorthTrueHeading());
        }
       
    }
}
