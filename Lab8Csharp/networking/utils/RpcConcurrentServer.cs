using System;
using System.Net.Sockets;
using System.Threading;
using Lab8Csharp.networking.protocol;
using Lab8Csharp.services;

namespace Lab8Csharp.networking.utils
{
    public class RpcConcurrentServer : AbsConcurrentServer
    {
        private readonly IServices _chatServer;

        public RpcConcurrentServer(int port, IServices chatServer) : base(port)
        {
            _chatServer = chatServer;
            Console.WriteLine("Chat - RpcConcurrentServer");
        }

        protected override Thread CreateWorker(TcpClient client)
        {
            ClientRpcWorker worker = new ClientRpcWorker(_chatServer, client);
            return new Thread(worker.Run);
        }

        public new void Stop()
        {
            Console.WriteLine("Stopping services ...");
            base.Stop();
        }
    }
}