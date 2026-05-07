using System;
using UnityEngine;

public class UI_TargetHoverInfo : OpenableUIBase
{
    [SerializeField] Vector2 shiftedPosition;

    [SerializeField] TMPro.TextMeshProUGUI RunCoolTime;
    [SerializeField] TMPro.TextMeshProUGUI TrapCoolTime;
    [SerializeField] UnityEngine.UI.Slider CoolTime;

    CharacterBase target;

    public override void Registration(UIManager manager)
    {
        base.Registration(manager);
        InputManager.OnMouseHover -= HoverInfoChange;
        InputManager.OnMouseHover += HoverInfoChange;
        //InputManager.OnMouseMove -= MoveToMouse;
        //InputManager.OnMouseMove += MoveToMouse;
    }

    private void Update()
    {
        if (target == null) return;
        transform.position = Camera.main.WorldToScreenPoint(target.transform.position) + (Vector3)shiftedPosition;
    }

    public override void Unregistration(UIManager manager)
    {
        base.Unregistration(manager);
        InputManager.OnMouseHover -= HoverInfoChange;
        //InputManager.OnMouseMove -= MoveToMouse;
    }

    private void HoverInfoChange(GameObject newTarget, GameObject oldTarget)
    {
        CharacterBase asCharacter = newTarget?.GetComponent<CharacterBase>();

        if (asCharacter)
        {
            if(asCharacter.GetModule<MovementModule>().isCooltime)
                RunCoolTime.SetText($"달리기 사용불가");
            else
                RunCoolTime.SetText($"달리기 사용가능");
            Open();
        }
        else Close();
        target = asCharacter;
    }

    private void MoveToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        transform.position = screenPosition + shiftedPosition;
    }
}
