using UnityEngine;

public class UI_ChooseChaserScreen : UI_ScreenBase
{
    [SerializeField] TMPro.TextMeshProUGUI ChaserChooseText;

    [SerializeField] GameObject ChaserChoose;

    [SerializeField] int MatchTimeLimit = 10;

    float startTime;
    float currentTime;

    int RandomNum;

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
        RandomNum = Random.Range(1, 4);
        GameManager.OnUpdateObject -= TimeUpdate;
        GameManager.OnUpdateObject += TimeUpdate;
        InputManager.OnCancel -= MatchExit;
        InputManager.OnCancel += MatchExit;
    }



    public void OnDisable()
    {
        GameManager.OnUpdateObject -= TimeUpdate;
        InputManager.OnCancel -= MatchExit;
        ChaserChoose.SetActive(false);
    }

    public void TimeUpdate(float deltaTime)
    {
        if (ChaserChooseText is null) return;

        currentTime = Time.time - startTime;
        int minutes = (int)(currentTime / 60f);
        int seconds = (int)(currentTime % 60f);
        if (seconds > 3 && seconds < MatchTimeLimit)
        {
            TimeSet();
        }   
        else if (seconds > MatchTimeLimit)
        {
            GameStart();
        }


    }

    public void TimeSet()
    {
        ChaserChooseText.SetText($"이번 술래는 Group{RandomNum} 입니다");
        ChaserChoose.SetActive(true);
    }

    public void GameStart()
    {
        UIManager.ClaimOpenScreen(UIType.InGame, ScreenChangeType.SlideChanger);

        UIManager.ClaimCloseUI(UIType.ChooseChaser);
    }

    private void MatchExit(bool value)
    {
        UIManager.ClaimCloseUI(UIType.ChooseChaser);
    }
}
