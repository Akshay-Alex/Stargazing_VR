using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityButtonData : MonoBehaviour
{
    public LocationManager locationManager;
    public string cityName;
    public string Latitude;
    public string Longitude;
    public void DisplayCoordinates()
    {
        locationManager.DisplayCoordinatesOfCity(this);
    }
}
