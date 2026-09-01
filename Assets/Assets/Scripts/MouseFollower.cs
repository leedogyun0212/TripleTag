using UnityEngine;

public class MouseFollower : MonoBehaviour, IFunctionable
{
    void Start()
    {
        RegistrationFunctions();
    }
    void OnDestroy()
    {
        UnregistrationFunctions();
    }

    public void RegistrationFunctions()
    {
        //마우스 움직임이 발생했을 때에 할 일에 => 마우스 따라가기를 넣기
        InputManager.OnAttack += (value) => UIManager.ClaimPopUp("확인", $"움직임 : {value}", "확인");
        InputManager.OnMove += (value) => UIManager.ClaimPopUp("확인", $"움직임 : {value}", "확인");
    }

    public void UnregistrationFunctions()
    {

    }

    void DestroyOnMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        ObjectManager.DestroyObject(GameManager.Input.GetGameObjectUnderCursor());
    }
    void CreateToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        //저희가 로딩해놓은 거 있잖아요!
        GameObject inst = ObjectManager.CreateObject("NemoMan");
    }

    void MoveToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        transform.position = worldPosition;

    }

    void TestAttack(bool value)
    {
        UIManager.ClaimPopUp($"확인", $"움직임", "확인");
    }
}