using UnityEngine;
using UnityEngine.UI;

public class UI_TitleScreen : UI_ScreenBase
{
    public void OnEnable()
    {
        InputManager.OnOption -= OptionStart;
        InputManager.OnOption += OptionStart;
        InputManager.OnExit -= GameExit;
        InputManager.OnExit += GameExit;
        InputManager.OnStart -= OpenStart;
        InputManager.OnStart += OpenStart;
    }

    public void OnDisable()
    {
        InputManager.OnOption -= OptionStart;
        InputManager.OnExit -= GameExit;
        InputManager.OnStart -= OpenStart;
    }

    void OptionStart(bool value)
    {
        UIManager.ClaimToggleUI(UIType.Option);
    }

    void GameExit(bool value)
    {
        UIManager.ClaimToggleUI(UIType.GameQuit);
    }

    void OpenStart(bool value)
    {
        UIManager.ClaimOpenScreen(UIType.Main, ScreenChangeType.SlideChanger);
        
    }
}
