using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Lab8Csharp.model;
using Lab8Csharp.persistence;
using Lab8Csharp.services;

namespace Lab8Csharp.server.server
{
    public class ServicesImpl : IServices
    {
        private readonly IUserRepo _userRepo;
        private readonly IParticipantRepo _participantRepo;
        private readonly IProbaRepo _probaRepo;
        private readonly IInscriereRepo _inscriereRepo;

        private readonly ConcurrentDictionary<string, IObserver> _loggedClients;
        private int _clientNo = 1;
        private readonly List<string> _users = new();

        public ServicesImpl(
            UserDBRepository userRepo,
            ParticipantDBRepository participantRepo,
            ProbaDBRepository probaRepo,
            InscriereDBRepository inscriereRepo)
        {
            _userRepo = userRepo;
            _participantRepo = participantRepo;
            _probaRepo = probaRepo;
            _inscriereRepo = inscriereRepo;
            _loggedClients = new ConcurrentDictionary<string, IObserver>();
        }

        public bool Authenticate(string username, string password)
        {
            var user = _userRepo.FindByUsername(username);
            if (user == null)
                return false;

            return user.HashedPassword == password;
        }

        public int? CountParticipants(long probaId)
        {
            return _inscriereRepo.CountParticipantsByProba(probaId);
        }

        public ProbaDTO[] GetAllProbe()
        {
            var probe = _probaRepo.FindAll();
            var probeDTO = new List<ProbaDTO>();

            foreach (var p in probe)
            {
                var dto = new ProbaDTO(p.Distanta, p.Stil, CountParticipants(p.Id));
                dto.Id = p.Id;
                probeDTO.Add(dto);
            }

            return probeDTO.ToArray();
        }

        // public List<Participant> GetAllParticipants()
        // {
        //     return (List<Participant>)_participantRepo.FindAll();
        // }

        public List<Participant> GetParticipantsForProba(long probaId)
        {
            return _inscriereRepo.FindParticipantsByProba(probaId);
        }

        public Participant FindOrCreateParticipant(string name, int age)
        {
            var existing = _participantRepo.FindByNameAndAge(name, age);
            if (existing != null)
                return existing;

            _participantRepo.Save(new Participant(name, age));
            return _participantRepo.FindByNameAndAge(name, age);
        }

        public void RegisterParticipantToProba(string name, int? age, List<long> probeIds)
        {
            var participant = FindOrCreateParticipant(name, age ?? 0);
            foreach (var probaId in probeIds)
            {
                var proba = _probaRepo.FindOne(probaId);
                _inscriereRepo.Save(new Inscriere(participant, proba));
            }

            foreach (var observer in _loggedClients.Values)
            {
                observer.ParticipantAdded(new List<ProbaDTO>(GetAllProbe()));
            }
        }

        public void Login(User user)
        {
            var found = _userRepo.FindByUsername(user.Username);
            if (found == null || found.HashedPassword != user.HashedPassword)
                throw new Exception("User not found");

            if (_users.Contains(user.Username))
                throw new Exception("User already logged in");

            _users.Add(user.Username);
        }

        public void AddObserver(IObserver observer)
        {
            _loggedClients.TryAdd(_clientNo.ToString(), observer);
            _clientNo++;
        }
    }
}
