using System;
using UnityEngine;

public class UI_MovableScreen : UIBase
{
    Vector3 popupPosition = Vector3.zero;
    Vector3 popupShift = new(20.0f, -20.0f);

    public override void Registration(UIManager manager)
    {
        base.Registration(manager);
        UIManager.OnpopUp -= PopUp;
        UIManager.OnpopUp += PopUp;
    }

    public override void Unregistration(UIManager manager)
    {
        base.Unregistration(manager);
        UIManager.OnpopUp -= PopUp;
    }

    protected override GameObject OnSetChild(GameObject newChild)
    {
        //새로운 자식한테 UIManager한테 가서 등록 받아오라고 시킬 것!
        UIManager.ClaimSetUI(newChild);
        return base.OnSetChild(newChild);
    }

    protected override void OnUnsetChild(GameObject oldChild)
    {
        UIManager.ClaimUnsetUI(oldChild);
        base.OnUnsetChild(oldChild);
    }
    
    void PopUp(string title, string context, string confirm)
    {
        GameObject newChild = SetChild(ObjectManager.CreateObject("PopUp"));
        if (newChild)
        {
            //이 친구가 시스템 메시지를 받을 수 있는 걸까?
            //ISystemMessagePossible인지 체크를 하고
            //메세지를 보내주기만 하면 끝
            if (newChild.TryGetComponent(out ISystemMessagePossible target))
            {
                target.SetSystemMessage(title, context, confirm);
            }
            if(newChild.TryGetComponent(out IConfirmable confirmTarget))
            {
                confirmTarget.SetConfirmAction(() =>
                {
                    UnsetChild(newChild);//자식에서 제외하고
                    ObjectManager.DestroyObject(newChild);//파괴한다
                });
            }
            newChild.transform.localPosition = popupPosition;
            popupPosition += popupShift;
        }
    }
}
