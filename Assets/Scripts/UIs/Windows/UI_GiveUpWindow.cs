using UnityEngine;

public class UI_GiveUpWindow : OpenableUIBase
{
    public void Confirm()
    {
        UIManager.ClaimOpenScreen(UIType.Main);
        UIManager.ClaimCloseUI(UIType.GiveUp);
        UIManager.ClaimCloseUI(UIType.Menu);
    }


    public void Cancel() => UIManager.ClaimCloseUI(UIType.GiveUp);
}
