using Lab8Csharp.model;
using Lab8Csharp.Models;

namespace Lab8Csharp.persistence;

public class ProbaEFRepository : IProbaRepo
{
    private readonly InotContextE _context;

    public ProbaEFRepository(InotContextE context)
    {
        _context = context;
    }

    public Proba FindOne(long id)
    {
        var entity = _context.Probas.Find(id);
        return entity != null ? MapToModel(entity) : null;
    }

    public IEnumerable<Proba> FindAll()
    {
        return _context.Probas
            .Select(MapToModel)
            .ToList();
    }

    public Proba Save(Proba proba)
    {
        var entity = new ProbaE { Distanta = proba.Distanta, Stil = proba.Stil };
        _context.Probas.Add(entity);
        _context.SaveChanges();
        proba.Id = entity.Id;
        return proba;
    }

    public Proba Delete(long id)
    {
        var entity = _context.Probas.Find(id);
        if (entity == null) return null;
        _context.Probas.Remove(entity);
        _context.SaveChanges();
        return MapToModel(entity);
    }

    public Proba Update(Proba proba)
    {
        var entity = _context.Probas.Find(proba.Id);
        if (entity == null) return null;
        entity.Distanta = proba.Distanta;
        entity.Stil = proba.Stil;
        _context.SaveChanges();
        return proba;
    }

    private static Proba MapToModel(ProbaE entity)
    {
        return new Proba(entity.Distanta, entity.Stil) { Id = entity.Id };
    }
}
