using System;
using System.IO;
using UnityEngine;

public class UI_IngameTimeWindow : OpenableUIBase
{
    [SerializeField] TMPro.TextMeshProUGUI roundTime;

    [SerializeField] int roundTimeLimit = 3;

    int currentRound = 0;

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
        currentRound++;
        GameManager.OnUpdateObject -= TimeUpdate;
    }

    public void TimeUpdate(float deltaTime)
    {
        if (roundTime is null) return;

        currentTime = Time.time - startTime;
        int minutes = (int)(currentTime / 60f);
        int seconds = (int)(currentTime % 60f);
        if (seconds > roundTimeLimit && !roundEnd)
        {
            roundEnd = true;
        }
        if (!roundEnd)
            TimeSet(minutes, seconds);
        else
        {
            RoundEnd();
        }
        // 
    }

    public void TimeSet(int min, int sec)
    {
        roundTime.SetText($"{min}:{sec:00}");
    }

    private void RoundEnd()
    {
        if (currentRound < 3)
        {
            UIManager.ClaimOpenScreen(UIType.ChooseChaser);
        }
        else
        {
            UIManager.ClaimOpenScreen(UIType.InGameEnd, ScreenChangeType.SlideChanger);
            currentRound = 0;
        }
        Debug.Log($"라운드 {currentRound}");
        roundEnd = false;
        UIManager.ClaimCloseUI(UIType.Matchmaking);
    }
}
