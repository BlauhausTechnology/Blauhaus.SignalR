using System;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.TestHelpers.MockBuilders;
using Blauhaus.TestHelpers;

namespace Blauhaus.SignalR.TestHelpers.Extensions
{
    public static class MockContainerExtensions
    {
        public static Func<DtoCacheMockBuilder<TDto>> AddMockDtoCache<TDto>(this MockContainer mocks)
        {
            return mocks.AddMock<DtoCacheMockBuilder<TDto>, IDtoCache<TDto>>();
        }
        
        public static Func<SignalRClientMockBuilder<TDto>> AddMockSignalRClient<TDto>(this MockContainer mocks)
        {
            return mocks.AddMock<SignalRClientMockBuilder<TDto>, ISignalRClient<TDto>>();
        }
    }
}