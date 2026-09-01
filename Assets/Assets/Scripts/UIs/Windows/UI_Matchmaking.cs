using System;
using UnityEngine;

public class UI_Matchmaking : OpenableUIBase
{
    [SerializeField] TMPro.TextMeshProUGUI matchTime;

    [SerializeField] int MatchTimeLimit = 10;

    float startTime;
    float currentTime;

    bool matchOn = false;

    public override void Registration(UIManager manager)
    {
        base.Registration(manager);
        startTime = Time.time;
        GameManager.OnUpdateObject -= TimeUpdate;
        GameManager.OnUpdateObject += TimeUpdate;
    }

    public override void Unregistration(UIManager manager)
    {
        base.Unregistration(manager);
        GameManager.OnUpdateObject -= TimeUpdate;
    }

    private void OnEnable()
    {
        startTime = Time.time;
        GameManager.OnUpdateObject -= TimeUpdate;
        GameManager.OnUpdateObject += TimeUpdate;
        InputManager.OnCancel -= MatchExit;
        InputManager.OnCancel += MatchExit;
    }

    

    public void OnDisable()
    {
        GameManager.OnUpdateObject -= TimeUpdate;
        InputManager.OnCancel -= MatchExit;
    }

    public void TimeUpdate(float deltaTime)
    {
        if (matchTime is null) return;

        currentTime = Time.time - startTime;
        int minutes = (int)(currentTime / 60f);
        int seconds = (int)(currentTime % 60f);
        
        if (seconds > MatchTimeLimit && !matchOn)
        {
            matchOn = true;
        }
        if (!matchOn)
            TimeSet(minutes, seconds);
        else
        {
            GameStart();
        }

        
    }

    public void TimeSet(int min, int sec)
    {
        matchTime.SetText($"{min}:{sec:00}");
    }

    public void GameStart()
    {
        UIManager.ClaimOpenScreen(UIType.ChooseChaser, ScreenChangeType.SlideChanger);
        matchOn = false;
        UIManager.ClaimCloseUI(UIType.Matchmaking);
    }
    
    private void MatchExit(bool value)
    {
        UIManager.ClaimCloseUI(UIType.Matchmaking);
    }
}
