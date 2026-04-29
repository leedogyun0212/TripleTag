using System;
using UnityEngine;

public class PlayerController : ControllerBase
{
    [SerializeField] MoveType runType = MoveType.Run;

    [SerializeField] private float duration = 1.2f;
    [SerializeField] private float cooldown = 8.0f;

    //빙의가 되면 내 캐릭터 생긴 거니까 키 입력
    //시작!
    protected override void OnPossess(CharacterBase newCharacter)
    {
        base.OnPossess(newCharacter);
        InputManager.OnMouseRightButton -= MoveToMousePosition;
        InputManager.OnMouseRightButton += MoveToMousePosition;
        InputManager.OnMove -= MoveToDirection;
        InputManager.OnMove += MoveToDirection;
        InputManager.OnRun  -= MoveToRunning;
        InputManager.OnRun  += MoveToRunning;
    }

    //해제가 되면 내 캐릭터 뺏긴 거니까 키 입력 받을 필요가 없음!
    protected override void OnUnpossess(CharacterBase oldCharacter)
    {
        base.OnUnpossess(oldCharacter);
        InputManager.OnMouseRightButton -= MoveToMousePosition;
        InputManager.OnMove -= MoveToDirection;
        InputManager.OnRun  -= MoveToRunning;
    }

    private void MoveToMousePosition(bool value, Vector2 screenPosition, Vector3 worldPosition)
    {
        if (value) CommandMoveToDestination(worldPosition, 0.0f);
    }

    private void MoveToRunning(bool value)
    {
        CommandChangeMoveType(runType, duration, cooldown);
    }

    private void MoveToDirection(Vector3 value)
    {
        CommandMoveToDirection(value);
    }
}
