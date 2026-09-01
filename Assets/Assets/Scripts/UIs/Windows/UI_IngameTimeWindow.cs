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
            if (currentRound == 0) return;
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
        if (currentRound < 3 && currentRound >0)
        {
            UIManager.ClaimOpenScreen(UIType.ChooseChaser, ScreenChangeType.ScreenChanger);
        }
        else
        {
            UIManager.ClaimOpenScreen(UIType.InGameEnd, ScreenChangeType.SlideChanger);
            currentRound = 0;
        }
        Debug.Log($"라운드 {currentRound}");
        roundEnd = false;
        //캐릭터의 움직임,공격,공격시 데미지 입는다,시야,라운드당 시간,
        //부활,트랩,플레이어별 점수 상승,멀티플레이,다듬기
        //     7월                    /  8월     / 9월
    }
}
