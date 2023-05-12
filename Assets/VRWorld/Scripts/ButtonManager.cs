using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public StarGenerator starGenerator;
    public GameObject logs;

    private void Start()
    {
    }
    public void ToggleStars()
    {
        starGenerator.ToggleStarVisibility();
    }
    public void ToggleLogs()
    {
        logs.SetActive(!logs.activeSelf);
    }
}
