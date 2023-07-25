using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class GestureDetection : MonoBehaviour
{
    public UnityEvent LeftGripPressed, LeftGripReleased, RightGripPressed,RightGripReleased;
    public Transform leftControllerReference, rightControllerReference;
    InputFeatureUsage<bool> gripPressFeature = CommonUsages.gripButton;
    InputDeviceCharacteristics leftControllerCharacterisitic;
    InputDeviceCharacteristics rightControllerCharacterisitic;
    List<InputDevice> leftControllers;
    List<InputDevice> rightControllers;
    bool isLeftGripPressed, isRightGripPressed, leftGripOutValue, rightGripOutValue;
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
        LeftGripPressed.AddListener(OnLeftGripPressed);
        RightGripPressed.AddListener(OnRightGripPressed);
        LeftGripReleased.AddListener(OnLeftGripReleased);
        RightGripReleased.AddListener(OnRightGripReleased);
    }
    void OnLeftGripPressed()
    {
        Debug.Log(" OnLeftGripPressed ");
        Debug.Log("Left :" + leftControllerReference.localPosition);
    }
    void OnRightGripPressed()
    {
        Debug.Log(" OnRightGripPressed ");
        Debug.Log("Right :" + rightControllerReference.localPosition);
    }
    void OnLeftGripReleased()
    {
        Debug.Log(" OnLeftGripReleased ");
        Debug.Log("Left :" + leftControllerReference.localPosition);
    }
    void OnRightGripReleased()
    {
        Debug.Log(" OnRightGripReleased ");
        Debug.Log("Right :" + rightControllerReference.localPosition);
    }
    // Update is called once per frame
    void Update()
    {
        CheckLeftGrip();
        CheckRightGrip();



    }
    void CheckLeftGrip()
    {
        InputDevices.GetDevicesWithCharacteristics(leftControllerCharacterisitic, leftControllers);
        for (int index = 0; index < leftControllers.Count; index++)
        {
            leftControllers[index].TryGetFeatureValue(gripPressFeature, out leftGripOutValue);
            if (leftGripOutValue == true && isLeftGripPressed == false)
            {
                isLeftGripPressed = true;
                LeftGripPressed.Invoke();
            }
            else if (leftGripOutValue == false && isLeftGripPressed == true)
            {
                isLeftGripPressed = false;
                LeftGripReleased.Invoke();
            }
        }
    }
    void CheckRightGrip()
    {
        InputDevices.GetDevicesWithCharacteristics(rightControllerCharacterisitic, rightControllers);
        for (int index = 0; index < rightControllers.Count; index++)
        {
            rightControllers[index].TryGetFeatureValue(gripPressFeature, out rightGripOutValue);
            if (rightGripOutValue == true && isRightGripPressed == false)
            {
                isRightGripPressed = true;
                RightGripPressed.Invoke();
            }
            else if (rightGripOutValue == false && isRightGripPressed == true)
            {
                isRightGripPressed = false;
                RightGripReleased.Invoke();
            }
        }
    }
}
