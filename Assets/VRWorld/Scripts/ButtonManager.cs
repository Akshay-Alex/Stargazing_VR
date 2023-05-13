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
