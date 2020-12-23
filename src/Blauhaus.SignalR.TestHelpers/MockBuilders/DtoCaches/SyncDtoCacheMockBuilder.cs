using System.Collections.Generic;
using System.Linq;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Abstractions.Sync;
using Blauhaus.Sync.Abstractions;
using Moq;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders.DtoCaches
{
    public class SyncDtoCacheMockBuilder<TDto> : BaseDtoCacheMockBuilder<SyncDtoCacheMockBuilder<TDto>,ISyncDtoCache<TDto>,TDto> where TDto : IClientEntity
    {
        public void VerifySaveDtosAsync(TDto dto)
        {
            Mock.Verify(x => x.SaveDtosAsync(It.Is<SyncResponse<TDto>>(y => 
                y.Dtos.FirstOrDefault(z => z.Equals(dto)) != null)));
        }
        
        public void VerifySaveDtosAsync(List<TDto> dtos)
        {
            foreach (var dto in dtos)
            {
                Mock.Verify(x => x.SaveDtosAsync(It.Is<SyncResponse<TDto>>(y => 
                    y.Dtos.FirstOrDefault(z => z.Equals(dto)) != null)));
            }
        }
        
        public void VerifySaveDtosAsync(params TDto[] dtos)
        {
            foreach (var dto in dtos)
            {
                Mock.Verify(x => x.SaveDtosAsync(It.Is<SyncResponse<TDto>>(y => 
                    y.Dtos.FirstOrDefault(z => z.Equals(dto)) != null)));
            }
        }
    }
}