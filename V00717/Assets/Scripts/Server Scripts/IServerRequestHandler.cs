public interface IServerRequestHandler
{
    // Should emit an event of its own as a request
    public void ServerReply(Enums.DataRequests portRequest);
}