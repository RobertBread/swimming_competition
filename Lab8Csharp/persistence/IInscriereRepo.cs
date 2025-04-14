using Lab8Csharp.model;

namespace Lab8Csharp.persistence;


public interface IInscriereRepo : IRepository<long,Inscriere>
{
    public List<Participant> FindParticipantsByProba(long probaId);
    public int CountParticipantsByProba(long probaId);
}