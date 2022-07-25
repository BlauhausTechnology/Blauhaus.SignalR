using System;
using Blauhaus.Errors;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public static class SignalRErrors
    {
        public static Error FailedToExtractUser = Error.Create("Unable to identify current user");
        public static Error HubConnectionFailed = Error.Create("Failed to connect to the server hub");
        public static Error NoInternet = Error.Create("There is no internet connection available currently");
        public static Error InvocationFailure(Exception e) => Error.Create("An unexpected error occured when calling the server: " + e.Message);
    }
}