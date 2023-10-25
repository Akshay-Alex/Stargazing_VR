using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ButtonManager : MonoBehaviour
{
    public GameObject logs;
    public GameObject compass;
    public GameObject menu;

    private void Start()
    {
        //ShowAllCityNames();
        //SearchCity();
        //Debug.Log(cityData.Cities.Count);
    }
    
   
    public void ToggleStars()
    {
        StarGenerator.starGenerator.ToggleStarVisibility();
        SFXSoundsManager.sFXSoundsManager.PlayButtonClickSFX();
    }
    public void ToggleLogs()
    {
        logs.SetActive(!logs.activeSelf);
        SFXSoundsManager.sFXSoundsManager.PlayButtonClickSFX();
    }
    public void ToggleCompass()
    {
        compass.SetActive(!compass.activeSelf);
        SFXSoundsManager.sFXSoundsManager.PlayButtonClickSFX();
    }
    public void ToggleMenu()
    {
        menu.SetActive(!menu.activeSelf);
        SFXSoundsManager.sFXSoundsManager.PlayButtonClickSFX();
    }
    public void QuitApp()
    {
        Application.Quit();
    }
}
