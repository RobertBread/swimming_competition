namespace Lab8Csharp.persistence;

public interface IRepository<ID, E>
{
    E FindOne(ID id);
    IEnumerable<E> FindAll();
    E Save(E entity);
    E Delete(ID id);
    E Update(E entity);
}