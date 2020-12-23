﻿using System;
using Blauhaus.Domain.Abstractions.Entities;

namespace Blauhaus.SignalR.Tests.TestObjects
{
    public class MyDto : ISyncClientEntity
    {
        public MyDto()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        
        public EntityState EntityState { get; set; }
        public long ModifiedAtTicks { get; set;}
        public SyncState SyncState { get; set; }
    }
}