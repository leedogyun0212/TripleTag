using System;
using UnityEngine;

[Serializable]
public struct UIClaim
{
    public string prefabName;
    public UIType uiType;
    public bool isOpen;

    public UIBase Execute()
    {
        //UI내놓아! => 예외가 있을 수 있음!
        //            이미...있는데?
        UIBase result = UIManager.ClaimGetUI(uiType);
        
        if(!result) result = UIManager.ClaimCreateUI(uiType, prefabName);
        
        if (!result) return result;

        if (result is IOpenable openTarget)
        {
            if (isOpen) openTarget.Open();
            else openTarget.Close();
        }

        return result;
    }
}

public class UI_ScreenBase : UIBase
{
    [SerializeField] UIClaim[] requiredUI;

    public override void Registration(UIManager manager)
    {
        base.Registration(manager);
        foreach (UIClaim currentClaim in requiredUI)
        {
            currentClaim.Execute();
        }
    }
}
