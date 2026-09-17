using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

public class MatchServer : MonoBehaviour
{
    private HubConnection _connection;

    private async void Start()
    {
        _connection = new HubConnectionBuilder().WithUrl("https://localhost:7055/match") .WithAutomaticReconnect().Build();

        _connection.On<int>("MatchWaiting", queueCount =>
            {
                Debug.Log($"[Match] 대기 중 : {queueCount}/6");
            });

        _connection.On<string, string[]>("MatchFound", (matchId, players) =>
            {
                Debug.Log($"[Match] 매칭 완료!");
                Debug.Log($"MatchId : {matchId}");

                foreach (string player in players)
                {
                    Debug.Log($"Player : {player}");
                }
            });
        string playerId = System.Guid.NewGuid().ToString();
        try
        {
            await _connection.StartAsync();

            Debug.Log("[SignalR] 서버 연결 성공");

            await _connection.InvokeAsync($"JoinRank", playerId);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SignalR] 연결 실패 : {e}");
        }
    }
    
    private async void OnDestroy()
    {
        if (_connection != null)
        {
            await _connection.DisposeAsync();
        }
    }
}