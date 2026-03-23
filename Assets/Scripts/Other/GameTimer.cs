using UnityEngine;
using TMPro;
using System;

public class GameTimer : MonoBehaviour
{
    [SerializeField] 
    private float timerDuration = 120f; 
    [SerializeField] 
    private TextMeshProUGUI timerText;
    public static event Action TimerFinished;

    private float timeRemaining;
    private bool finished = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeRemaining = timerDuration;
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (finished == true)
        {
            return;
        }

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            finished = true;
            TimerFinished();
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    // getetr functions
    public float GetTimeRemaining()
    {
        return timeRemaining;
    }

    public float GetTimerDuration()
    {
        return timerDuration;
    }

    // returns % of time remaining
    public float GetTimeRemainingPercent()
    {
        return (timeRemaining / timerDuration) * 100f;
    }

}
