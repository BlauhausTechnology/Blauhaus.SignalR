using System.Collections.Generic;
using System.Linq;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Abstractions.Sync;
using Moq;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders.DtoCaches
{
    public class SyncDtoCacheMockBuilder<TDto> : BaseDtoCacheMockBuilder<SyncDtoCacheMockBuilder<TDto>,ISyncDtoCache<TDto>,TDto> where TDto : class, IClientEntity
    {

        public SyncDtoCacheMockBuilder()
        {
            Where_LoadSyncRequestAsync_returns(new SyncRequest());
        }

        public SyncDtoCacheMockBuilder<TDto> Where_LoadSyncRequestAsync_returns(SyncRequest request)
        {
            Mock.Setup(x => x.LoadSyncRequestAsync()).ReturnsAsync(request);
            return this;
        }
        
        
        public void VerifySaveDtosAsync(TDto dto)
        {
            Mock.Verify(x => x.SaveSyncResponseAsync(It.Is<SyncResponse<TDto>>(y => 
                y.Dtos.FirstOrDefault(z => z.Equals(dto)) != null)));
        }
        
        public void VerifySaveDtosAsync(List<TDto> dtos)
        {
            foreach (var dto in dtos)
            {
                Mock.Verify(x => x.SaveSyncResponseAsync(It.Is<SyncResponse<TDto>>(y => 
                    y.Dtos.FirstOrDefault(z => z.Equals(dto)) != null)));
            }
        }
        
        public void VerifySaveDtosAsync(params TDto[] dtos)
        {
            foreach (var dto in dtos)
            {
                Mock.Verify(x => x.SaveSyncResponseAsync(It.Is<SyncResponse<TDto>>(y => 
                    y.Dtos.FirstOrDefault(z => z.Equals(dto)) != null)));
            }
        }
    }
}