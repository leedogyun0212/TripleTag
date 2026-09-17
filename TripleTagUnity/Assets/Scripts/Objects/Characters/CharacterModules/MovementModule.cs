using System.Collections;
using UnityEngine;

public class MovementModule : CharacterModule, IRunnable
{
    protected Vector3? targetDirection = null;
    protected Vector3? targetDestination = null;
    protected float targetTolerance;

    protected float Speed = 8.0f; // 이동 속도
    protected float rotationSpeed = 5.0f; // 회전 속도

    protected MoveType moveType = MoveType.walk; // 현재 이동 상태

    public bool isCooltime = false; // 달리기 쿨타임 여부
    public bool isGround = true; // 땅에 닿았는지 여부
    protected bool isJump = false; // 점프 중인지 여부
    public bool IsjumpCheck() => isJump;

    [SerializeField] float duration = 1.2f;
    [SerializeField] float cooldown = 8.0f;

    [SerializeField] Transform FootLeftTrans;

    [SerializeField] Rigidbody rigid;

    protected float SaveYDir;

    [SerializeField] float minRotationMoveSqr = 0.001f; // 회전 판단을 위한 최소 이동 제곱거리

    //이런 거대한모듈을 만들 때에 한번 "대분류"로 분류하기
    //자식에서 더 이상 못 바꾸게!
    public sealed override System.Type RegistrationType => typeof(MovementModule);

    public Vector3 LastMoveDelta { get; private set; }

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);
    }


    public override void OnUnregistration(CharacterBase oldOwner)
    {
        base.OnUnregistration(oldOwner);
        GameManager.OnPhysicCharacter -= MovementUpdate;
    }

    public void MovementUpdate(float deltaTime)
    {
        if (Owner.PlayerSet != PlayerSet.Alive) return;
        GroundCheck();
        // 실제 이동 처리
        PhysicUpdate(deltaTime);

        // 실제로 이동한 거리 계산
        LastMoveDelta = rigid.linearVelocity * deltaTime; //이동한 위치의 차이를 계산

        if (targetDirection.HasValue && targetDirection.Value != Vector3.zero)
        {
            UpdateToRotation(LastMoveDelta, deltaTime);
        }
        
        
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
    }

    public virtual float GetMoveSpeed() => Speed;
    public virtual float GetMoveSpeed(float deltaTime) => GetMoveSpeed() * deltaTime;

    public void Translate(Vector3 delta)
    {
        transform.position += delta;
    }

    public void UpdateToDirection(float deltaTime)
    {
        
        if (targetDirection is null) return;

        float currentMoveSpeed = GetMoveSpeed();

        Vector3 velocity = rigid.linearVelocity;

        // X/Z 이동만 변경
        //
        velocity.x = targetDirection.Value.x * currentMoveSpeed;
        velocity.z = targetDirection.Value.z * currentMoveSpeed;

        rigid.linearVelocity = velocity;
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
        targetDestination = null;

        // 이동에는 X/Z만 사용
        //
        Vector3 move = direction;
        move.y = 0f;

        targetDirection = move.normalized;
    }

    public void JumpDirection(Vector3 direction)
    {
        GroundCheck();

        targetDestination = null;

        if (direction.y <= 0.0f) return;

        if (isJump) return;

        if (!isGround) return;

        isJump = true;
        SaveYDir = direction.y;

        Vector3 velocity = rigid.linearVelocity;
        velocity.y = (GetMoveSpeed() * 0.8f) * SaveYDir;

        rigid.linearVelocity = velocity;
        //
    }

    public void StopMovement()
    {
        targetDestination = null; //목적지를 제거한다
        targetDirection = null;// 방향으로는 움직이지 않겠다!
    }

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

        if (isJump && isGround)
        {
            isJump = false;
        }

        return isGround;
    }

    public Vector3 InputDirection
    {
        get
        {
            return targetDirection ?? Vector3.zero;
        }
    }
}
