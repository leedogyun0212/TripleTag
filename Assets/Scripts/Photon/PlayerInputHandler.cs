using Fusion;
using Fusion.Sockets;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector3 MoveDirection;
}

public class PlayerInputHandler : MonoBehaviour
{
    private Vector3 moveDirection;

    public static PlayerInputHandler Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SetMoveDirection(Vector3 direction)
    {
        moveDirection = direction;
    }


    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        Debug.Log("OnInput 호출");
        NetworkInputData data = new NetworkInputData();

        data.MoveDirection = moveDirection;

        input.Set(data);
    }


    public void OnInputMissing(
        NetworkRunner runner,
        PlayerRef player,
        NetworkInput input)
    {

    }
}