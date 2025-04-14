using System;

namespace Lab8Csharp.model
{
    [Serializable]
    public class Inscriere : Entity<long>
    {
        public Participant Participant { get; set; }
        public Proba Proba { get; set; }

        public Inscriere() {}

        public Inscriere(Participant participant, Proba proba)
        {
            Participant = participant;
            Proba = proba;
        }
    }
}