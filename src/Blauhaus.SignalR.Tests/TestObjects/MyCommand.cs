using System;

namespace Blauhaus.SignalR.Tests.TestObjects
{
    public class MyCommand
    {
        public MyCommand()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
    }
}