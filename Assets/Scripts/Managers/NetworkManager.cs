using Fusion;
using UnityEngine;


public class NetworkManager : NetworkBehaviour
{
    [Networked]
    public Vector3 NetworkMoveDelta { get; set; }

    [SerializeField] MovementModule movement;

    private Vector3 _previousPosition;

    //public override void Spawned()
    //{
    //    Debug.Log(
    //        $"Spawned {Object.Id} " +
    //        $"State:{Object.HasStateAuthority} " +
    //        $"Input:{Object.HasInputAuthority}"
    //    );

    //    if (Object.HasStateAuthority)
    //    {
    //        NetworkMoveDelta = Vector3.zero;
    //    }
    //}

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        if (GetInput(out NetworkInputData input))
        {

            movement.MoveToDirection(input.MoveDirection);
            movement.MovementUpdate(Runner.DeltaTime);

            NetworkMoveDelta = movement.LastMoveDelta;

        }
    }

    public override void Render()
    {
        if (Object.HasInputAuthority)
        {
            GameManager.Camera.CameraMove(
                NetworkMoveDelta,
                movement.Owner.Head.position);
        }

        movement.Owner.MovementNotify(NetworkMoveDelta);
    }
}