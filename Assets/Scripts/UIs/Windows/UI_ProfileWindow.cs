using UnityEngine;

public class UI_ProfileWindow : OpenableUIBase
{
    [SerializeField] TMPro.TextMeshProUGUI Nickname;
    [SerializeField] TMPro.TextMeshProUGUI joinDate;
    [SerializeField] TMPro.TextMeshProUGUI WinCount;
    [SerializeField] TMPro.TextMeshProUGUI mvpCount;
    [SerializeField] GameObject ChangeOn;
    public TMPro.TMP_InputField nickNameInput;

    public override void Registration(UIManager manager)
    {
        base.Registration(manager);
        MyProfile();
    }

    public override void Unregistration(UIManager manager)
    {
        base.Unregistration(manager);
    }

    private void MyProfile()
    {
        Nickname.SetText($"닉네임 : { GameManager.DB.CurrentUserNormalData.nickName}");
        joinDate.SetText($"가입일 : {GameManager.DB.AssignDate.ToString("yyyy-MM-dd")}");
        WinCount.SetText($"승리횟수 : { GameManager.DB.CurrentUserPlayerData.winCount}");
        mvpCount.SetText($"MVP횟수 : { GameManager.DB.CurrentUserPlayerData.mvpCount}");
    }

    public void ChangeNicknameOn()
    {
        ChangeOn.SetActive(true);
    }

    public void ChangeName()
    {
        GameManager.DB.nameChange(nickNameInput.text);
        Nickname.SetText($"닉네임 : {GameManager.DB.CurrentUserNormalData.nickName}");
        ChangeOn.SetActive(false);
    }
}

