using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class GestureDetection : MonoBehaviour
{
    public UnityEvent LeftTriggerPressed, LeftTriggerReleased, RightTriggerPressed,RightTriggerReleased;
    InputFeatureUsage<bool> triggerPressFeature = CommonUsages.triggerButton;
    InputDeviceCharacteristics leftControllerCharacterisitic;
    InputDeviceCharacteristics rightControllerCharacterisitic;
    List<InputDevice> leftControllers;
    List<InputDevice> rightControllers;
    bool isLeftTriggerPressed, isRightTriggerPressed, leftTriggerOutValue, rightTriggerOutValue;
    public static GestureDetection gestureDetection;
    // Start is called before the first frame update
    void Start()
    {
        gestureDetection = this;
        leftControllerCharacterisitic = InputDeviceCharacteristics.Left;
        rightControllerCharacterisitic = InputDeviceCharacteristics.Right;
        leftControllers = new List<InputDevice>();
        rightControllers = new List<InputDevice>();
        TestEvents();
    }
    void TestEvents()
    {
        LeftTriggerPressed.AddListener(OnLeftTriggerPressed);
        RightTriggerPressed.AddListener(OnRightTriggerPressed);
        LeftTriggerReleased.AddListener(OnLeftTriggerReleased);
        RightTriggerReleased.AddListener(OnRightTriggerReleased);
    }
    void OnLeftTriggerPressed()
    {
        Debug.Log(" OnLeftTriggerPressed ");
    }
    void OnRightTriggerPressed()
    {
        Debug.Log(" OnRightTriggerPressed ");
    }
    void OnLeftTriggerReleased()
    {
        Debug.Log(" OnLeftTriggerReleased ");
    }
    void OnRightTriggerReleased()
    {
        Debug.Log(" OnRightTriggerReleased ");
    }
    // Update is called once per frame
    void Update()
    {
        CheckLeftTrigger();
        CheckRightTrigger();



    }
    void CheckLeftTrigger()
    {
        InputDevices.GetDevicesWithCharacteristics(leftControllerCharacterisitic, leftControllers);
        for (int index = 0; index < leftControllers.Count; index++)
        {
            leftControllers[index].TryGetFeatureValue(triggerPressFeature, out leftTriggerOutValue);
            if (leftTriggerOutValue == true && isLeftTriggerPressed == false)
            {
                isLeftTriggerPressed = true;
                LeftTriggerPressed.Invoke();
            }
            else if (leftTriggerOutValue == false && isLeftTriggerPressed == true)
            {
                isLeftTriggerPressed = false;
                LeftTriggerReleased.Invoke();
            }
        }
    }
    void CheckRightTrigger()
    {
        InputDevices.GetDevicesWithCharacteristics(rightControllerCharacterisitic, rightControllers);
        for (int index = 0; index < rightControllers.Count; index++)
        {
            rightControllers[index].TryGetFeatureValue(triggerPressFeature, out rightTriggerOutValue);
            if (rightTriggerOutValue == true && isRightTriggerPressed == false)
            {
                isRightTriggerPressed = true;
                RightTriggerPressed.Invoke();
            }
            else if (rightTriggerOutValue == false && isRightTriggerPressed == true)
            {
                isRightTriggerPressed = false;
                RightTriggerReleased.Invoke();
            }
        }
    }
}
