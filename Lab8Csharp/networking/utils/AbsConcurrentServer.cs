using System;
using System.Net.Sockets;
using System.Threading;

namespace Lab8Csharp.networking.utils
{
    public abstract class AbsConcurrentServer : AbstractServer
    {
        protected AbsConcurrentServer(int port) : base(port)
        {
            Console.WriteLine("Concurrent AbstractServer");
        }

        protected override void ProcessRequest(TcpClient client)
        {
            Thread worker = CreateWorker(client);
            worker.Start();
        }

        protected abstract Thread CreateWorker(TcpClient client);
    }
}