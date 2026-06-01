using System.IO;
using UnityEngine;

public class UI_IngameTimeWimdow : OpenableUIBase
{
    [SerializeField] TMPro.TextMeshProUGUI matchTime;

    [SerializeField] int MatchTimeLimit = 10;

    float startTime;
    float currentTime;

    bool matchOn = false;

    public void TimeUpdate(float deltaTime)
    {
        if (matchTime is null) return;

        currentTime = Time.time - startTime;
        int minutes = (int)(currentTime / 60f);
        int seconds = (int)(currentTime % 60f);
        Debug.Log($"{seconds:00}" + matchOn);
        if (seconds > MatchTimeLimit && !matchOn)
        {
            matchOn = true;
        }
        if (!matchOn)
            TimeSet(minutes, seconds);
        else
        {
            
        }


    }

    public void TimeSet(int min, int sec)
    {
        matchTime.SetText($"{min}:{sec:00}");
    }
}
