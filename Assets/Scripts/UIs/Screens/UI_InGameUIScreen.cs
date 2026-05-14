using UnityEngine;

public class UI_InGameUIScreen : UI_ScreenBase
{
    private void OnEnable()
    {
        InputManager.OnOption -= OptionStart;
        InputManager.OnOption += OptionStart;

        InputManager.OnExit -= Exit;
        InputManager.OnExit += Exit;
    }

    private void OnDisable()
    {
        InputManager.OnOption -= OptionStart;
        InputManager.OnExit -= Exit;
    }

    void OptionStart(bool value)
    {
        UIManager.ClaimToggleUI(UIType.Option);
    }

    void Exit(bool value)
    {
        UIManager.ClaimOpenUI(UIType.GiveUp);
    }
}
