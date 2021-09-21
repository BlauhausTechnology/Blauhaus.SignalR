using Blauhaus.Domain.Abstractions.Sync;

namespace Blauhaus.SignalR.Client.Clients
{
    public class EmptyDtoBatch<TDto> : IDtoBatch<TDto>
    {
        public EmptyDtoBatch(int currentDtoCount, int remainingDtoCount, long batchLastModifiedTicks)
        {
            CurrentDtoCount = currentDtoCount;
            RemainingDtoCount = remainingDtoCount;
            BatchLastModifiedTicks = batchLastModifiedTicks;
        }

        public EmptyDtoBatch(DtoObjectBatch objectBatch, long batchLastModifiedTicks)
        {
            CurrentDtoCount = objectBatch.DtoObjects.Count;
            RemainingDtoCount = objectBatch.RemainingDtoCount;
        }

        public int CurrentDtoCount { get; }
        public int RemainingDtoCount { get; }
        public long BatchLastModifiedTicks { get; }
    }
}