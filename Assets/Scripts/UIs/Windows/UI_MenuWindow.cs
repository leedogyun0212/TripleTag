using UnityEngine;

public class UI_MenuWindow : OpenableUIBase
{
    public void OpenSetting()
    {
        UIManager.ClaimToggleUI(UIType.Option);
        UIManager.ClaimCloseUI(UIType.Menu);
    }
}
