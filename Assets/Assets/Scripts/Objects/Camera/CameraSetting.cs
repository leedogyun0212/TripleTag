using Unity.Cinemachine;
using UnityEngine;

public class CameraSetting : MonoBehaviour
{
    [SerializeField] CinemachineCamera cinemachineCamera;

    public static CameraSetting Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SetTarget(Transform target)
    {
        cinemachineCamera.Target.TrackingTarget = target;
    }
}
