using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ButtonManager : MonoBehaviour
{
    public GameObject logs;
    public GameObject compass;
    public GameObject menu;
    public CityData cityData;

    private void Start()
    {
        //ShowAllCityNames();
        SearchCity();
        //Debug.Log(cityData.Cities.Count);
    }
    void ShowAllCityNames()
    {
        foreach(CityData.City city in cityData.Cities)
        {
            Debug.Log(city.Name);
        }
    }
    void SearchCity()
    {
        string searchString = "kochi";
        IEnumerable<CityData.City> citiesWithSameName = cityData.Cities.Where(city => city.AlternateName.Contains(searchString));


        //Debug.Log(citiesWithSameName.Last().Name);
        if (citiesWithSameName.Count() == 0)
        {
            Debug.Log("empty");
        }
        else
        {
            foreach(CityData.City city in citiesWithSameName)
            {
                Debug.Log("City Name " + city.Name + " City co ordinates " + city.Coordinates);
            }
        }
    }
    public void ToggleStars()
    {
        StarGenerator.SG.ToggleStarVisibility();
    }
    public void ToggleLogs()
    {
        logs.SetActive(!logs.activeSelf);
    }
    public void ToggleCompass()
    {
        compass.SetActive(!compass.activeSelf);
    }
    public void ToggleMenu()
    {
        menu.SetActive(!menu.activeSelf);
    }
}
