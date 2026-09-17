namespace TripleTagServer.Players;

public class PlayerInfo
{
    public string PlayerId { get; }

    public PlayerInfo(string playerId)
    {
        PlayerId = playerId;
    }
}