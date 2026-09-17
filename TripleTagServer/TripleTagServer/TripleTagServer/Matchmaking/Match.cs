using TripleTagServer.Players;

namespace TripleTagServer.Matchmaking;

public class Match
{
    public string MatchId { get; }

    public List<PlayerConnection> Players { get; }

    public MatchState State { get; private set; }

    public HashSet<string> ReadyPlayers { get; } = new();

    public string FusionSessionName { get; }

    public Match(string matchId, List<PlayerConnection> players)
    {
        MatchId = matchId;
        Players = players;

        State = MatchState.Waiting;

        FusionSessionName = $"TripleTag_{matchId}";
    }

    public void SetState(MatchState state)
    {
        State = state;
    }
}