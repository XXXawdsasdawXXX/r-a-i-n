namespace Core.GameLoop
{
    public interface INetworkOwnership
    {
        bool IsOwnedByClient { get; }
    }
}