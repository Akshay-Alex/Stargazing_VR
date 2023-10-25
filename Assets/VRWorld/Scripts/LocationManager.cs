using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro;
using System;

public class LocationManager : MonoBehaviour
{
    public CityData cityData;
    public Transform scrollViewContentTransform;
    public GameObject buttonWithCityName;
    public TMP_InputField inputField;
    public GameObject scrollViewParentObject;
    public GameObject confirmDialogBox;
    public GameObject parentCanvas;
    public TextMeshProUGUI timeSkipText;
    public TextMeshProUGUI dateTimeText;
    public GameObject cameraUIGO;
    public bool _debug;
    public float deltaValueMultiplier;
    public float cameraUIPersistanceTime;
    public Timer cameraUITimer;
    public enum TimeScale
    {
        hours,
        days,
        weeks
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
        SFXSoundsManager.sFXSoundsManager.PlayButtonClickSFX();
        StarGenerator.starGenerator.GenerateAndPositionStars();
        //StarGenerator.starGenerator.StartTimerForUpdatingStarPosition();
        parentCanvas.SetActive(false);
        NonNativeKeyboard.Instance.gameObject.SetActive(false);
        GestureDetection.gestureDetection._enableVfX = true;
        if (_debug)
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
        SFXSoundsManager.sFXSoundsManager.PlayButtonClickSFX();
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
        if(StarGenerator.starGenerator.starsGenerated)
        {
            int newTimeScale = ((int)timeScale + 1) % 3;
            timeScale = (TimeScale)newTimeScale;
            timeSkipText.text = "Current time skip interval: " + timeScale;
            ShowCameraUI();
            SFXSoundsManager.sFXSoundsManager.PlayButtonClickSFX();
            if (_debug)
            {
                Debug.Log("Timescale changed to " + timeScale);
            }
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
    void ChangeGameTime()
    {
        if (StarGenerator.starGenerator.starsGenerated)
        {
            int hoursToAdd = 0;
            switch (timeScale)
            {
                case TimeScale.hours:
                    hoursToAdd = (int)Mathf.Round((float)(deltaValueMultiplier * GestureDetection.gestureDetection.deltaValue));
                    break;
                case TimeScale.days:
                    hoursToAdd = (int)Mathf.Round((float)(deltaValueMultiplier * GestureDetection.gestureDetection.deltaValue * 24));
                    break;
                case TimeScale.weeks:
                    hoursToAdd = (int)Mathf.Round((float)(deltaValueMultiplier * GestureDetection.gestureDetection.deltaValue * 168));
                    break;
                default:
                    break;
            }
            StarGenerator.starGenerator.UpdateStarPosition(hoursToAdd);
            dateTimeText.text = "In-game date and time: " + StarGenerator.starGenerator.currentGameTime.ToLocalTime().ToString();
            ShowCameraUI();
            if (_debug)
            {
                Debug.Log("Adding " + hoursToAdd + " hours to game time");
            }
        }
    }
    void OnDestroy()
    {
        UnSubscribeToGestureEvent();
        UnSubscribeToTimeSkip();
    }
    void ShowCameraUI()
    {
        if(!cameraUIGO.activeSelf)
        {
            cameraUIGO.SetActive(true);
            cameraUITimer.SetTimer(cameraUIPersistanceTime);
            cameraUITimer.TimerFinished += HideCameraUI;
        }

    }
    void HideCameraUI()
    {
        cameraUITimer.TimerFinished -= HideCameraUI;
        if (cameraUIGO.activeSelf)
        {
            cameraUIGO.SetActive(false);
        }
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
        SetCameraUI();
        SubscribeToGestureEvent();
        SubscribeToTimeSkip();
    }
    void SetCameraUI()
    {
        timeScale = TimeScale.hours;
        cameraUITimer = gameObject.AddComponent<Timer>();
        timeSkipText.text = timeScale.ToString();
        dateTimeText.text = DateTime.Now.ToLocalTime().ToString();
    }
    void SubscribeToTimeSkip()
    {
        GestureDetection.gestureDetection.GripReleasedAfterBeingHeldDown.AddListener(ChangeGameTime);
    }
    void UnSubscribeToTimeSkip()
    {
        GestureDetection.gestureDetection.GripReleasedAfterBeingHeldDown.RemoveAllListeners();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
