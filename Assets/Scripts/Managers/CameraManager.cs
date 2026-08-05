using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
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

    public void CameraMove(Vector3 Position, Vector3 Head)
    {
        Head.y += 5.5f;
        //Head.z -= 8.0f;
        //Head.x -= 8.0f;
        MainCamera.transform.position = Head;
        //MainCamera.transform.position += Position;
    }
}
