using System;
using System.Collections.Generic;
using Lab8Csharp.model;

namespace Lab8Csharp.services
{
    public interface IServices
    {
        void Login(User user);
        void AddObserver(IObserver observer);
        void RegisterParticipantToProba(string name, int? age, List<long> probaIds);

        ProbaDTO[] GetAllProbe();
        int? CountParticipants(long probaId);
        bool Authenticate(string username, string password);
        Participant FindOrCreateParticipant(string name, int age);
        List<Participant> GetParticipantsForProba(long probaId);

        // Dacă vei folosi aceste metode în viitor, le poți decomenta:
        // void Logout(User user, IObserver observer);
        // User[] GetLoggedFriends(User user);
    }
}