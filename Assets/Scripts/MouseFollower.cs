using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    void Start()
    {
        //마우스 움직임이 발생했을 때에 할 일에 => 마우스 따라가기를 넣기
        InputManager.OnMouseLeftUp += CreateToMouse;
        InputManager.OnMouseRightDown += DestroyOnMouse;
    }

    void DestroyOnMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        Debug.Log(GameManager.Instance.Input.GetGameObjectUnderCursor());
    }
    void CreateToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        //저희가 로딩해놓은 거 있잖아요!
        Instantiate(DataManager.LoadDataFile<GameObject>("Square 10"),worldPosition,Quaternion.identity);
    }

    void MoveToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        //worldPosition.z += 10;
        transform.position = worldPosition;

    }
}