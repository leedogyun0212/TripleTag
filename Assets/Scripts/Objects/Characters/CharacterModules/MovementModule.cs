using UnityEngine;

public class MovementModule : CharacterModule, IRunnable
{
    protected Vector3? targetDirection = null;
    protected Vector3? targetDestination = null;
    protected float targetTolerance;
    
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
        Owner.MovementNotify(positionDelta);                              //이동한 양에 따라서 애니메이션을 업데이트
    }

    public void PhysicUpdate(float deltaTime)
    {
        UpdateToDirection(deltaTime);
        UpdateToDestination(deltaTime);
    }

    public virtual float GetMoveSpeed() => 5.0f;
    public virtual float GetMoveSpeed(float deltaTime) => GetMoveSpeed() * deltaTime;

    public void Translate(Vector3 delta)
    {
        transform.position += delta;
    }

    public void UpdateToDirection(float deltaTime)
    {
        if (targetDirection is null) return;

        float currentMoveSpeed = GetMoveSpeed(deltaTime);
        Translate(currentMoveSpeed * targetDirection.Value);

    }
    public void UpdateToDestination(float deltaTime)
    {
        //목적지가 없으면 리턴
        if (targetDestination is null) return;

        //여길 넘어서면 목적지가 있는 상태

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
    }

    public void StopMovement()
    {
        targetDestination = null; //목적지를 제거한다
        targetDirection = null;// 방향으로는 움직이지 않겠다!
    }
}
