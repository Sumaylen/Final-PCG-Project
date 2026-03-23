using UnityEngine;
using System;

public class CATimer : MonoBehaviour
{
    public static event Action PlayerWon;

    void OnEnable()
    {
        GameTimer.TimerFinished += HandleTimerFinished;
    }

    void OnDisable()
    {
        GameTimer.TimerFinished -= HandleTimerFinished;
    }

    void HandleTimerFinished()
    {
        PlayerWon.Invoke();
    }
}
