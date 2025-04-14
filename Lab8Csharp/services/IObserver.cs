using System;
using System.Collections.Generic;
using Lab8Csharp.model;

namespace Lab8Csharp.services
{
    public interface IObserver
    {
        void ParticipantAdded(List<ProbaDTO> probaDtoList);
        
        // Dacă vei folosi metodele comentate ulterior, le poți adăuga astfel:
        // void FriendLoggedIn(User user);
        // void FriendLoggedOut(User user);
    }
}