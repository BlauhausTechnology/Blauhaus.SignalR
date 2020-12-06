using System;

namespace Blauhaus.SignalR.Tests.TestObjects
{
    public class MyDto
    {
        public MyDto()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
    }
}