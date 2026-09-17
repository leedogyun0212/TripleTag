using TripleTagServer.Players;

namespace TripleTagServer.Matchmaking;

public class Match
{
    public string MatchId { get; }

    public List<PlayerConnection> Players { get; }

    public Match(
        string matchId,
        List<PlayerConnection> players)
    {
        MatchId = matchId;
        Players = players;
    }
}