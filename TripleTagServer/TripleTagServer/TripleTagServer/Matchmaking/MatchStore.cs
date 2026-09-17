using System.Collections.Concurrent;

namespace TripleTagServer.Matchmaking;

public class MatchStore
{
    private readonly ConcurrentDictionary<string, Match> _matches = new();

    public int Count => _matches.Count;

    public void Add(Match match)
    {
        _matches[match.MatchId] = match;
    }

    public bool TryGet(string matchId, out Match? match)
    {
        return _matches.TryGetValue(matchId, out match);
    }

    public bool Remove(string matchId)
    {
        return _matches.TryRemove(matchId, out _);
    }

    public IReadOnlyCollection<Match> GetAll()
    {
        return _matches.Values.ToList();
    }
}