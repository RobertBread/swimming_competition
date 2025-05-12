using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
// using Lab8Csharp.grpc;  // namespace generat de proto
using Lab8Csharp.model;
using Lab8Csharp.persistence;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Lab8Csharp.Models.ParticipantE;
using Microsoft.EntityFrameworkCore;
using Org.Example.Grpc;
using Lab8Csharp.Models;
//
namespace Lab8Csharp.server.server
{
    public class ServicesGrpcImpl : IServices.IServicesBase
    {
        private readonly IUserRepo _userRepo;
        private readonly IParticipantRepo _participantRepo;
        private readonly IProbaRepo _probaRepo;
        private readonly IInscriereRepo _inscriereRepo;

        private readonly ConcurrentDictionary<string, IServerStreamWriter<ProbeResponse>> _observers = new();
        private readonly HashSet<string> _loggedUsers = new();

        public ServicesGrpcImpl(IUserRepo userRepo, IParticipantRepo participantRepo, IProbaRepo probaRepo, IInscriereRepo inscriereRepo)
        {
            _userRepo = userRepo;
            _participantRepo = participantRepo;
            _probaRepo = probaRepo;
            _inscriereRepo = inscriereRepo;
        }

        public override Task<Empty> Login(GrpcUser request, ServerCallContext context)
        {
            var user = _userRepo.FindByUsername(request.Username);

            if (user == null || user.HashedPassword != request.HashedPassword)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Invalid username or password"));
            }

            if (_loggedUsers.Contains(request.Username))
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, "User already logged in"));
            }

            _loggedUsers.Add(request.Username);
            return Task.FromResult(new Empty());
        }

        public override Task<CountResponse> CountParticipants(ProbaIdRequest request, ServerCallContext context)
        {
            var count = _inscriereRepo.CountParticipantsByProba(request.ProbaId);
            return Task.FromResult(new CountResponse { Count = count });
        }

        public override Task<ProbeResponse> GetAllProbe(Empty request, ServerCallContext context)
        {
            var probeList = _probaRepo.FindAll();
            var response = new ProbeResponse();

            foreach (var p in probeList)
            {
                response.Probe.Add(new GrpcProbaDTO
                {
                    Distanta = p.Distanta,
                    Stil = p.Stil,
                    NrParticipanti = _inscriereRepo.CountParticipantsByProba(p.Id),
                    Id = p.Id
                });
            }

            return Task.FromResult(response);
        }

        public override Task<ParticipantsResponse> GetParticipantsForProba(ProbaIdRequest request, ServerCallContext context)
        {
            var participants = _inscriereRepo.FindParticipantsByProba(request.ProbaId);
            var response = new ParticipantsResponse();

            foreach (var p in participants)
            {
                response.Participants.Add(new GrpcParticipant
                {
                    Name = p.Name,
                    Age = p.Age.GetValueOrDefault()

                });
            }

            return Task.FromResult(response);
        }

        public override Task<Empty> RegisterParticipantToProba(RegisterParticipantRequest request, ServerCallContext context)
        {
            var participant = FindOrCreateParticipant(request.Name, request.Age);

            foreach (var probaId in request.ProbeIds)
            {
                var proba = _probaRepo.FindOne(probaId);
                _inscriereRepo.Save(new Inscriere(participant, proba));
            }

            _ = NotifyObserversAsync();  // rulează notificarea async, fără await

            return Task.FromResult(new Empty());
        }

        public override async Task ListenForUpdates(Empty request, IServerStreamWriter<ProbeResponse> responseStream, ServerCallContext context)
        {
            var key = context.Peer;
            _observers.TryAdd(key, responseStream);

            try
            {
                await Task.Delay(-1, context.CancellationToken);  // ține conexiunea deschisă până se anulează
            }
            catch (TaskCanceledException)
            {
                // clientul s-a deconectat
            }

            _observers.TryRemove(key, out _);
        }

        private Participant FindOrCreateParticipant(string name, int age)
        {
            var existing = _participantRepo.FindByNameAndAge(name, age);
            if (existing != null)
                return existing;

            _participantRepo.Save(new Participant(name, age));
            return _participantRepo.FindByNameAndAge(name, age);
        }

        private async Task NotifyObserversAsync()
        {
            var probeList = _probaRepo.FindAll();
            var response = new ProbeResponse();

            foreach (var p in probeList)
            {
                response.Probe.Add(new GrpcProbaDTO
                {
                    Distanta = p.Distanta,
                    Stil = p.Stil,
                    NrParticipanti = _inscriereRepo.CountParticipantsByProba(p.Id),
                    Id = p.Id
                });
            }

            foreach (var observer in _observers.Values.ToList())
            {
                try
                {
                    await observer.WriteAsync(response);
                }
                catch
                {
                    // client disconnected, remove observer
                }
            }
        }
    }
}


