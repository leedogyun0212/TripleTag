using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovementModule : CharacterModule, IRunnable
{
    protected Vector3? targetDirection = null;
    protected Vector3? targetDestination = null;
    protected float targetTolerance;

    protected float Speed = 5.0f;
    protected float rotationSpeed = 5.0f;

    protected MoveType moveType = MoveType.walk;

    public bool isCooltime = false;
    public bool isGround = true;
    protected bool isJump = false;

    [SerializeField] float duration = 1.2f;
    [SerializeField] float cooldown = 8.0f;

    [SerializeField] Transform FootLeftTrans;

    protected float SaveYDir;

    bool _inputJumpPressed = false;

    [SerializeField] float minRotationMoveSqr = 0.001f; // 회전 판단을 위한 최소 이동 제곱거리

    //이런 거대한모듈을 만들 때에 한번 "대분류"로 분류하기
    //자식에서 더 이상 못 바꾸게!
    public sealed override System.Type RegistrationType => typeof(MovementModule);

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);
        GameManager.OnPhysicCharacter -= MovementUpdate;
        GameManager.OnPhysicCharacter += MovementUpdate;
    }


    public override void OnUnregistration(CharacterBase oldOwner)
    {
        base.OnUnregistration(oldOwner);
        GameManager.OnPhysicCharacter -= MovementUpdate;
    }

    public void MovementUpdate(float deltaTime)
    {
        Vector3 originPosition = transform.position;                 //이동하기전에 제 위치를 저장
        
        PhysicUpdate(deltaTime);                                     //물리 업데이트
        Vector3 positionDelta = transform.position - originPosition; //이동한 위치의 차이를 계산
        if (targetDirection.HasValue && targetDirection.Value != Vector3.zero)
        {
            UpdateToRotation(positionDelta, deltaTime);
        }
        Owner.MovementNotify(positionDelta);                              //이동한 양에 따라서 애니메이션을 업데이트
        
    }

    /// <summary> 입력 키에 따라 시선 방향으로 몸이 회전   </summary>
    public void UpdateToRotation(Vector3 delta, float deltaTime)
    {
        Vector3 lookForward;
        if ( targetDirection.Value.sqrMagnitude >= minRotationMoveSqr)
        {
            lookForward = new Vector3(targetDirection.Value.x, 0f, targetDirection.Value.z);
        }
        else
            return;

        if (lookForward.sqrMagnitude < 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(lookForward);

        float maxDegrees = rotationSpeed * Mathf.Max(deltaTime, 0.0001f);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, maxDegrees);
    }


    public void PhysicUpdate(float deltaTime)
    {
        UpdateToDirection(deltaTime);
        UpdateToDestination(deltaTime);
    }

    public virtual float GetMoveSpeed() => Speed;
    public virtual float GetMoveSpeed(float deltaTime) => GetMoveSpeed() * deltaTime;

    public void Translate(Vector3 delta)
    {
        transform.position += delta;
    }

    public void UpdateToDirection(float deltaTime)
    {
        JumpForce();
        if (targetDirection is null) return;

        float currentMoveSpeed = GetMoveSpeed(deltaTime);
        Translate(currentMoveSpeed * targetDirection.Value);
        //포톤에서 이동 관련 문제(적합성??) => 포톤에 접속을 하면 렉걸린 것 처럼 움직임
        GameManager.Camera.CameraMove(GetMoveSpeed(deltaTime) * targetDirection.Value, Owner.Head.position);
    }
    public void UpdateToDestination(float deltaTime)
    {
        //목적지가 없으면 리턴
        if (targetDestination is null) return;

        //여길 넘어서면 목적지가 있는 상태
        //

        //해당 위치로 조금씩 가는 법!
        //목적지 - 출발지
        Vector3 currentMoveDirection = (targetDestination.Value - transform.position);

        //일단 얼마나 더 가야 해요?
        float distance = currentMoveDirection.magnitude;
        //   거리가       인정범위 밖
        if (distance > targetTolerance)
        {
            //방향을 잡아봅시다
            currentMoveDirection.Normalize();

            float currentMoveSpeed = GetMoveSpeed(deltaTime);

            //지금 이 프레임에 나는 몇m를 갈 수 있을까?
            //거리 : 속력 * 시간
            float resultMoveSpeed = Mathf.Min(currentMoveSpeed, distance);

            Translate(resultMoveSpeed * currentMoveDirection);
        }
    }

    public void MoveToDestination(Vector3 destination, float tolerance)
    {
        targetDirection = null;//방향으로는 움직이지 안겠다!
        targetDestination = destination;
        targetTolerance = tolerance;
    }

    public void MoveToDirection(Vector3 direction)
    {

        targetDestination = null; //목적지를 제거한다
        targetDirection = direction.normalized;
        if (direction.normalized.y > 0.0f)
        {
            SaveYDir = direction.normalized.y;
            _inputJumpPressed = true;
        }
    }

    public void StopMovement()
    {
        targetDestination = null; //목적지를 제거한다
        targetDirection = null;// 방향으로는 움직이지 않겠다!
        _inputJumpPressed = false;
    }

    //
    /// <summary> 키를 누를시 실행하는 기능  </summary>
    public void ChangeMoveType(MoveType wantType)
    {
        if (isCooltime) return;

        StartCoroutine(ChangeMoveStateRoutine(wantType));
    }

    /// <summary> 유지시간동안 MoveType을 변경해주고 이후 원래대로 돌아온 후 쿨타임 동안 사용 불가   </summary>
    private IEnumerator ChangeMoveStateRoutine(MoveType wantType)
    {
        isCooltime = true;

        MoveType originalType = moveType;

        moveType = wantType;

        yield return new WaitForSeconds(duration);

        moveType = originalType;
        Debug.Log("쿨타임");
        yield return new WaitForSeconds(cooldown);
        isCooltime = false;
    }

    /// <summary> 캐릭터가 땅에 닿았는지 체크  </summary>
    public bool GroundCheck(string LayerName = "Ground")
    {
        int layerMask = 1 << LayerMask.NameToLayer(LayerName);
        isGround = Physics.Raycast(FootLeftTrans.position, Vector3.down, 0.3f, layerMask);
        return isGround;
    }                                                                                                                                                                                                                                                                              

    /// <summary> 점프하는 동안에는 힘 유지 </summary>
    public void JumpForce()
    {
        // 1. 현재 바닥 상태 확인
        GroundCheck();

        if (!targetDirection.HasValue) return;

        Vector3 temp = targetDirection.Value;
        
        if (!isGround && _inputJumpPressed)
        {
            isJump = false;
            if (targetDirection.Value.y < SaveYDir)
            {
                temp.y = SaveYDir;
                targetDirection = temp;
            }
        }
        else if (!isJump && isGround)
        {
            temp.y = 0.0f;
            targetDirection = temp;
            _inputJumpPressed = false;
            isJump = true;
        }
    }
}
