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
        Console.WriteLine($"[Match] 6 players matched : {match.MatchId}");

        // 매칭된 6명에게만 MatchFound 전송
        foreach (var player in match.Players)
        {
            await Clients.Client(player.ConnectionId).SendAsync("MatchFound", match.MatchId, match.Players.Select(x => x.PlayerId).ToList());
        }
    }
}