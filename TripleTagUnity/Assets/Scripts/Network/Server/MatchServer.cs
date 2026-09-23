using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Fusion;
using System;

public class MatchServer : MonoBehaviour
{
    private HubConnection _connection;

    public static Action OnMatchFound;

    private string _playerId;
    private string _matchId;

    private NetworkRunner _runner;

    private string _pendingMatchId;
    private string _pendingSessionName;
    private bool _startFusionRequested;

    private bool _matchReadyRequested = false;

    private async void OnEnable()
    {
        _playerId = System.Guid.NewGuid().ToString();

        _connection = new HubConnectionBuilder().WithUrl("http://192.168.0.44:5125/match").WithAutomaticReconnect().Build();

        _connection.On("ReadySuccess", () =>
        {
            Debug.Log("[Match] Ready 성공!");

            _matchReadyRequested = true;
        });

        _connection.On("ReadyFailed", () =>
        {
            Debug.LogError("[Match] Ready 실패!");
        });

        _connection.On<string, string>("MatchStarting", (matchId, sessionName) =>
        {
            Debug.Log($"[Match] 게임 시작 준비 : {matchId}");

            Debug.Log($"[Match] Fusion Session : {sessionName}");

            _pendingMatchId = matchId;
            _pendingSessionName = sessionName;

            _startFusionRequested = true;
        });

        _connection.On<int>("MatchWaiting", queueCount =>
        {
            Debug.Log($"[Match] 대기 중 : {queueCount}/6");
        });

        _connection.On<string, string[]>("MatchFound", async (matchId, players) =>
        {
            _matchId = matchId;

            Debug.Log($"[Match] 매칭 완료!");
            Debug.Log($"MatchId : {matchId}");

            foreach (string player in players)
            {
                Debug.Log($"Player : {player}");
            }

            // 테스트용 자동 Ready
            try
            {
                Debug.Log($"[Match] Ready 요청 : {_playerId}");

                await _connection.InvokeAsync("Ready", _matchId, _playerId);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Match] Ready 요청 실패 : {e}");

                return;
            }

        });

        try
        {
            await _connection.StartAsync();

            Debug.Log("[SignalR] 서버 연결 성공");

            await _connection.InvokeAsync("JoinRank", _playerId);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SignalR] 연결 실패 : {e}");
        } 


        GameManager.OnUpdateManager -= UpdateManager;
        GameManager.OnUpdateManager += UpdateManager;
    }

    private void OnDisable()
    {
        _startFusionRequested = true;
    }

    private void UpdateManager(float deltaTime)
    {
        if (_matchReadyRequested)
        {
            _matchReadyRequested = false;

            Debug.Log("[MatchServer] OnMatchFound Invoke!");

            OnMatchFound?.Invoke();

            Debug.Log("[MatchServer] OnMatchFound Invoke 완료!");
        }

        UpdateFusion(deltaTime);
    }

    private void UpdateFusion(float deltaTime)
    {
        if (!_startFusionRequested)
        {
            return;
        }

        _startFusionRequested = false;

        StartFusion(_pendingMatchId, _pendingSessionName);
    }

    private async void StartFusion(string matchId, string sessionName)
    {
        try
        {
            Debug.Log($"[Fusion] Runner 생성 시작 : {sessionName}");

            _runner = FindFirstObjectByType<NetworkRunner>();

            if (_runner == null)
            {
                Debug.LogError(
                    "[Fusion] NetworkRunner를 찾을 수 없습니다.");

                return;
            }

            Debug.Log(
                $"[Fusion] NetworkRunner 발견 : {_runner.name}");

            if (_runner.IsRunning)
            {
                Debug.LogWarning(
                    "[Fusion] NetworkRunner가 이미 실행 중입니다.");

                return;
            }


            NetworkSceneInfo sceneInfo = new NetworkSceneInfo();

            Scene activeScene = SceneManager.GetActiveScene();

            Debug.Log($"[Fusion] 현재 Scene : {activeScene.name}");

            Debug.Log($"[Fusion] Scene BuildIndex : {activeScene.buildIndex}");

            if (activeScene.buildIndex < 0)
            {
                Debug.LogError("[Fusion] 현재 Scene이 Build Settings에 등록되어 있지 않습니다.");

                return;
            }

            sceneInfo.AddSceneRef(SceneRef.FromIndex(activeScene.buildIndex), LoadSceneMode.Single);

            Debug.Log("[Fusion] Scene 정보 생성 완료");

            NetworkSceneManagerDefault sceneManager = _runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

            Debug.Log("[Fusion] SceneManager 생성 완료");

            Debug.Log($"[Fusion] StartGame 시작 : {sessionName}");

            StartGameResult result = await _runner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.AutoHostOrClient,
                SessionName = sessionName,
                Scene = sceneInfo,
                SceneManager = sceneManager
            });

            if (result.Ok)
            {
                Debug.Log($"[Fusion] Session 접속 성공 : {sessionName}");
            }
            else
            {
                Debug.LogError($"[Fusion] Session 접속 실패 : {result.ShutdownReason}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Fusion] StartGame 처리 중 예외 발생\n{e}");
        }
    }

    

    private async void OnDestroy()
    {
        GameManager.OnUpdateManager -= UpdateManager;

        if (_connection != null)
        {
            await _connection.DisposeAsync();
        }

    }
}