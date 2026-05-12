using UnityEngine;

public class UI_GameStartWindow : OpenableUIBase
{
    public void Ranking()
    {
        UIManager.ClaimCloseUI(UIType.GameStart);
    }
}
