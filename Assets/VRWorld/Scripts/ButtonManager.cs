using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public StarGenerator starGenerator;
    public GameObject Compass;
    bool isCompassActive;

    private void Start()
    {
        isCompassActive = false;
    }
    public void ToggleStars()
    {
        starGenerator.ToggleStarVisibility();
    }
    public void ToggleCompass()
    {
        isCompassActive = !isCompassActive;
        Compass.SetActive(isCompassActive);
    }
}
