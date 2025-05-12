using Lab8Csharp.model;
using Lab8Csharp.Models;
using Lab8Csharp.persistence;

public class UserEFRepository : IUserRepo
{
    private readonly InotContextE _context;

    public UserEFRepository(InotContextE context)
    {
        _context = context;
    }

    public User FindOne(long id)
    {
        var entity = _context.Users.Find(id);
        return entity != null ? MapToModel(entity) : null;
    }

    public IEnumerable<User> FindAll()
    {
        return _context.Users
            .Select(MapToModel)
            .ToList();
    }

    public User FindByUsername(string username)
    {
        var entity = _context.Users.FirstOrDefault(u => u.Username == username);
        return entity != null ? MapToModel(entity) : null;
    }

    public User Save(User user)
    {
        var entity = new UserE { Username = user.Username, HashedPassword = user.HashedPassword };
        _context.Users.Add(entity);
        _context.SaveChanges();
        user.Id = entity.Id;
        return user;
    }

    public User Delete(long id)
    {
        var entity = _context.Users.Find(id);
        if (entity == null) return null;
        _context.Users.Remove(entity);
        _context.SaveChanges();
        return MapToModel(entity);
    }

    public User Update(User user)
    {
        var entity = _context.Users.Find(user.Id);
        if (entity == null) return null;
        entity.Username = user.Username;
        entity.HashedPassword = user.HashedPassword;
        _context.SaveChanges();
        return user;
    }

    private static User MapToModel(UserE entity)
    {
        return new User(entity.Username, entity.HashedPassword) { Id = entity.Id };
    }
}
