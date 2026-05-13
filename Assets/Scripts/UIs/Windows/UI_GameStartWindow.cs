using UnityEngine;

public class UI_GameStartWindow : OpenableUIBase
{
    [SerializeField] GameObject TimePrefab;

    public void Ranking()
    {
        MatachmakingStart();
        UIManager.ClaimCloseUI(UIType.GameStart);
    }

    public void MatachmakingStart()
    {
        UIBase result = UIManager.ClaimGetUI(UIType.Matchmaking);

        if (!result) result = UIManager.ClaimCreateUI(UIType.Matchmaking, TimePrefab.name);

        if (result is IOpenable openTarget)
        {
            openTarget.Open();
        }
    }
}
