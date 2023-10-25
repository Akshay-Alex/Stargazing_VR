using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class GestureDetection : MonoBehaviour
{
    public UnityEvent LeftGripPressed, LeftGripReleased, RightGripPressed,RightGripReleased,LeftSecondaryButtonPressed,RightSecondaryButtonPressed,GripReleasedAfterBeingHeldDown;
    public Transform leftControllerReference, rightControllerReference, cameraReference;
    InputFeatureUsage<bool> gripPressFeature = CommonUsages.gripButton;
    InputFeatureUsage<bool> secondaryButtonFeature = CommonUsages.secondaryButton;
    InputDeviceCharacteristics leftControllerCharacterisitic;
    InputDeviceCharacteristics rightControllerCharacterisitic;
    List<InputDevice> leftControllers;
    List<InputDevice> rightControllers;
    bool isLeftGripPressed, isRightGripPressed, leftGripOutValue, rightGripOutValue,isLeftController,buttonHeldDown,leftSecondaryOutValue,rightSecondaryOutValue;
    public static GestureDetection gestureDetection;
    Vector3 leftRightAxis, startPosition, endPosition,controllerMovementVector, currentControllerMovementVector;
    double timeInSeconds;
    public double deltaValue;
    DateTime startTime;
    float lastLeftSecondaryPressTime, lastRightSecondaryPressTime;
    public float debounceTime;
    public bool _debug;
    public bool _enableVfX;
    public ParticleSystem LeftSparkParticle, rightSparkParticle;
    // Start is called before the first frame update
    void Start()
    {
        gestureDetection = this;
        isLeftController = false;
        buttonHeldDown = false;
        _enableVfX = false;
        leftControllerCharacterisitic = InputDeviceCharacteristics.Left;
        rightControllerCharacterisitic = InputDeviceCharacteristics.Right;
        leftControllers = new List<InputDevice>();
        rightControllers = new List<InputDevice>();
        SetDebounceTime();
        SubscribeToGripEvents();
    }
    void PlayLeftSpark()
    {
        if (_enableVfX)
        {
            SFXSoundsManager.sFXSoundsManager.PlayTimeSkipSFX();
            LeftSparkParticle.Play();
        }
    }
    void PlayRightSpark()
    {
        if (_enableVfX)
        {
            SFXSoundsManager.sFXSoundsManager.PlayTimeSkipSFX();
            rightSparkParticle.Play();
        }
    }
    void SetDebounceTime()
    {
        lastLeftSecondaryPressTime = Time.time;
        lastRightSecondaryPressTime = Time.time;
    }
    bool CheckIfButtonPressValid(bool isLeft)
    {
        if(isLeft)
        {
            if (Time.time - lastLeftSecondaryPressTime > debounceTime)
            {
                lastLeftSecondaryPressTime = Time.time;
                return true;
            }
        }
        else
        {
            if (Time.time - lastRightSecondaryPressTime > debounceTime)
            {
                lastRightSecondaryPressTime = Time.time;
                return true;
            }        
        }
        return false;
    }
    void SubscribeToGripEvents()
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
            PlayLeftSpark();
            GripReleasedAfterBeingHeldDown.Invoke();
        }
    }
    void OnRightGripReleased()
    {
        if (!isLeftController)
        {
            buttonHeldDown = false;
            PlayRightSpark();
            GripReleasedAfterBeingHeldDown.Invoke();
        }
    }
    // Update is called once per frame
    void Update()
    {
        CheckLeftGrip();
        CheckRightGrip();
        CheckSecondaryButtonPress();
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
    void CheckSecondaryButtonPress()
    {
        InputDevices.GetDevicesWithCharacteristics(leftControllerCharacterisitic, leftControllers);
        InputDevices.GetDevicesWithCharacteristics(rightControllerCharacterisitic, rightControllers);
        for (int index = 0; index < rightControllers.Count; index++)
        {
            rightControllers[index].TryGetFeatureValue(secondaryButtonFeature, out rightSecondaryOutValue);
            if (rightSecondaryOutValue == true && CheckIfButtonPressValid(false))
            {
                RightSecondaryButtonPressed.Invoke();
                if(_debug)
                {
                    Debug.Log("Right secondary button pressed");
                }
            }
        }
        for (int index = 0; index < leftControllers.Count; index++)
        {
            leftControllers[index].TryGetFeatureValue(secondaryButtonFeature, out leftSecondaryOutValue);
            if (leftSecondaryOutValue == true && CheckIfButtonPressValid(true))
            {
                LeftSecondaryButtonPressed.Invoke();
                if (_debug)
                {
                    Debug.Log("Left secondary button pressed");
                }
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
