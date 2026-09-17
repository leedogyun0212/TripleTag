using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;

public struct NetworkInputData : INetworkInput
{
    public Vector3 MoveDirection;
    public NetworkBool Attack;
}

public class PlayerInputHandler : MonoBehaviour
{
    private Vector3 moveDirection;

    private bool _attackPressed;

    public static PlayerInputHandler Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SetMoveDirection(Vector3 direction)
    {
        moveDirection = direction;
    }

    public void OnAttack()
    {
        _attackPressed = true;
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        NetworkInputData data = new NetworkInputData();

        data.MoveDirection = moveDirection;

        data.Attack = _attackPressed;
        _attackPressed = false;
        Debug.Log($"OnInput 호출+{data.Attack}");

        input.Set(data);
    }


    public void OnInputMissing(
        NetworkRunner runner,
        PlayerRef player,
        NetworkInput input)
    {

    }
}