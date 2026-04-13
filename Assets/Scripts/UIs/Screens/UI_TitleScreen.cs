using UnityEngine;
using UnityEngine.UI;

public class UI_TitleScreen : UI_ScreenBase
{
    //[SerializeField] Button confirmButton;
    public override void Registration(UIManager manager)
    {
        base.Registration(manager);
        InputManager.OnOption -= OptionStart;
        InputManager.OnOption += OptionStart;
        //confirmButton.onClick.AddListener(Confirm);
    }

    public override void Unregistration(UIManager manager)
    {
        base.Unregistration(manager);
        InputManager.OnOption -= OptionStart;
        //confirmButton.onClick.RemoveListener(Confirm);
    }

    //public void Confirm()
    //{
    //    UIManager.ClaimToggleUI(UIType.Option);
    //    UIManager.ClaimToggleUI(UIType.Title);
    //}
    void OptionStart(bool value)
    {
        Debug.Log("Dsa");
        UIManager.ClaimToggleUI(UIType.Option);
    }
}
