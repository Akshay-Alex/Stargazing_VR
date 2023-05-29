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
    public float buttonYOffset = -23.6f;
    void ShowAllCityNames()
    {
        foreach (CityData.City city in cityData.Cities)
        {
            Debug.Log(city.Name);
        }
    }
    public void SearchCity()
    {
        Debug.Log("SearchCity called");
        if (inputField.text.Length > 2)
        {
            IEnumerable<CityData.City> similarCities = cityData.Cities.Where(city => city.AlternateName.Contains(inputField.text));
            if (similarCities.Count() == 0)
            {
                Debug.Log("empty");
            }
            else
            {
                int buttonPosition = 1;
                foreach (CityData.City city in similarCities)
                {
                    var currentCityButton = GameObject.Instantiate(buttonWithCityName, scrollViewContentTransform);
                    CreateCityButton(currentCityButton,buttonPosition, city);
                    buttonPosition++;
                    //currentCityButton.GetComponent<RectTransform>().position
                    //Debug.Log("City Name " + city.Name + " City co ordinates " + city.Coordinates);
                }
            }
        }
        
    }
    void CreateCityButton(GameObject button, int buttonPosition,CityData.City cityData)
    {
        var rectTransform = button.GetComponent<RectTransform>();
        var CityBittonData = button.GetComponent<CityButtonData>();
        rectTransform.rect.Set(0f, buttonYOffset + rectTransform.rect.width * buttonPosition, rectTransform.rect.width, rectTransform.rect.height);
        button.GetComponentInChildren<TextMeshProUGUI>().text = cityData.Name;
    }
    void InitializeKeyboard()
    {
        NonNativeKeyboard.Instance.InputField = inputField;
        NonNativeKeyboard.Instance.PresentKeyboard(inputField.text);
    }
    // Start is called before the first frame update
    void Start()
    {
        InitializeKeyboard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
