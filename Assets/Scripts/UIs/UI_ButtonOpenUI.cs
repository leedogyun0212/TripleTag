using UnityEngine;

public class UI_ButtonOpenUI : MonoBehaviour
{
    [SerializeField] UIType wantType;
    [SerializeField] bool wantToggle;

    public void Open()
    {
        if (wantToggle) UIManager.ClaimOpenScreen(wantType);
        else UIManager.ClaimOpenUI(wantType);
    }
}
