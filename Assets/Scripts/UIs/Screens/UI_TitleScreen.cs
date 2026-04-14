using UnityEngine;
using UnityEngine.UI;

public class UI_TitleScreen : UI_ScreenBase
{
    public void OnEnable()
    {
        InputManager.OnOption -= OptionStart;
        InputManager.OnOption += OptionStart;
    }

    public void OnDisable()
    {
        InputManager.OnOption -= OptionStart;
    }

    void OptionStart(bool value)
    {
        UIManager.ClaimOpenScreen(UIType.Option);
    }
}
