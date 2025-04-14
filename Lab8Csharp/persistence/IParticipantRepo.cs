

using Lab8Csharp.model;

namespace Lab8Csharp.persistence;

public interface IParticipantRepo : IRepository<long, Participant>
{
    public Participant FindByNameAndAge(string name, int age);
}