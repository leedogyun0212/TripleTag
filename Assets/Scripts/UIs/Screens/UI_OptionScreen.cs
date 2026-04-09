using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_OptionScreen : UI_ScreenBase
{
    [SerializeField] Button confirmButton;
    //Action ConfirmAction;

    public override void Registration(UIManager manager)
    {
        base.Registration(manager);
        confirmButton.onClick.AddListener(Confirm);
        InputManager.OnOption -= OptionStart;
        InputManager.OnOption += OptionStart;
    }

    public override void Unregistration(UIManager manager)
    {
        base.Unregistration(manager);
        confirmButton.onClick.RemoveListener(Confirm);
        InputManager.OnOption -= OptionStart;
        //ConfirmAction = null;
    }

    protected override GameObject OnSetChild(GameObject newChild)
    {
        UIManager.ClaimSetUI(newChild);

        if (newChild)
        {
            UI_OptionMusic asOptionMusic = newChild.GetComponentInChildren<UI_OptionMusic>();
            if (asOptionMusic)
            {

            }
        }

        return base.OnSetChild(newChild);
    }

    public void Confirm()
    {
        UIManager.ClaimOpenScreen(UIType.Title);
    }
    public void OptionStart(bool value)
    {
        //UIManager.ClaimToggleUI(UIType.Title);
    }
}
