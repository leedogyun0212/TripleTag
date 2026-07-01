using System.IO;
using UnityEngine;

public class UI_IngameTimeWindow : OpenableUIBase
{
    [SerializeField] TMPro.TextMeshProUGUI roundTime;

    [SerializeField] int roundTimeLimit = 3;

    float startTime;
    float currentTime;

    bool roundEnd = false;

    private void OnEnable()
    {
        startTime = Time.time;
        GameManager.OnUpdateObject -= TimeUpdate;
        GameManager.OnUpdateObject += TimeUpdate;
    }



    public void OnDisable()
    {
        GameManager.OnUpdateObject -= TimeUpdate;
    }

    public void TimeUpdate(float deltaTime)
    {
        if (roundTime is null) return;

        currentTime = Time.time - startTime;
        int minutes = (int)(currentTime / 60f);
        int seconds = (int)(currentTime % 60f);
        Debug.Log($"{seconds:00}" + roundEnd);
        if (minutes > roundTimeLimit && !roundEnd)
        {
            roundEnd = true;
        }
        if (!roundEnd)
            TimeSet(minutes, seconds);
        else
        {
            
        }


    }

    public void TimeSet(int min, int sec)
    {
        roundTime.SetText($"{min}:{sec:00}");
    }
}
