using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro;

public class LocationManager : MonoBehaviour
{
    public CityData cityData;
    public Transform scrollViewContentTransform;
    public GameObject buttonWithCityName;
    public TMP_InputField inputField;
    public GameObject scrollViewParentObject;
    public GameObject confirmDialogBox;
    public GameObject parentCanvas;
    public bool _debug;
    public enum TimeScale
    {
        hours,
        days,
        months
    }
    public TimeScale timeScale;
    //public float buttonYOffset = -23.6f;
    public List<GameObject> currentCityButtons = new List<GameObject>();
    void ShowAllCityNames()
    {
        foreach (CityData.City city in cityData.Cities)
        {
            Debug.Log(city.Name);
        }
    }
    public void ConfirmCity()
    {
        StarGenerator.starGenerator.GenerateAndPositionStars();
        //StarGenerator.starGenerator.StartTimerForUpdatingStarPosition();
        parentCanvas.SetActive(false);
        NonNativeKeyboard.Instance.gameObject.SetActive(false);
        if(_debug)
        {
            Debug.Log("City confirmed");
        }
    }
    public void SearchCity()
    {
        //Debug.Log("SearchCity called");
        if (inputField.text.Length > 2)
        {
            IEnumerable<CityData.City> similarCities = cityData.Cities.Where(city => city.AlternateName.Contains(inputField.text));
            if (similarCities.Count() == 0)
            {
                Debug.Log("empty");
            }
            else
            {
                DeleteOldCityButtons();
                currentCityButtons.Clear();
                foreach (CityData.City city in similarCities)
                {
                    var currentCityButton = GameObject.Instantiate(buttonWithCityName, scrollViewContentTransform);
                    CreateCityButton(currentCityButton, city);
                    currentCityButtons.Add(currentCityButton);
                    //currentCityButton.GetComponent<RectTransform>().position
                    //Debug.Log("City Name " + city.Name + " City co ordinates " + city.Coordinates);
                }
            }
        }
        if (_debug)
        {
            Debug.Log("Search city function called");
        }
    }
    public void ShowConfirmDialogBox()
    {
        scrollViewParentObject.SetActive(false);
        confirmDialogBox.SetActive(true);
    }
    public void HideConfirmDialogBox()
    {
        scrollViewParentObject.SetActive(true);
        confirmDialogBox.SetActive(false);
    }
    void DeleteOldCityButtons()
    {
        foreach (GameObject button in currentCityButtons)
        {
            Destroy(button);
        }
    }
    void CreateCityButton(GameObject button,CityData.City cityData)
    {
        var CityButtonData = button.GetComponent<CityButtonData>();
        string[] Coordinates = cityData.Coordinates.Split(',');
        CityButtonData.Latitude = Coordinates[0];
        CityButtonData.Longitude = Coordinates[1];
        CityButtonData.locationManager = this;
        button.GetComponentInChildren<TextMeshProUGUI>().text = cityData.Name;
    }
    void InitializeKeyboard()
    {
        NonNativeKeyboard.Instance.InputField = inputField;
        NonNativeKeyboard.Instance.PresentKeyboard(inputField.text);
    }
    public void ChangeTimeScale()
    {
        int newTimeScale = ((int)timeScale + 1) % 3;
        timeScale =(TimeScale) newTimeScale;
        if (_debug)
        {
            Debug.Log("Timescale changed to " + timeScale);
        }
        
    }
    void SubscribeToGestureEvent()
    {
        GestureDetection.gestureDetection.LeftSecondaryButtonPressed.AddListener(ChangeTimeScale);
        GestureDetection.gestureDetection.RightSecondaryButtonPressed.AddListener(ChangeTimeScale);
    }
    void UnSubscribeToGestureEvent()
    {
        GestureDetection.gestureDetection.LeftSecondaryButtonPressed.RemoveAllListeners();
        GestureDetection.gestureDetection.RightSecondaryButtonPressed.RemoveAllListeners();
    }
    void OnDestroy()
    {
        UnSubscribeToGestureEvent();
    }
    /*
    public void DisplayCoordinatesOfCity(CityButtonData data)
    {
        Debug.Log("Latitude " + data.Latitude + " Longitude " + data.Longitude);
    }
    */
    // Start is called before the first frame update
    void Start()
    {
        InitializeKeyboard();
        timeScale = TimeScale.hours;
        SubscribeToGestureEvent();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
