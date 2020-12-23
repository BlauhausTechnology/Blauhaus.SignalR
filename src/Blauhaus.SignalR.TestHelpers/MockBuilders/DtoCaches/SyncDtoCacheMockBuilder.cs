using System.Collections.Generic;
using System.Linq;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.Sync.Abstractions;
using Moq;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders.DtoCaches
{
    public class SyncDtoCacheMockBuilder<TDto> : BaseDtoCacheMockBuilder<SyncDtoCacheMockBuilder<TDto>,ISyncDtoCache<TDto>,TDto> where TDto : ISyncClientEntity
    {
        public void VerifySaveDtosAsync(TDto dto)
        {
            Mock.Verify(x => x.SaveDtosAsync(It.Is<SyncResult<TDto>>(y => 
                y.EntityBatch.FirstOrDefault(z => z.Equals(dto)) != null)));
        }
        
        public void VerifySaveDtosAsync(List<TDto> dtos)
        {
            foreach (var dto in dtos)
            {
                Mock.Verify(x => x.SaveDtosAsync(It.Is<SyncResult<TDto>>(y => 
                    y.EntityBatch.FirstOrDefault(z => z.Equals(dto)) != null)));
            }
        }
        
        public void VerifySaveDtosAsync(params TDto[] dtos)
        {
            foreach (var dto in dtos)
            {
                Mock.Verify(x => x.SaveDtosAsync(It.Is<SyncResult<TDto>>(y => 
                    y.EntityBatch.FirstOrDefault(z => z.Equals(dto)) != null)));
            }
        }
    }
}