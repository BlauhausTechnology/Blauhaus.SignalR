using System;
using Blauhaus.Domain.Abstractions.Entities;

namespace Blauhaus.SignalR.Tests.TestObjects
{
    public class MyDto : IClientEntity<Guid>
    {
        public MyDto(Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        
        public EntityState EntityState { get; set; }
        public long ModifiedAtTicks { get; set; }

    }
}