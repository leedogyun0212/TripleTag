using System;
using UnityEngine;

public class UI_Matchmaking : OpenableUIBase
{
    [SerializeField] TMPro.TextMeshProUGUI matchTime;

    [SerializeField] int MatchTimeLimit = 10;

    [SerializeField] private GameObject matchServerPrefab;

    private GameObject matchServerObject;

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

        matchServerObject = Instantiate(matchServerPrefab);

        MatchServer.OnMatchFound -= MatchFound;
        MatchServer.OnMatchFound += MatchFound;

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

        TimeSet(minutes, seconds);
    }

    public void TimeSet(int min, int sec)
    {
        matchTime.SetText($"{min}:{sec:00}");
    }

    public void MatchFound()
    {
        Debug.Log("[UI_Matchmaking] GameStart 호출!");

        UIManager.ClaimOpenScreen(UIType.ChooseChaser, ScreenChangeType.SlideChanger);

        Debug.Log("[UI_Matchmaking] ChooseChaser Open 요청 완료!");
        
        matchOn = false;
        UIManager.ClaimCloseUI(UIType.Matchmaking);
        
        Debug.Log("[UI_Matchmaking] Matchmaking Close 요청 완료!");
    }
    
    private void MatchExit(bool value)
    {
        UIManager.ClaimCloseUI(UIType.Matchmaking);
        Destroy(matchServerObject);
    }
}
