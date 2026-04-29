using UnityEngine;

public class UI_ButtonOpenScreen : MonoBehaviour
{
    [SerializeField] UIType wantType;
    [SerializeField] ScreenChangeType changeType;

    public void Open()
    {
        UIManager.ClaimOpenScreen(wantType, changeType);
    }
}
