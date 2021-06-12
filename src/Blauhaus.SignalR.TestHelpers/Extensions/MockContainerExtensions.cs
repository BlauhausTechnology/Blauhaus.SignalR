using System;
using Blauhaus.Common.Abstractions;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.TestHelpers.MockBuilders.DtoCaches;
using Blauhaus.SignalR.TestHelpers.MockBuilders.DtoSavers;
using Blauhaus.SignalR.TestHelpers.MockBuilders.SignalRClients;
using Blauhaus.TestHelpers;

namespace Blauhaus.SignalR.TestHelpers.Extensions
{
    public static class MockContainerExtensions
    {
        public static Func<DtoCacheMockBuilder<TDto, TId>> AddMockDtoCache<TDto, TId>(this MockContainer mocks) where TDto : class, IHasId<TId>
        {
            return mocks.AddMock<DtoCacheMockBuilder<TDto, TId>, IDtoCache<TDto, TId>>();
        }

        public static Func<DtoHandlerMockBuilder<TDto, TId>> AddMockDtoHandler<TDto, TId>(this MockContainer mocks) where TDto : class, IHasId<TId>
        {
            return mocks.AddMock<DtoHandlerMockBuilder<TDto, TId>, IDtoHandler<TDto>>();
        }
         
        public static Func<SignalRDtoClientMockBuilder<TDto>> AddMockSignalRDtoClient<TDto>(this MockContainer mocks) where TDto : class
        {
            return mocks.AddMock<SignalRDtoClientMockBuilder<TDto>, ISignalRDtoClient<TDto>>();
        }
         
    }
}