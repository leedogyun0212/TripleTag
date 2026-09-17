using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraManager : ManagerBase
{
    public Camera MainCamera { get; private set; }

    

    protected override IEnumerator OnConnected(GameManager newManager)
    {
        SetMainCamera(Camera.main);
        yield return null;
    }

    protected override void OnDisconnect()
    {

    }

    public void SetMainCamera(Camera wantCamera)
    {
        MainCamera = wantCamera;
    }

    public void GetRaycastResult(Vector2 screenPosition, List<RaycastResult> outResult)
    {
        EventSystem currentEvent = EventSystem.current;
        if (!currentEvent) return;

        PointerEventData eventData = new(currentEvent);
        eventData.position = screenPosition;
        currentEvent.RaycastAll(eventData, outResult);
    }

    public void CameraMove(Transform target)
    {
        //Vector3 Position, Vector3 Head
        //Vector3 cameraPosition = Head;

        //cameraPosition += Vector3.up * 7.0f;
        //cameraPosition += Vector3.back * 5.0f;

        //MainCamera.transform.position = cameraPosition;
        //MainCamera.transform.LookAt(Head);

        //cinemachineCamera.Follow = target;
    }
}
