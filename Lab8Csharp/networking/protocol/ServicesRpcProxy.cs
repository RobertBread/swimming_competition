using System;
using System.Collections.Concurrent;
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
    public class ServicesRpcProxy : IServices
    {
        private readonly string _host;
        private readonly int _port;
        private List<IObserver> _clients;
        private TcpClient _connection;
        private NetworkStream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;
        private BlockingCollection<Response> _qresponses = new();
        private volatile bool _finished;

        public ServicesRpcProxy(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public void Login(User user)
        {
            _clients = new();
            InitializeConnection();
            var request = new Request.Builder().Type(RequestType.LOGIN).Data(user).Build();
            SendRequest(request);
            var response = ReadResponse();
            if (response.Type == ResponseType.ERROR)
            {
                CloseConnection();
                throw new Exception(response.Data.ToString());
            }
        }

        public void AddObserver(IObserver observer)
        {
            _clients.Add(observer);
            var request = new Request.Builder().Type(RequestType.ADD_CLIENT).Build();
            SendRequest(request);
            var response = ReadResponse();
            if (response.Type == ResponseType.ERROR)
            {
                CloseConnection();
                throw new Exception(response.Data.ToString());
            }
        }

        public void RegisterParticipantToProba(string name, int? age, List<long> probaIds)
        {
            var participant = new model.Participant(name, age);
            var inscrieri = new List<Inscriere>();
            foreach (var id in probaIds)
            {
                var proba = new Proba { Id = id };
                inscrieri.Add(new Inscriere(participant, proba));
            }

            var request = new Request.Builder().Type(RequestType.ADD_PARTICIPANT).Data(inscrieri).Build();
            SendRequest(request);
            var response = ReadResponse();
            if (response.Type == ResponseType.ERROR)
            {
                CloseConnection();
                throw new Exception(response.Data.ToString());
            }
        }

        public ProbaDTO[] GetAllProbe()
        {
            var request = new Request.Builder().Type(RequestType.GET_PROBE).Build();
            SendRequest(request);
            var response = ReadResponse();
            if (response.Type == ResponseType.ERROR)
                throw new Exception(response.Data.ToString());

            return JsonSerializer.Deserialize<ProbaDTO[]>(response.Data.ToString());
        }

        public int? CountParticipants(long probaId)
        {
            var request = new Request.Builder().Type(RequestType.GET_PROBE_COUNT).Data(probaId).Build();
            SendRequest(request);
            var response = ReadResponse();
            if (response.Type == ResponseType.ERROR)
                throw new Exception(response.Data.ToString());

            return JsonSerializer.Deserialize<int>(response.Data.ToString());
        }

        public List<Participant> GetParticipantsForProba(long probaId)
        {
            var request = new Request.Builder().Type(RequestType.GET_PARTICIPANTS).Data(probaId).Build();
            SendRequest(request);
            var response = ReadResponse();
            if (response.Type == ResponseType.ERROR)
                throw new Exception(response.Data.ToString());

            return JsonSerializer.Deserialize<List<Participant>>(response.Data.ToString());
        }

        public bool Authenticate(string username, string password) => false;

        public Participant FindOrCreateParticipant(string name, int age) => null;

        private void InitializeConnection()
        {
            try
            {
                _connection = new TcpClient(_host, _port);
                _stream = _connection.GetStream();
                _writer = new StreamWriter(_stream, Encoding.UTF8) { AutoFlush = true };
                _reader = new StreamReader(_stream, Encoding.UTF8);
                _finished = false;
                StartReader();
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
            }
        }

        private void CloseConnection()
        {
            _finished = true;
            try
            {
                _reader?.Close();
                _writer?.Close();
                _stream?.Close();
                _connection?.Close();
                _clients = null;
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
            }
        }

        private void SendRequest(Request request)
        {
            try
            {
                // string json = JsonSerializer.Serialize(request);
                string json = JsonSerializer.Serialize(request) + "\n";
                _writer.Write(json);
            }
            catch (IOException e)
            {
                throw new Exception("Error sending request", e);
            }
        }

        private Response ReadResponse()
        {
            try
            {
                return _qresponses.Take();
            }
            catch (Exception e)
            {
                throw new Exception("Error reading response", e);
            }
        }

        private void StartReader()
        {
            var thread = new Thread(() =>
            {
                while (!_finished)
                {
                    try
                    {
                        string line = _reader.ReadLine();
                        if (line != null)
                        {
                            var response = JsonSerializer.Deserialize<Response>(line);
                            Console.WriteLine("response received " + response);
                            if (IsUpdate(response))
                            {
                                HandleUpdate(response);
                            }
                            else
                            {
                                _qresponses.Add(response);
                            }
                        }
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine("Reading error: " + e);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Unexpected error: " + e);
                    }
                }
            });
            thread.Start();
        }

        private void HandleUpdate(Response response)
        {
            if (response.Type == ResponseType.PARTICIPANT_ADDED)
            {
                Console.WriteLine("S-a adaugat ceva");

                try
                {
                    var lista = JsonSerializer.Deserialize<List<ProbaDTO>>(response.Data.ToString());
                    foreach (var c in _clients)
                    {
                        c.ParticipantAdded(lista);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Client update error: " + e);
                }
            }
        }

        private bool IsUpdate(Response response)
        {
            return response.Type == ResponseType.PARTICIPANT_ADDED;
        }
    }
}
