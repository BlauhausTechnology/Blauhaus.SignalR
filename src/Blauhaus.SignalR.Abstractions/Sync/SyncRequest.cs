namespace Blauhaus.SignalR.Abstractions.Sync
{
    public class SyncRequest
    {
        public SyncRequest(long modifiedAfter = 0)
        {
            ModifiedAfter = modifiedAfter;
        }

        public long ModifiedAfter { get; }
    }
}