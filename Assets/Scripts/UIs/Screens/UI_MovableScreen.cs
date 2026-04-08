using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_MovableScreen : UI_ScreenBase
{
    [SerializeField] List<UIBase> popupList = new();
    Vector3 popupPosition = Vector3.zero;
    Vector3 popupShift = new(20.0f, -20.0f);

    UI_DraggableWindow currentDragTarget = null;

    public override void Registration(UIManager manager)
    {
        base.Registration(manager);
        InputManager.OnDash += (value) => UIManager.ClaimToggleUI(UIType.Profile);
        InputManager.OnMouseMove -= MouseMove;
        InputManager.OnMouseMove += MouseMove;
        InputManager.OnMouseLeftButton -= MouseLeft;
        InputManager.OnMouseLeftButton += MouseLeft;
        UIManager.OnpopUp -= PopUp;
        UIManager.OnpopUp += PopUp;
    }

    public override void Unregistration(UIManager manager)
    {
        base.Unregistration(manager);
        InputManager.OnMouseMove -= MouseMove;
        InputManager.OnMouseLeftButton -= MouseLeft;
        UIManager.OnpopUp -= PopUp;
    }

    protected override GameObject OnSetChild(GameObject newChild)
    {
        //새로운 자식한테 UIManager한테 가서 등록 받아오라고 시킬 것!
        UIManager.ClaimSetUI(newChild);

        if(newChild)
        {
            UI_DraggableWindow asDraggable = newChild.GetComponentInChildren<UI_DraggableWindow>();
            if(asDraggable)
            {
                asDraggable.OnDragStart -= SetDragTarget;
                asDraggable.OnDragStart += SetDragTarget;
            }
        }

        return base.OnSetChild(newChild);
    }

    protected override void OnUnsetChild(GameObject oldChild)
    {
        UIManager.ClaimUnsetUI(oldChild);

        if (oldChild)
        {
            UI_DraggableWindow asDraggable = oldChild.GetComponentInChildren<UI_DraggableWindow>();
            if (asDraggable)
            {
                asDraggable.OnDragStart -= SetDragTarget;
            }
        }

        base.OnUnsetChild(oldChild);
    }

    void SetDragTarget(UI_DraggableWindow dragTarget, Vector2 startPosition)
    {
        currentDragTarget = dragTarget;
        if (currentDragTarget)
        {
            currentDragTarget.SetMouseStartPosition(startPosition);
        }
    }
    private void MouseLeft(bool value, Vector2 screenPosition, Vector3 worldPosition)
    {
        if(!value) currentDragTarget = null;
    }

    void MouseMove(Vector2 screenPosition, Vector3 worldPosition)
    {
        if(currentDragTarget) //지금 움직여야하는 친구한테
        {
            //움직이라고 이야기 하기!
            currentDragTarget.SetMouseCurrentPosition(screenPosition);
        }
    }

    void PopUp(string title, string context, string confirm)
    {
        GameObject newChild = SetChild(ObjectManager.CreateObject("PopUp"));
        if (newChild)
        {
            newChild.transform.localPosition = GetNextPopupPosition();

            if (newChild.TryGetComponent(out UIBase newUI))
            {
                //팝업창에 합류
                //대신 원래 네가 여기 없었다면 => 하나의 팝업인데 두번의 리스트에 들어가면?
                //일단 띠껍긴 함
                if (!popupList.Contains(newUI)) popupList.Add(newUI);
            }
            //이 친구가 시스템 메시지를 받을 수 있는 걸까?
            //ISystemMessagePossible인지 체크를 하고
            //메세지를 보내주기만 하면 끝
            if (newChild.TryGetComponent(out ISystemMessagePossible target))
            {
                target.SetSystemMessage(title, context, confirm);
            }
            if(newChild.TryGetComponent(out IConfirmable confirmTarget))
            {
                confirmTarget.SetConfirmAction(() => // 팝업창을 누르면
                {
                    if(newUI) popupList.Remove(newUI);//너는 팝업도 아니고
                    UnsetChild(newChild);//자식에서 제외하고
                    ObjectManager.DestroyObject(newChild);//파괴한다
                });
            }

        }
    }

    public Vector3 GetNextPopupPosition()
    {
        //그러면 팝업 포지션은 어떻게 계산할까?
        //지금 가지고 있는 팝업리스트 중에서 가장 오른쪽 아래에 있는 녀석을 구하기
        //아무도 없으면? Vector3.zero
        Vector3 bestScore = Vector2.zero;

        if (popupList.Count == 0) return bestScore;

        foreach (UIBase currentPopup in popupList)
        {
            Vector3 currentScore = currentPopup.transform.localPosition;
            //x축 일등인지
            if (bestScore.x < currentScore.x) bestScore.x = currentScore.x;
            //y축 일등인지
            if (bestScore.y > currentScore.y) bestScore.y = currentScore.y;
        }

        return bestScore + popupShift;
    }

}
