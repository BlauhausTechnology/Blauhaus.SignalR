using System;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.TestHelpers.MockBuilders;
using Blauhaus.TestHelpers;

namespace Blauhaus.SignalR.TestHelpers.Extensions
{
    public static class MockContainerExtensions
    {

         
        public static Func<SignalRDtoClientMockBuilder<TDto>> AddMockSignalRDtoClient<TDto>(this MockContainer mocks) where TDto : class
        {
            return mocks.AddMock<SignalRDtoClientMockBuilder<TDto>, ISignalRDtoClient<TDto>>();
        }
         
    }
}