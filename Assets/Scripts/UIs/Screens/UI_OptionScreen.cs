using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class UI_OptionScreen : UI_ScreenBase
{
    [SerializeField] RectTransform myTransform;

    public void OnEnable()
    {
        //myTransform.SetAsLastSibling();
        InputManager.OnCancel -= Exit;
        InputManager.OnCancel += Exit;
    }

    public void OnDisable()
    {
        InputManager.OnCancel -= Exit;
    }

    private void Exit(bool value)
    {
        UIManager.ClaimCloseUI(UIType.Option);
    }
}
