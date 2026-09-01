using Fusion;
using Unity.Cinemachine;
using UnityEngine;


public class NetworkManager : NetworkBehaviour
{
    [Networked] public Vector3 NetworkMoveDelta { get; set; }

    [Networked] private int AttackSequence { get; set; }

    private int _lastAttackSequence;

    [SerializeField] MovementModule movement;

    [SerializeField] AnimationModule animModule;

    [SerializeField] Rigidbody rigid;

    public override void Spawned()
    {
        rigid.isKinematic = !Object.HasStateAuthority;

        if (!Object.HasInputAuthority)
            return;

        CameraSetting.Instance.SetTarget(transform);
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        if (GetInput(out NetworkInputData input))
        {

            movement.MoveToDirection(input.MoveDirection);
            movement.JumpDirection(input.MoveDirection);

            movement.MovementUpdate(Runner.DeltaTime);

            NetworkMoveDelta = movement.LastMoveDelta;

            if (input.Attack)
            {
                AttackSequence++;
            }
        }
    }


    //public override void FixedUpdateNetwork()
    //{
    //    if (GetInput(out NetworkInputData input))
    //    {
    //        movement.MoveToDirection(input.MoveDirection);
    //        movement.JumpDirection(input.MoveDirection);

    //        movement.MovementUpdate(Runner.DeltaTime);

    //        if (Object.HasStateAuthority)
    //        {
    //            NetworkMoveDelta = movement.LastMoveDelta;

    //            if (input.Attack)
    //            {
    //                AttackSequence++;
    //            }
    //        }
    //    }
    //}

    public override void Render()
    {
        //Debug.Log($"[Render] Object:{Object.Id} " + $"[Render] Tick:{Runner.Tick} " +$"Input:{Object.HasInputAuthority} " + $"State:{Object.HasStateAuthority} " + $"Pos:{transform.position}");


        movement.Owner.MovementNotify(NetworkMoveDelta);

        if (_lastAttackSequence != AttackSequence)
        {
            _lastAttackSequence = AttackSequence;

            animModule.AnimationByAttack(true);
        }
    }
}
