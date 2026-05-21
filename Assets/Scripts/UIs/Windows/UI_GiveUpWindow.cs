using UnityEngine;

public class UI_GiveUpWindow : OpenableUIBase
{
    public void Confirm()
    {
        UIManager.ClaimOpenScreen(UIType.InGameEnd);
        UIManager.ClaimCloseUI(UIType.GiveUp);
        UIManager.ClaimCloseUI(UIType.Menu);
    }

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
        UIManager.ClaimCloseUI(UIType.GiveUp);
    }

    public void Cancel() => UIManager.ClaimCloseUI(UIType.GiveUp);
}
