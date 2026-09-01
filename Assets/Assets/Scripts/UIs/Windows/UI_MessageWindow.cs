using UnityEngine;

public class UI_MessageWindow : OpenableUIBase
{
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
        UIManager.ClaimCloseUI(UIType.Message);
    }
}
