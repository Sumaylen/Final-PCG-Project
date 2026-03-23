using UnityEngine;

public class BSPTimer : MonoBehaviour
{
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
    }
}
