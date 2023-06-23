using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TimerEvent();
public class Timer : MonoBehaviour
{
    [SerializeField]
    private float timeRemaining;
    public event TimerEvent TimerFinished;
    //public float duration;
    public bool timerRunning;
    public void SetTimer(float duration)
    {
        //Debug.Log("Timer set");
        timeRemaining = duration;
        timerRunning = true;
    }
    public void StopTimer()
    {
        timerRunning = false;
    }


    void FixedUpdate()
    {
        if(timerRunning)
        {
            if(timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                timerRunning = false;
                TimerFinished.Invoke();
                //Debug.Log("Timer event should be fired now");
            }
            
        }
    }
}
