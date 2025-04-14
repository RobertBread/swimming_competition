using Lab8Csharp.model;

namespace Lab8Csharp.persistence;

public interface IUserRepo : IRepository<long,User>
{
    public User FindByUsername(string username);
}