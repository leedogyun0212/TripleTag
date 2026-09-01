using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static DBManager;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class DBManager : ManagerBase
{
    FirebaseAuth authentication;
    private FirebaseUser user;
    private DatabaseReference rootDB;

    public UserNormalData CurrentUserNormalData { get; private set; }
    public UserPlayerData CurrentUserPlayerData { get; private set; }

    public DateTime AssignDate { get; private set; }


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
            rootDB = FirebaseDatabase.DefaultInstance.RootReference;

            //로그인을 합시다
            GuestLogin();

            Debug.Log($"Firebase Initialize");
        }
        else
        {
            Debug.LogError($"Fail to Initialize Firebase : {task.Exception}");
        }
    }

    public TMPro.TMP_InputField nickNameInput;

    public void MakeUserData()
    {
        WriteData(MakeNewUserData(nickNameInput.text), "users", "userData", user.UserId);
    }
    public void MakenewUserData()
    {
        WriteData(MakeNewUserNormalData(nickNameInput.text), "users",user.UserId , "UserNormalData");
        WriteData(MakeNewUserPlayerData(10000), "users",user.UserId , "UserPlayData");
    }

    public async void GuestLogin()
    {
        //인증기가 존재하지 않으면     ?? 
        if (authentication is null) return;
        //이미 로그인 되었는지 확인하기
        if(user is not null)
        {
            Debug.LogError($"Login Failed : Already Has Login Data ({user.IsValid()}, {user.UserId})");
            CurrentUserNormalData = await ReadDataAsync<UserNormalData>("users", user.UserId, "UserNormalData");
            CurrentUserPlayerData = await ReadDataAsync<UserPlayerData>("users", user.UserId, "UserPlayData");
            AssignDate = DateTimeOffset.FromUnixTimeSeconds(CurrentUserNormalData.joinDate).LocalDateTime;
            CurrentUserPlayerData.lastLoginDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (CurrentUserNormalData is not null && CurrentUserPlayerData is not null)
            {
                Debug.Log($"닉네임 : {CurrentUserNormalData.nickName}");
                Debug.Log($"가입일 : {AssignDate.ToString("yyyy-MM-dd")}");
                Debug.Log($"돈 : {CurrentUserPlayerData.money}");
                Debug.Log($"승리횟수 : {CurrentUserPlayerData.winCount}");
            }
            else
            {
                WriteData(MakeNewUserData("???"), "users", "userData", user.UserId);
            }
            return;
        }
        //익명으로 로그인하기!
        await authentication.SignInAnonymouslyAsync().ContinueWithOnMainThread(OnLoginResult);
    }

    void OnLoginResult(Task<AuthResult> task)
    {
        if (task.IsCanceled || task.IsFaulted)
        {
            Debug.LogError($"Fail to Sign in : {task.Exception}");
        }

        user = task.Result.User;
        WriteData(MakeNewUserData("???"), "users", "userData");
        Debug.Log($"Login Succeed : {user.UserId}");
    }

    [Serializable]
    public class UserData
    {
        public string nickName;
        public DateTime assignDate;
        public long joinDate;
        public int userLevel;
        public int money;
        public int attendtime;
    }
    public class UserNormalData
    {
        public string nickName;
        public long joinDate;
    }

    public class UserPlayerData
    {
        public int money;
        public long lastLoginDate;
        public int rating;
        public int winCount;
        public int LoseCount;
        public int mvpCount;
    }
    public UserData MakeNewUserData(string wantNickname) => new()
    {
        nickName    = wantNickname,
        joinDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        userLevel   = 1,
        money       = 100000,
        attendtime  = 0
    };
    public UserNormalData MakeNewUserNormalData(string wantNickname) => new()
    {
        nickName    = wantNickname,
        joinDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
    };
    public UserPlayerData MakeNewUserPlayerData(int wantMoney) => new()
    {
        money       = wantMoney,
        rating     = 0,
        winCount    = 0,   
        LoseCount   = 0,
        mvpCount    = 0,
        lastLoginDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
    };


    public void nameChange(string changeName)
    {
        if (CurrentUserNormalData is null) return;

        Dictionary<string, object> changes = new()
        {
            { "nickName", changeName }
        };

        WriteData(changes, "users", user.UserId, "UserNormalData");
        CurrentUserNormalData.nickName = changeName;
    }

    public DatabaseReference GetFinalDirectory(DatabaseReference root, params string[] directory)
    {
        if (directory is null || directory.Length == 0) return root;
        DatabaseReference currentReference = root;
        foreach (string currentChild in directory)
        {
            currentReference = currentReference.Child(currentChild);
        }
        return currentReference;
    }

    void OnTaskResult(Task task)
    {
        if (task.IsCanceled || task.IsFaulted)
        {
            Debug.LogError(task.Exception);
        }
    }

    public void WriteData(object wantData, params string[] directory)
    {
        if (rootDB is null || wantData is null) return;
        //NoSQL은 무엇으로 저장하는지 기억하시나요?
        //JSON으로 저장합니다!
        //{    키       값   => 딕셔너리에서 들어본 거다! 
        //   "이름" : "내용"
        //}
        string jsonData = JsonUtility.ToJson(wantData);
        //일단 뿌리에서 시작
        GetFinalDirectory(rootDB, directory).SetRawJsonValueAsync(jsonData).ContinueWithOnMainThread(OnTaskResult);
    }

    public void WriteData(Dictionary<string, object> changes, params string[] directory) 
    {
        if (rootDB is null || changes is null) return;
        //폴더를 따라 내려가는 것
        //제일 처음에 만든 rootDB가 바로 root폴더 => c드라이브다
        //Update : 최신화하다 => 내용을 기입하다

        GetFinalDirectory(rootDB, directory).UpdateChildrenAsync(changes).ContinueWithOnMainThread(OnTaskResult);
        
    }
    public void ReadData(Action<Task<DataSnapshot>> OnReadData, params string[] directory)
    {
        GetFinalDirectory(rootDB, directory).GetValueAsync().ContinueWithOnMainThread(OnReadData);
    }

    public IEnumerator ReadDataCoroutine(Action<Task<DataSnapshot>> OnReadData, params string[] directory)
    {
        Task<DataSnapshot> readtask = GetFinalDirectory(rootDB, directory).GetValueAsync();
        yield return readtask.WaitForTask();
        OnReadData.Invoke(readtask);
    }

    public async Task<T> ReadDataAsync<T>(params string[] directory)
    {
        
        //다른 비동기 함수가 진행되는 동안 기다린다라고 알려주는 구문
        DataSnapshot currentTask = await GetFinalDirectory(rootDB, directory).GetValueAsync();

        if (currentTask is null) return default;
        Debug.Log("???");
        if (!currentTask.Exists) return default;

        //2. 복합타입
        //구조화된 존재를 어떻게 저장하고 있었을까?
        //JSON의 형태로 저장했었다!
        try
        {
            if (currentTask.HasChildren)
            {
                return JsonUtility.FromJson<T>(currentTask.GetRawJsonValue());
            }
            //2. 단일타입 
            return (T)System.Convert.ChangeType(currentTask.Value, typeof(T));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return default;
        }
    }
}