// public class ServicesGrpcImpl : IServices.IServicesBase
// {
//     private readonly InotContextE _context;
//     private readonly ConcurrentDictionary<string, IServerStreamWriter<ProbeResponse>> _observers = new();
//     private readonly HashSet<string> _loggedUsers = new();
//
//     public ServicesGrpcImpl(InotContextE context)
//     {
//         _context = context;
//     }
//
//     public override Task<Empty> Login(GrpcUser request, ServerCallContext context)
//     {
//         var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);
//         if (user == null || user.HashedPassword != request.HashedPassword)
//             throw new RpcException(new Status(StatusCode.PermissionDenied, "Invalid username or password"));
//
//         if (_loggedUsers.Contains(request.Username))
//             throw new RpcException(new Status(StatusCode.AlreadyExists, "User already logged in"));
//
//         _loggedUsers.Add(request.Username);
//         return Task.FromResult(new Empty());
//     }
//
//     public override Task<CountResponse> CountParticipants(ProbaIdRequest request, ServerCallContext context)
//     {
//         var count = _context.Inscrieres.Count(i => i.ProbaId == request.ProbaId);
//         return Task.FromResult(new CountResponse { Count = count });
//     }
//
//     public override Task<ProbeResponse> GetAllProbe(Empty request, ServerCallContext context)
//     {
//         var response = new ProbeResponse();
//
//         var probe = _context.Probas.ToList();
//         foreach (var p in probe)
//         {
//             var count = _context.Inscrieres.Count(i => i.ProbaId == p.Id);
//             response.Probe.Add(new GrpcProbaDTO
//             {
//                 Id = p.Id,
//                 Distanta = p.Distanta,
//                 Stil = p.Stil,
//                 NrParticipanti = count
//             });
//         }
//
//         return Task.FromResult(response);
//     }
//
//     public override Task<ParticipantsResponse> GetParticipantsForProba(ProbaIdRequest request, ServerCallContext context)
//     {
//         var response = new ParticipantsResponse();
//
//         var participants = _context.Inscrieres
//             .Include(i => i.Participant)
//             .Where(i => i.ProbaId == request.ProbaId)
//             .Select(i => i.Participant)
//             .Distinct()
//             .ToList();
//
//         foreach (var p in participants)
//         {
//             response.Participants.Add(new GrpcParticipant
//             {
//                 Name = p.Name,
//                 Age = p.Age
//             });
//         }
//
//         return Task.FromResult(response);
//     }
//
//     public override Task<Empty> RegisterParticipantToProba(RegisterParticipantRequest request, ServerCallContext context)
//     {
//         var participant = _context.Participants.FirstOrDefault(p => p.Name == request.Name && p.Age == request.Age);
//         if (participant == null)
//         {
//             participant = new ParticipantE { Name = request.Name, Age = request.Age };
//             _context.Participants.Add(participant);
//             _context.SaveChanges();
//         }
//
//         foreach (var probaId in request.ProbeIds)
//         {
//             var inscriere = new InscriereE
//             {
//                 ParticipantId = participant.Id,
//                 ProbaId = (int)probaId
//             };
//             _context.Inscrieres.Add(inscriere);
//         }
//
//         _context.SaveChanges();
//         _ = NotifyObserversAsync(); // async notify
//         return Task.FromResult(new Empty());
//     }
//
//     public override async Task ListenForUpdates(Empty request, IServerStreamWriter<ProbeResponse> responseStream, ServerCallContext context)
//     {
//         var key = context.Peer;
//         _observers.TryAdd(key, responseStream);
//
//         try
//         {
//             await Task.Delay(-1, context.CancellationToken);
//         }
//         catch (TaskCanceledException) { }
//
//         _observers.TryRemove(key, out _);
//     }
//
//     private async Task NotifyObserversAsync()
//     {
//         var response = new ProbeResponse();
//
//         var probe = _context.Probas.ToList();
//         foreach (var p in probe)
//         {
//             var count = _context.Inscrieres.Count(i => i.ProbaId == p.Id);
//             response.Probe.Add(new GrpcProbaDTO
//             {
//                 Id = p.Id,
//                 Distanta = p.Distanta,
//                 Stil = p.Stil,
//                 NrParticipanti = count
//             });
//         }
//
//         foreach (var observer in _observers.Values.ToList())
//         {
//             try
//             {
//                 await observer.WriteAsync(response);
//             }
//             catch { /* client deconectat */ }
//         }
//     }
// }
