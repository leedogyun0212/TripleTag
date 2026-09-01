using UnityEngine;

public class UI_GameStartWindow : OpenableUIBase
{
    [SerializeField] GameObject TimePrefab;


    public void OnEnable()
    {
        InputManager.OnCancel -= Exit;
        InputManager.OnCancel += Exit;
    }

    public void OnDisable()
    {
        InputManager.OnCancel -= Exit;
    }

    private void Exit(bool value)
    {
        UIManager.ClaimCloseUI(UIType.GameStart);
    }

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
