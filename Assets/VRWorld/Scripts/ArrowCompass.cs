using UnityEngine;
using System;

public class ArrowCompass : MonoBehaviour
{
    public float compassSmooth = 0.5f;
    private float m_lastMagneticHeading = 0f;
    void Start()
    {
        Input.location.Start();
        Input.compass.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        //do rotation based on compass
        float currentMagneticHeading = (float)Math.Round(Input.compass.magneticHeading, 2);
        if (m_lastMagneticHeading < currentMagneticHeading - compassSmooth || m_lastMagneticHeading > currentMagneticHeading + compassSmooth)
        {
            m_lastMagneticHeading = currentMagneticHeading;
            transform.localRotation = Quaternion.Euler(0, m_lastMagneticHeading, 0);
        }
    }
}
