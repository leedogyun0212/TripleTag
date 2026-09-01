using Unity.VisualScripting;
using UnityEngine;

public class RunModule : MovementModule
{
    /// <summary>  추가 스피드 </summary>
    [SerializeField] float RunSpeed = 1.00f;
    float SaveSpeed = 0f;
    bool PlusSpeed;

    /// <summary>  달리기 시 추가로 얻는 스피드 </summary>
    public float GetRunMove()
    {
        if (moveType is not MoveType.Run)
        {
            PlusSpeed= true;
            if(SaveSpeed is not 0f) RunSpeed = SaveSpeed;
            return RunSpeed;
        }

        if (Owner.CharType is CharacterType.Runner && PlusSpeed)
        {
            PlusSpeed = false;
            SaveSpeed = RunSpeed;
            return RunSpeed *= 1.25f;
        }
        else if (Owner.CharType is CharacterType.Chaser && PlusSpeed)
        {
            PlusSpeed = false;
            SaveSpeed = RunSpeed;
            return RunSpeed *= 1.13f;
        }
        else if (!PlusSpeed) return RunSpeed;
        
        return RunSpeed;
    }

    //스피드 적용
    public override float GetMoveSpeed()
    {
        RunSpeed = GetRunMove();
        if (moveType is MoveType.Run)
        {
            return Speed * RunSpeed;
        }
        else return Speed * RunSpeed;
    }

    //원래 생각 했던 것이 시간초가 짧고 속도 증가도 그리 크지 않다.

}

