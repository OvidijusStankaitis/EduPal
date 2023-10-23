using System.Linq.Expressions;
using PSI_Project.Models;

namespace PSI_Project.Repositories;

public interface IRepository<TEntity>
{
    TEntity Get(string id);
    IEnumerable<TEntity> GetAll();
    IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

    void Add(TEntity entity);
    void Remove(TEntity entity);
}