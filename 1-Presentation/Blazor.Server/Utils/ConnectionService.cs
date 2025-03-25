using Microsoft.JSInterop;

namespace Blazor.Server
{
    public class ConnectionService
    {
        [JSInvokable]
        public static Task CheckConnection()
        {
            // This method doesn't need to do anything.
            // If it can be called, the SignalR connection is still up.
            return Task.CompletedTask;
        }
    }
}
