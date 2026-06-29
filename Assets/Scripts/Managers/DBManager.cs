using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class DBManager : ManagerBase
{
    FirebaseAuth authentication;
    private FirebaseUser user;
    private DatabaseReference DBReference;

    protected override IEnumerator OnConnected(GameManager newManager)
    {
        //                  의존성 검사           비동기
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(InitializeFireBase);
        yield return null;
    }

    protected override void OnDisconnect()
    {

    }

    void InitializeFireBase(Task<DependencyStatus> task)
    {
        if(task.Result == DependencyStatus.Available)
        {
            //인증용 인스턴스 가져오기
            authentication = FirebaseAuth.DefaultInstance;
            //인증을 하기 위해서는 "유저"가 있어야 한다
            user = authentication.CurrentUser;
            //데이터 베이스에 가려면 데이터 베이스가 어디에 있는지 찾아갈 수 있어야 한다!
            //데이터베이스 참조()
            DBReference = FirebaseDatabase.DefaultInstance.RootReference;

            //로그인을 합시다
            GuestLogin();

            Debug.Log("Firebase Initialize");
        }
        else
        {
            Debug.LogError($"Fail to Initialize Firebase : {task.Exception}");
        }
    }

    public void GuestLogin()
    {
        if(user is not null)
        {
            Debug.Log("Login Failed : Already Has Login Data");
        }

        authentication.SignInAnonymouslyAsync().ContinueWith(OnLoginResult);
    }

    void OnLoginResult(Task<AuthResult> task)
    {
        if (task.IsCanceled || task.IsFaulted)
        {
            Debug.LogError($"Fail to Sign in : {task.Exception}");
        }

        user = task.Result.User;
        Debug.Log($"Login Succeed : {user.UserId}");
    }
}
