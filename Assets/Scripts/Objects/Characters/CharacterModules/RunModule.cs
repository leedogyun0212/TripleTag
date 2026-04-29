using Unity.VisualScripting;
using UnityEngine;

public class RunModule : MovementModule
{
    [SerializeField] float RunSpeed = 1.00f;
    float SaveSpeed = 0f;
    bool PlusSpeed;

    //달리기 시 추가로 얻는 스피드
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
        Debug.Log(Speed+$"{Owner.CharType}");
        Debug.Log(Speed * RunSpeed);
        if (moveType is MoveType.Run)
        {
            return Speed * RunSpeed;
        }
        else return Speed * RunSpeed;
    }

    //원래 생각 했던 것이 시간초가 짧고 속도 증가도 그리 크지 않다.

    //방향에 맞춰 회전
    public void MoveToRotation(Vector3 rotation)
    {

    }

    public void Jump()
    {

    }
}

