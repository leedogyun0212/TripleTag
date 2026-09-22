using TripleTagServer.Players;

namespace TripleTagServer.Matchmaking;

public class MatchmakingService
{
    private readonly List<PlayerConnection> _queue = new();

    private const int MaxPlayers = 2;
    
    public int QueueCount => _queue.Count;

    private readonly MatchStore _matchStore;

    public MatchmakingService(MatchStore matchStore)
    {
        _matchStore = matchStore;
    }

    public int PlayerMax()
    {
        return MaxPlayers;
    }

    public Match? AddPlayer(string playerId, string connectionId)
    {
        if (_queue.Any(x => x.PlayerId == playerId))
        {
            Console.WriteLine($"{_queue.Any(x => x.PlayerId == playerId)} == {playerId}???");
            return null;
        }

        PlayerConnection player = new PlayerConnection(
            playerId,
            connectionId);

        _queue.Add(player);

        Console.WriteLine($"[Queue] Player {playerId} joined. " + $"({_queue.Count}/{MaxPlayers})");

        if (_queue.Count < MaxPlayers)
        {
            return null;
        }

        return CreateMatch();
    }

    private Match CreateMatch()
    {
        List<PlayerConnection> players = _queue.Take(MaxPlayers).ToList();

        _queue.RemoveRange(0, MaxPlayers);

        string matchId = Guid.NewGuid().ToString();

        Match match = new Match(matchId, players);

        _matchStore.Add(match);

        Console.WriteLine( $"[MatchStore] Match 저장 완료 : {match.MatchId}, 현재 Match 수 : {_matchStore.Count}");

        Console.WriteLine( $"[Match] Match created : {match.MatchId}");

        Console.WriteLine($"[Match] State : {match.State}");

        foreach (PlayerConnection player in players)
        {
            Console.WriteLine($"[Match] Player : {player.PlayerId}");
        }

        return match;
    }

    public bool RemovePlayer(string playerId)
    {
        PlayerConnection? player = _queue.FirstOrDefault(x => x.PlayerId == playerId);

        if (player == null)
        {
            return false;
        }

        _queue.Remove(player);

        Console.WriteLine($"[Queue] Player {playerId} removed.");

        return true;
    }

    public bool RemovePlayerByConnection(string connectionId)
    {
        PlayerConnection? player =
            _queue.FirstOrDefault(x => x.ConnectionId == connectionId);

        if (player == null)
        {
            return false;
        }

        _queue.Remove(player);

        Console.WriteLine(
            $"[Queue] Player {player.PlayerId} disconnected. " + $"({connectionId}) " + $"({_queue.Count}/{MaxPlayers})");

        return true;
    }

    public bool SetPlayerReady(string matchId, string playerId)
    {
        if (!_matchStore.TryGet(matchId, out Match? match))
        {
            return false;
        }

        bool isPlayerInMatch = match.Players.Any(x => x.PlayerId == playerId);

        if (!isPlayerInMatch)
        {
            return false;
        }

        match.ReadyPlayers.Add(playerId);

        Console.WriteLine($"[Ready] Player {playerId} ready " + $"({match.ReadyPlayers.Count}/{match.Players.Count})");

        if (match.ReadyPlayers.Count == match.Players.Count)
        {
            match.SetState(MatchState.Ready);

            Console.WriteLine($"[Match] All players ready : {match.MatchId}");

            Console.WriteLine($"[Match] State : {match.State}");

            // 모든 플레이어가 준비되었으므로 Starting 상태로 전환
            match.SetState(MatchState.Starting);

            Console.WriteLine($"[Match] State : {match.State}");
        }

        return true;
    }

    public Match? GetMatch(string matchId)
    {
        if (_matchStore.TryGet(matchId, out Match? match))
        {
            return match;
        }

        return null;
    }
}