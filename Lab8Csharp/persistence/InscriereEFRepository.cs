using Lab8Csharp.model;
using Lab8Csharp.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab8Csharp.persistence;

public class InscriereEFRepository : IInscriereRepo
{
    private readonly InotContextE _context;

    public InscriereEFRepository(InotContextE context)
    {
        _context = context;
    }

    public Inscriere FindOne(long id)
    {
        var entity = _context.Inscrieres
            .Include(i => i.Participant)
            .Include(i => i.Proba)
            .FirstOrDefault(i => i.Id == id);

        return entity != null ? MapToModel(entity) : null;
    }

    public IEnumerable<Inscriere> FindAll()
    {
        return _context.Inscrieres
            .Include(i => i.Participant)
            .Include(i => i.Proba)
            .Select(MapToModel)
            .ToList();
    }

    public List<Participant> FindParticipantsByProba(long probaId)
    {
        return _context.Inscrieres
            .Include(i => i.Participant)
            .Where(i => i.ProbaId == probaId)
            .Select(i => new Participant(i.Participant.Name, i.Participant.Age) { Id = i.Participant.Id })
            .Distinct()
            .ToList();
    }

    public int CountParticipantsByProba(long probaId)
    {
        return _context.Inscrieres.Count(i => i.ProbaId == probaId);
    }

    public Inscriere Save(Inscriere inscriere)
    {
        var entity = new InscriereE
        {
            ParticipantId = (int)inscriere.Participant.Id,
            ProbaId = (int)inscriere.Proba.Id
        };
        _context.Inscrieres.Add(entity);
        _context.SaveChanges();
        inscriere.Id = entity.Id;
        return inscriere;
    }

    public Inscriere Delete(long id)
    {
        var entity = _context.Inscrieres.Find(id);
        if (entity == null) return null;
        _context.Inscrieres.Remove(entity);
        _context.SaveChanges();
        return MapToModel(entity);
    }

    public Inscriere Update(Inscriere inscriere)
    {
        var entity = _context.Inscrieres.Find(inscriere.Id);
        if (entity == null) return null;
        entity.ParticipantId = (int)inscriere.Participant.Id;
        entity.ProbaId = (int)inscriere.Proba.Id;
        _context.SaveChanges();
        return inscriere;
    }

    private Inscriere MapToModel(InscriereE entity)
    {
        var participant = new Participant(entity.Participant.Name, entity.Participant.Age) { Id = entity.Participant.Id };
        var proba = new Proba(entity.Proba.Distanta, entity.Proba.Stil) { Id = entity.Proba.Id };
        return new Inscriere(participant, proba) { Id = entity.Id };
    }
}
