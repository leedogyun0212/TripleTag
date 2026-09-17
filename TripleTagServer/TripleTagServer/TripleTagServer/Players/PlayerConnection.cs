namespace TripleTagServer.Players;

public class PlayerConnection
{
    public string PlayerId { get; }

    public string ConnectionId { get; }

    public PlayerConnection(
        string playerId,
        string connectionId)
    {
        PlayerId = playerId;
        ConnectionId = connectionId;
    }
}   