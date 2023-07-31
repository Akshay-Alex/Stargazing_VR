using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class GestureDetection : MonoBehaviour
{
    public UnityEvent LeftGripPressed, LeftGripReleased, RightGripPressed,RightGripReleased;
    public Transform leftControllerReference, rightControllerReference, cameraReference;
    InputFeatureUsage<bool> gripPressFeature = CommonUsages.gripButton;
    InputDeviceCharacteristics leftControllerCharacterisitic;
    InputDeviceCharacteristics rightControllerCharacterisitic;
    List<InputDevice> leftControllers;
    List<InputDevice> rightControllers;
    bool isLeftGripPressed, isRightGripPressed, leftGripOutValue, rightGripOutValue,isLeftController,buttonHeldDown;
    public static GestureDetection gestureDetection;
    Vector3 leftRightAxis, startPosition, endPosition,controllerMovementVector, currentControllerMovementVector;
    public LineRenderer ControllerAxisLine;
    double timeInSeconds, deltaValue;
    DateTime startTime;
    public bool _debug;
    // Start is called before the first frame update
    void Start()
    {
        gestureDetection = this;
        isLeftController = false;
        buttonHeldDown = false;
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
        isLeftController = true;
        buttonHeldDown = true;
        startPosition = leftControllerReference.position;
        startTime = DateTime.Now;
    }
    void OnRightGripPressed()
    {
        isLeftController = false;
        buttonHeldDown = true;
        startPosition = rightControllerReference.position;
        startTime = DateTime.Now;
    }
    void OnLeftGripReleased()
    {
        if(isLeftController)
        {
            buttonHeldDown = false;
        }
    }
    void OnRightGripReleased()
    {
        if (!isLeftController)
        {
            buttonHeldDown = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        CheckLeftGrip();
        CheckRightGrip();
    }
    private void FixedUpdate()
    {
        if (buttonHeldDown)
        {
            leftRightAxis = Vector3.Cross(Vector3.up, cameraReference.forward);
            if (isLeftController)
            {
                endPosition = leftControllerReference.position;
                //currentControllerMovementVector = startPosition - leftControllerReference.position;             
            }
            else
            {
                endPosition = rightControllerReference.position;
                //currentControllerMovementVector = startPosition - rightControllerReference.position;
            }
            currentControllerMovementVector = startPosition - endPosition;
            timeInSeconds = (startTime - DateTime.Now).TotalSeconds;
            deltaValue = Vector3.Dot(currentControllerMovementVector, leftRightAxis) * (1/timeInSeconds);
            if (_debug)
            {
                Debug.Log("Delta Value : " + deltaValue);
                ControllerAxisLine.SetPosition(0, startPosition);
                ControllerAxisLine.SetPosition(1, endPosition);
            }
        }
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
