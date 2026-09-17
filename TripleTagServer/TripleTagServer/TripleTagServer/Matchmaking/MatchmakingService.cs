using TripleTagServer.Players;

namespace TripleTagServer.Matchmaking;

public class MatchmakingService
{
    private readonly List<PlayerConnection> _queue = new();

    private const int MaxPlayers = 6;

    public int QueueCount => _queue.Count;

    public Match? AddPlayer(string playerId, string connectionId)
    {
        if (_queue.Any(x => x.PlayerId == playerId))
        {
            return null;
        }

        PlayerConnection player = new PlayerConnection(
            playerId,
            connectionId);

        _queue.Add(player);

        Console.WriteLine(
            $"[Queue] Player {playerId} joined. " +
            $"({_queue.Count}/{MaxPlayers})");

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

        Match match = new Match(
            matchId,
            players);

        Console.WriteLine(
            $"[Match] Match created : {match.MatchId}");

        foreach (PlayerConnection player in players)
        {
            Console.WriteLine(
                $"[Match] Player : {player.PlayerId}");
        }

        return match;
    }

    public bool RemovePlayer(string playerId)
    {
        PlayerConnection? player = _queue
            .FirstOrDefault(x => x.PlayerId == playerId);

        if (player == null)
        {
            return false;
        }

        _queue.Remove(player);

        Console.WriteLine(
            $"[Queue] Player {playerId} removed.");

        return true;
    }
}