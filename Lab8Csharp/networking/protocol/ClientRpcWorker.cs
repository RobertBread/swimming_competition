using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using Lab8Csharp.model;
using Lab8Csharp.services;

namespace Lab8Csharp.networking.protocol
{
    public class ClientRpcWorker : IObserver
    {
        private readonly IServices _server;
        private readonly TcpClient _connection;
        private NetworkStream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;
        private volatile bool _connected;
        private static readonly Response OkResponse = new Response.Builder().Type(ResponseType.OK).Build();

        public ClientRpcWorker(IServices server, TcpClient connection)
        {
            _server = server;
            _connection = connection;

            try
            {
                _stream = _connection.GetStream();
                _reader = new StreamReader(_stream, Encoding.UTF8, leaveOpen: true);
                _writer = new StreamWriter(_stream, Encoding.UTF8, leaveOpen: true) { AutoFlush = true };
                _connected = true;
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
            }
        }

        public async void Run()
        {
            while (_connected)
            {
                try
                {
                    var line = await _reader.ReadLineAsync();
                    if (line == null)
                    {
                        Console.WriteLine("Client disconnected (null line). Closing connection.");
                        _connected = false;
                        break;
                    }

                    var request = JsonSerializer.Deserialize<Request>(line);
                    var response = HandleRequest(request);

                    if (response != null)
                    {
                        await SendResponseAsync(response);
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("Stream error: " + e.Message);
                    _connected = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected error: " + e);
                }

                try
                {
                    await Task.Delay(1000);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            try
            {
                _reader?.Close();
                _writer?.Close();
                _stream?.Close();
                _connection?.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine("Error closing connection: " + e);
            }
        }

        private Response HandleRequest(Request request)
        {
            try
            {
                switch (request.Type)
                {
                    case RequestType.LOGIN:
                        Console.WriteLine("Login request ...");
                        User user = JsonSerializer.Deserialize<User>(request.Data.ToString());
                        _server.Login(user);
                        return OkResponse;

                    case RequestType.ADD_CLIENT:
                        Console.WriteLine("Add client request ...");
                        _server.AddObserver(this);
                        return OkResponse;

                    case RequestType.GET_PROBE:
                        var probeList = _server.GetAllProbe();
                        return new Response.Builder().Type(ResponseType.GET_PROBE).Data(probeList).Build();

                    case RequestType.GET_PROBE_COUNT:
                        var count = _server.CountParticipants(JsonSerializer.Deserialize<long>(request.Data.ToString()));
                        return new Response.Builder().Data(count).Build();

                    case RequestType.GET_PARTICIPANTS:
                        var probaId = JsonSerializer.Deserialize<long>(request.Data.ToString());
                        var participants = _server.GetParticipantsForProba(probaId);
                        return new Response.Builder().Data(participants).Build();

                    case RequestType.ADD_PARTICIPANT:
                        Console.WriteLine("Add participant request");
                        var inscrieri = JsonSerializer.Deserialize<List<Inscriere>>(request.Data.ToString());
                        var probaIds = new List<long>();
                        foreach (var i in inscrieri)
                        {
                            probaIds.Add(i.Proba.Id);
                        }

                        _server.RegisterParticipantToProba(inscrieri[0].Participant.Name, inscrieri[0].Participant.Age, probaIds);
                        Thread.Sleep(1000);
                        return new Response.Builder().Type(ResponseType.OK).Data(inscrieri).Build();

                    default:
                        return null;
                }
            }
            catch (Exception e)
            {
                _connected = false;
                return new Response.Builder().Type(ResponseType.ERROR).Data(e.Message).Build();
            }
        }

        private async Task SendResponseAsync(Response response)
        {
            try
            {
                string json = JsonSerializer.Serialize(response);
                await _writer.WriteLineAsync(json);
            }
            catch (IOException)
            {
                Console.WriteLine("Client disconnected or stream corrupted. Removing worker...");
                _connected = false;
            }
        }

        public void ParticipantAdded(List<ProbaDTO> lista)
        {
            var resp = new Response.Builder().Type(ResponseType.PARTICIPANT_ADDED).Data(lista).Build();
            Console.WriteLine("Notific: participant adăugat");
            _ = SendResponseAsync(resp); // fire-and-forget
        }
    }
}
