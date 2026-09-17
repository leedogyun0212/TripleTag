using Microsoft.AspNetCore.SignalR;
using TripleTagServer.Matchmaking;
using TripleTagServer.Players;

namespace TripleTagServer.Hubs;

public class MatchHub : Hub
{
    private readonly MatchmakingService _matchmakingService;

    public MatchHub(MatchmakingService matchmakingService)
    {
        _matchmakingService = matchmakingService;
    }

    public async Task JoinRank(string playerId)
    {
        Console.WriteLine($"[Request] JoinRank : {playerId}");

        string connectionId = Context.ConnectionId;

        Match? match = _matchmakingService.AddPlayer(playerId, connectionId);

        // 아직 매칭되지 않음
        if (match == null)
        {
            await Clients.Caller.SendAsync("MatchWaiting", _matchmakingService.QueueCount);

            return;
        }

        // 6명 매칭 완료
        //
        Console.WriteLine($"[Match] {_matchmakingService.PlayerMax()} players matched : {match.MatchId}");

        // 매칭된 6명에게만 MatchFound 전송
        foreach (var player in match.Players)
        {
            await Clients.Client(player.ConnectionId).SendAsync("MatchFound", match.MatchId, match.Players.Select(x => x.PlayerId).ToList());
        }
    }

    public async Task Ready(string matchId, string playerId)
    {
        Console.WriteLine($"[Request] Ready : {playerId}");

        bool success = _matchmakingService.SetPlayerReady(matchId, playerId);

        if (!success)
        {
            await Clients.Caller.SendAsync("ReadyFailed");

            return;
        }

        await Clients.Caller.SendAsync("ReadySuccess");

        // 모든 플레이어가 Ready되어 Starting 상태가 되었는지 확인
        Match? match = _matchmakingService.GetMatch(matchId);

        if (match == null)
        {
            return;
        }

        if (match.State == MatchState.Starting)
        {
            Console.WriteLine($"[Match] Starting : {match.MatchId}");

            foreach (var player in match.Players)
            {
                await Clients.Client(player.ConnectionId).SendAsync("MatchStarting", match.MatchId, match.FusionSessionName);
            }
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string connectionId = Context.ConnectionId;

        _matchmakingService.RemovePlayerByConnection(connectionId);

        await base.OnDisconnectedAsync(exception);
    }
}