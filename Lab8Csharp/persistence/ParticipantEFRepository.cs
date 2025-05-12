using Lab8Csharp.model;
using Lab8Csharp.Models;

namespace Lab8Csharp.persistence;

public class ParticipantEFRepository : IParticipantRepo
{
    private readonly InotContextE _context;

    public ParticipantEFRepository(InotContextE context)
    {
        _context = context;
    }

    public Participant FindOne(long id)
    {
        var entity = _context.Participants.Find(id);
        return entity != null ? MapToModel(entity) : null;
    }

    public IEnumerable<Participant> FindAll()
    {
        return _context.Participants
            .Select(MapToModel)
            .ToList();
    }

    public Participant FindByNameAndAge(string name, int age)
    {
        var entity = _context.Participants.FirstOrDefault(p => p.Name == name && p.Age == age);
        return entity != null ? MapToModel(entity) : null;
    }

    public Participant Save(Participant participant)
    {
        var entity = new ParticipantE { Name = participant.Name, Age = participant.Age.GetValueOrDefault() };
        _context.Participants.Add(entity);
        _context.SaveChanges();
        participant.Id = entity.Id;
        return participant;
    }

    public Participant Delete(long id)
    {
        var entity = _context.Participants.Find(id);
        if (entity == null) return null;
        _context.Participants.Remove(entity);
        _context.SaveChanges();
        return MapToModel(entity);
    }

    public Participant Update(Participant participant)
    {
        var entity = _context.Participants.Find(participant.Id);
        if (entity == null) return null;
        entity.Name = participant.Name;
        entity.Age = participant.Age.GetValueOrDefault();
        _context.SaveChanges();
        return participant;
    }

    private static Participant MapToModel(ParticipantE entity)
    {
        return new Participant(entity.Name, entity.Age) { Id = entity.Id };
    }
}
