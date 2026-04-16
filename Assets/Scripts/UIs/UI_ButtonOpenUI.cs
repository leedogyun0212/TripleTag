using System.Collections;
using UnityEngine;

public class UI_ButtonOpenUI : MonoBehaviour
{
    [SerializeField] UIType wantType;
    [SerializeField] bool wantToggle;

    public void OpenScreen()
    {
        if (wantToggle) UIManager.ClaimOpenScreen(wantType,ScreenChangeType.ScreenChanger);
        else UIManager.ClaimOpenUI(wantType);
    }

    public void Open()
    {
        if (wantToggle) UIManager.ClaimToggleUI(wantType);
        else UIManager.ClaimOpenUI(wantType);
    }


    public void Close()
    {
        UIManager.ClaimCloseUI(wantType);
    }
}
