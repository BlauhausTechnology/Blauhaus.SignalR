﻿using System;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.TestHelpers.MockBuilders.DtoCaches;
using Blauhaus.SignalR.TestHelpers.MockBuilders.SignalRClients;
using Blauhaus.TestHelpers;

namespace Blauhaus.SignalR.TestHelpers.Extensions
{
    public static class MockContainerExtensions
    {
        public static Func<DtoCacheMockBuilder<TDto>> AddMockDtoCache<TDto>(this MockContainer mocks) where TDto : class
        {
            return mocks.AddMock<DtoCacheMockBuilder<TDto>, IDtoCache<TDto>>();
        }
        
        public static Func<SyncDtoCacheMockBuilder<TDto>> AddMockSyncDtoCache<TDto>(this MockContainer mocks) where TDto : class, IClientEntity
        {
            return mocks.AddMock<SyncDtoCacheMockBuilder<TDto>, ISyncDtoCache<TDto>>();
        }
        
        public static Func<SignalRClientMockBuilder<TDto>> AddMockSignalRClient<TDto>(this MockContainer mocks) where TDto : class
        {
            return mocks.AddMock<SignalRClientMockBuilder<TDto>, ISignalRClient<TDto>>();
        }
        
        public static Func<SignalRSyncClientMockBuilder<TDto>> AddMockSignalRSyncClient<TDto>(this MockContainer mocks) where TDto : class, IClientEntity
        {
            return mocks.AddMock<SignalRSyncClientMockBuilder<TDto>, ISignalRSyncClient<TDto>>();
        }
    }
}