using UnityEngine;

public class UI_MainScreen : UI_ScreenBase
{
    public void OnEnable()
    {
        InputManager.OnOption -= OptionStart;
        InputManager.OnOption += OptionStart;

        InputManager.OnExit -= OpenTitle;
        InputManager.OnExit += OpenTitle;

        InputManager.OnShop -= Shop;    
        InputManager.OnShop += Shop;
          
        InputManager.OnRanking -= Rank;
        InputManager.OnRanking += Rank;
             
        InputManager.OnMessage -= Message;
        InputManager.OnMessage += Message;

        InputManager.OnProfile -= Profile;   
        InputManager.OnProfile += Profile;
     
        InputManager.OnStart -= OpenStart;
        InputManager.OnStart += OpenStart;
    }

    public void OnDisable()
    {
        InputManager.OnOption -= OptionStart;
        InputManager.OnExit -= OpenTitle;
        InputManager.OnShop -= Shop;
        InputManager.OnRanking -= Rank;    
        InputManager.OnMessage -= Message; 
        InputManager.OnProfile -= Profile;
        InputManager.OnStart -= OpenStart;
    }

    void OptionStart(bool value)
    {
        UIManager.ClaimToggleUI(UIType.Option);
    }

    void Shop(bool value)
    {
        UIManager.ClaimToggleUI(UIType.Shop);
    }
    void Rank(bool value)
    {
        UIManager.ClaimToggleUI(UIType.Rank);
    }
    void Message(bool value)
    {
        UIManager.ClaimToggleUI(UIType.Message);
    }
    void Profile(bool value)
    {
        UIManager.ClaimToggleUI(UIType.Profile);
    }

    void OpenTitle(bool value)
    {
        UIManager.ClaimOpenScreen(UIType.Title, ScreenChangeType.ScreenChanger);
    }
    void OpenStart(bool value)
    {
        UIManager.ClaimOpenUI(UIType.GameStart);

    }
}
