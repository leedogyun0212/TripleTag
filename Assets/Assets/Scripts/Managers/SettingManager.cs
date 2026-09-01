using System.Collections;
using UnityEngine;

public class SettingManager : ManagerBase
{
    protected override IEnumerator OnConnected(GameManager newManager)
    {
        //스마트폰의 방향은 가로 세로 역가로 역세로 => 허용
        Screen.autorotateToLandscapeLeft      = true; // 카메라가 왼쪽
        Screen.autorotateToLandscapeRight     = true; // 카메라가 오른쪽
        Screen.autorotateToPortrait           = true; // 카메라가 위쪽
        Screen.autorotateToPortraitUpsideDown = true; // 카메라가 아래쪽
        //방향을 하나로 한정지을 수도 있긴 함!
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        //게임하다가 화면을 클릭 오래 안하는 게임도 있잖아요?
        //컷신을 보게 되는 경우도 있고
        //스크린이 얼마나 오랫동안 터치가 안되면 꺼질지!
        //SleepTimeout.SystemSetting => 시스템 세팅에 따름
        //SleepTimeout.NeverSleep    => 절대 안잠자기
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        yield return null;
    }

    protected override void OnDisconnect()
    {

    }
}
