using UnityEngine;

public class RunModule : MovementModule
{
    [SerializeField] float RunSpeed = 1.00f;
    float SaveSpeed;

    public float GetRunMove()
    {
        if (Owner.CharType is CharacterType.Runner)
            return RunSpeed * 1.25f;
        else if (Owner.CharType is CharacterType.Chaser)
            return RunSpeed * 1.13f;
        else return RunSpeed = 1.00f;
    }

    public override float GetMoveSpeed()
    {
        RunSpeed = GetRunMove();
        if (moveType is MoveType.Run)
            return GetMoveSpeed() * RunSpeed;
        else return GetMoveSpeed();
    }
}

